using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using MonstersSA.Web.Data;
using MonstersSA.Web.Models;
using System.Globalization;

namespace MonstersSA.Web.Services;

public class StatementProcessingService
{
    // Serviço responsável por fazer a leitura do arquivo .xlsx
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

    public StatementProcessingService(IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        // A fábrica é injetada diretamente como se fosse o contexto direto
        _contextFactory = contextFactory;
    }

    public async Task ProcessStatementStreamAsync(Stream fileStream, string fileName, string playerName)
    {
        // Cria um contexto novo e descartável para esta operação
        using var context = await _contextFactory.CreateDbContextAsync();

        // Verificação de duplicidade pelo nome do arquivo
        if (await context.StatementFiles.AnyAsync(f => f.OriginalFileName == fileName))
        {
            return;
        }

        // Carrega TODAS as definições de torneio
        var allDefinitions = await context.TournamentDefinitions.ToListAsync();

        // Busca ou cria o jogador
        var player = await context.Players.FirstOrDefaultAsync(p => p.Name == playerName);
        if (player == null)
        {
            player = new Player { Name = playerName };
            context.Players.Add(player);
            await context.SaveChangesAsync(); // Salva para gerar o ID do novo player
        }

        // Mapeia os dados referentes ao arquivo para adicionar no banco
        var statementFile = new StatementFile
        {
            OriginalFileName = fileName,
            UploadDateUtc = DateTime.UtcNow,
            PlayerId = player.PlayerId,
            PeriodStartDate = DateTime.UtcNow,
            PeriodEndDate = DateTime.UtcNow
        };
        context.StatementFiles.Add(statementFile);

        // Usa o ClosedXML para ler o Stream do .xlsx
        using var workbook = new XLWorkbook(fileStream);
        var worksheet = workbook.Worksheets.First();
        
        // Variável para pular todas as linhas até achar o cabeçalho "Date" e puder começar a ler e armazenar no banco
        var rows = worksheet.RangeUsed().RowsUsed();
        
        var dataRows = rows
            .SkipWhile(row => row.Cell(1).GetValue<string>() != "Date")
            .Skip(1); // Pula a própria linha que contém "Date"

        // Se não sobrar nada, o arquivo está com formato errado ou vazio
        if (!dataRows.Any())
        {
            throw new Exception("Formato do arquivo inválido: Cabeçalho 'Date' não encontrado.");
        }

        var transactions = new List<Transaction>();

        foreach (var row in dataRows)
        {
            // Se a célula de data estiver vazia, para, pois é o fim do arquivo)
            if (row.Cell(1).IsEmpty()) continue;

            // Lendo a data para 
            var rawDate = row.Cell(1).GetDateTime();
            var transactionDateBrasilia = rawDate.AddHours(-3); // Comversão de UTC (horário da bodog) para horário de brasília, permitindo utilizar os horários de late register

            string description = row.Cell(2).GetValue<string>();
            string referenceId = row.Cell(3).GetValue<string>();

            if (!row.Cell(4).TryGetValue(out decimal cashAmount)) cashAmount = 0;
            if (!row.Cell(6).TryGetValue(out decimal points)) points = 0;

            if (!description.Contains("Poker Multi Table Tournament"))
                continue;

            // Mapeia os dados do registro (linha) do arquivo para adicionar no banco
            transactions.Add(new Transaction
            {
                TransactionDate = transactionDateBrasilia,
                Description = description,
                ReferenceId = referenceId,
                CashAmount = cashAmount,
                Points = points,
                StatementFile = statementFile
            });
        }

        // Agrupamento de transactions por arquivo 
        var groupedTransactions = transactions
            .GroupBy(t => t.ReferenceId)
            .ToList();

        var playedTournaments = new List<PlayedTournament>();

        // Lógica de Matching
        foreach (var group in groupedTransactions)
        {
            // Pega as transações de Buy-In (valor negativo)
            var buyIns = group.Where(t => t.CashAmount < 0).ToList();
            var payouts = group.Where(t => t.CashAmount > 0).ToList();

            // Se não tem buy-in (ex: ticket ou erro), continua, mas trata depois!!
            if (!buyIns.Any()) continue;

            // Pega o valor do primeiro buy-in para identificar o tipo
            decimal firstBuyInAmount = Math.Abs(buyIns.First().CashAmount);

            // Pega o horário do PRIMEIRO buy-in
            var firstBuyInDate = group.Min(t => t.TransactionDate);
            var buyInTime = TimeOnly.FromDateTime(firstBuyInDate);

            // Filtragem Inicial de torneios por Valor
            var candidates = allDefinitions
                .Where(d => d.BuyInAmount == firstBuyInAmount)
                .ToList();

            TournamentDefinition definition;

            if (candidates.Count == 0)
            {
                // Caso a transação não se encaixe em nenhum torneio no filtro por valor, é retornada como torneio não mapeado
                definition = new TournamentDefinition
                {
                    Name = $"NÃO MAPEADO (${firstBuyInAmount})",
                    BuyInAmount = firstBuyInAmount,
                    StartTime = buyInTime
                };
            }
            else if (candidates.Count == 1)
            {
                definition = candidates.First();
            }
            else
            {
                // LÓGICA DE TEMPO CIRCULAR!!!
                // Um relógio tem 24h, então a distância entre 23:00 e 01:00 é 2 horas, não 22 horas.
                definition = candidates.OrderBy(def =>
                {
                    double diffMinutes = Math.Abs((buyInTime - def.StartTime).TotalMinutes);
                    // A distância real é o menor valor entre a diferença direta e a volta no relógio
                    double circularDiff = Math.Min(diffMinutes, 1440 - diffMinutes);
                    return circularDiff;
                }).First();
            }

            decimal totalBuyIn = Math.Abs(buyIns.Sum(t => t.CashAmount));
            decimal totalPayout = payouts.Sum(t => t.CashAmount);

            // Por fim, mapeia uma instância de um torneio jogado e armazena ele no banco
            playedTournaments.Add(new PlayedTournament
            {
                ReferenceId = group.Key,
                StartDate = firstBuyInDate,
                TotalBuyIn = totalBuyIn,
                TotalPayout = totalPayout,
                NetResult = totalPayout - totalBuyIn,
                PlayerId = player.PlayerId,
                TournamentDefinition = definition
            });
        }

        // Adiciona tudo ao contexto e salva de uma vez
        context.Transactions.AddRange(transactions);
        context.PlayedTournaments.AddRange(playedTournaments);

        await context.SaveChangesAsync();
    }
}