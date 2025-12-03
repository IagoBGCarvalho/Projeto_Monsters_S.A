using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using MonstersSA.Web.Data;
using MonstersSA.Web.Models;

namespace MonstersSA.Web.Services;

public class StatementProcessingService
{
    // Serviço responsável por fazer a leitura do arquivo .xlsx
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory; // A fábrica é injetada diretamente como se fosse o contexto direto

    public StatementProcessingService(IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task ProcessStatementStreamAsync(Stream fileStream, string fileName, string playerName)
    {
        // Cria um contexto novo e descartável para esta operação
        using var context = await _contextFactory.CreateDbContextAsync();

        // Verifica se o arquivo já foi processado para evitar duplicidade
        if (await context.StatementFiles.AnyAsync(f => f.OriginalFileName == fileName))
        {
            return; 
        }

        // Carrega TODAS as definições de torneio
        var allDefinitions = await context.TournamentDefinitions.ToListAsync(); 

        // Carrega TODAS as definições de torneio
        var player = await context.Players.FirstOrDefaultAsync(p => p.Name == playerName) ?? new Player { Name = playerName };
        
        if (player.PlayerId == 0) context.Players.Add(player);

        // Cria o registro do arquivo
        var statementFile = new StatementFile
        {
            OriginalFileName = fileName,
            UploadDateUtc = DateTime.UtcNow,
            Player = player,
            PeriodStartDate = DateTime.UtcNow, 
            PeriodEndDate = DateTime.UtcNow
        };
        context.StatementFiles.Add(statementFile);

        // Usa o ClosedXML para ler o Stream do .xlsx
        using (var workbook = new XLWorkbook(fileStream))
        {
            var worksheet = workbook.Worksheet(1);
            var rows = worksheet.RowsUsed().Skip(6);
            var transactions = new List<Transaction>();

            foreach (var row in rows)
            {
                try
                {
                    // Tenta ler a célula 6 (Coluna F). Se falhar (ex: vazia), pointsValue será 0.
                    row.Cell(6).TryGetValue(out decimal pointsValue);
                    var transaction = new Transaction
                    {
                        TransactionDate = row.Cell(1).GetValue<DateTime>(),
                        Description = row.Cell(2).GetValue<string>(),
                        ReferenceId = row.Cell(3).GetValue<string>(),
                        CashAmount = row.Cell(4).GetValue<decimal>(),
                        Points = pointsValue,
                        StatementFile = statementFile
                    };
                    transactions.Add(transaction);
                }
                catch (Exception ex) { Console.WriteLine(ex.Message); }
            }
            
            context.Transactions.AddRange(transactions);
            
            // Chama a transformação
            var playedTournaments = TransformTransactions(transactions, player, allDefinitions);
            
            // ADICIONA OS TORNEIOS CALCULADOS AO CONTEXTO
            context.PlayedTournaments.AddRange(playedTournaments);
            
            await context.SaveChangesAsync();
        }
    }

    private List<PlayedTournament> TransformTransactions(
        List<Transaction> transactions, 
        Player player, 
        List<TournamentDefinition> allDefinitions)
    {
        var tournamentTransactions = transactions
           .Where(t =>!string.IsNullOrEmpty(t.ReferenceId) && 
                (t.Description.StartsWith("Poker Multi Table Tournament") ||
                 t.Description.StartsWith("Poker Single Table Tournament") ||
                 t.Description.StartsWith("Poker Free Roll")))
           .ToList();

        var groupedByRefId = tournamentTransactions.GroupBy(t => t.ReferenceId);
        var playedTournaments = new List<PlayedTournament>();

        foreach (var group in groupedByRefId)
        {
            var buyIns = group.Where(t => t.Description.Contains("Buy-In")).ToList();
            var payouts = group.Where(t => t.Description.Contains("Cashout/Payout")).ToList();

            if (!buyIns.Any()) continue;

            // Usa .Last() para pegar o Buy-In original (o arquivo vem ordenado do mais novo para o antigo)
            var firstBuyInTransaction = buyIns.Last();
            decimal firstBuyInAmount = Math.Abs(firstBuyInTransaction.CashAmount);
            
            // Ajuste de Fuso Horário (-3h) para comparar UTC do arquivo com Horário de Brasília da Grade
            var buyInTime = TimeOnly.FromTimeSpan(firstBuyInTransaction.TransactionDate.TimeOfDay).AddHours(-3);

            // Lógica de busca do torneio correto
            var candidates = allDefinitions
               .Where(d => Math.Abs(d.BuyInAmount - firstBuyInAmount) < 0.01m)
               .ToList();

            TournamentDefinition? definition = null;

            if (candidates.Count == 0)
            {
                // Torneio não mapeado (cria um dummy para não quebrar)
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
                // Lógica de desempate por horário mais próximo
                definition = candidates.OrderBy(def => {
                    var diff = buyInTime.ToTimeSpan() - def.StartTime.ToTimeSpan();
                    // Se a diferença for negativa (ex: jogou às 01:00 num torneio de 23:00 do dia anterior)
                    if (diff < TimeSpan.Zero) diff += TimeSpan.FromHours(24);
                    return Math.Abs(diff.TotalHours);
                }).First();
            }

            decimal totalBuyIn = Math.Abs(buyIns.Sum(t => t.CashAmount));
            decimal totalPayout = payouts.Sum(t => t.CashAmount);

            playedTournaments.Add(new PlayedTournament
            {
                ReferenceId = group.Key,
                StartDate = group.Min(t => t.TransactionDate),
                TotalBuyIn = totalBuyIn,
                TotalPayout = totalPayout,
                NetResult = totalPayout - totalBuyIn,
                Player = player,
                TournamentDefinition = definition
            });
        }

        return playedTournaments; 
    }
}