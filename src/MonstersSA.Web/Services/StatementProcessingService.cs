// Salvar como: MonstersSA.Web/Services/StatementProcessingService.cs
using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using MonstersSA.Web.Data;
using MonstersSA.Web.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MonstersSA.Web.Services
{
    public class StatementProcessingService : IStatementProcessingService
    {
        // Serviço responsável por fazer a leitura do arquivo .xlsx
        private readonly ApplicationDbContext _context;

        public StatementProcessingService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task ProcessStatementStreamAsync(Stream fileStream, string fileName, string playerName)
        {
            var allDefinitions = await _context.TournamentDefinitions.ToListAsync(); // Carrega TODAS as definições de torneio

            var player = await _context.Players.FirstOrDefaultAsync(p => p.Name == playerName) ?? new Player { Name = playerName }; // Encontra ou cria o Jogador

            // Cria o registro do arquivo
            var statementFile = new StatementFile
            {
                OriginalFileName = fileName,
                UploadDateUtc = DateTime.UtcNow,
                Player = player,
                PeriodStartDate = DateTime.UtcNow, // Placeholder
                PeriodEndDate = DateTime.UtcNow   // Placeholder
            };
            _context.StatementFiles.Add(statementFile);

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
                        row.Cell(6).TryGetValue(out decimal pointsValue); // Tenta ler a célula 6 (Coluna F). Se falhar (ex: vazia), pointsValue será 0.

                        var transaction = new Transaction
                        {
                            TransactionDate = row.Cell(1).GetValue<DateTime>(),
                            Description = row.Cell(2).GetValue<string>(),
                            ReferenceId = row.Cell(3).GetValue<string>(),
                            CashAmount = row.Cell(4).GetValue<decimal>(),
                            Points = pointsValue, // Usa o valor seguro
                            StatementFile = statementFile
                        };
                        transactions.Add(transaction);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro ao ler linha: {ex.Message}");
                    }
                }
                
                _context.Transactions.AddRange(transactions);

                // Chama a lógica de Transformação (passando a LISTA de definições)
                var playedTournaments = TransformTransactions(transactions, player, allDefinitions);
                _context.PlayedTournaments.AddRange(playedTournaments);
                
                await _context.SaveChangesAsync();
            }
        }

        // Lógica de Transformação (Método Privado) ATUALIZADA
        private List<PlayedTournament> TransformTransactions(
            List<Transaction> transactions, 
            Player player, 
            List<TournamentDefinition> allDefinitions)
        {
            // Filtro expandido que inclui TODOS os tipos de torneio
            var tournamentTransactions = transactions
                .Where(t => !string.IsNullOrEmpty(t.ReferenceId) && 
                    (t.Description.StartsWith("Poker Multi Table Tournament") ||
                        t.Description.StartsWith("Poker Single Table Tournament") ||
                        t.Description.StartsWith("Poker Free Roll")))
                .ToList();

            Console.WriteLine($"Total de transações de torneio encontradas: {tournamentTransactions.Count}");

            var groupedByRefId = tournamentTransactions.GroupBy(t => t.ReferenceId); // Agrupa por ReferenceId

            Console.WriteLine($"Total de grupos (torneios únicos): {groupedByRefId.Count()}");

            var playedTournaments = new List<PlayedTournament>();

            foreach (var group in groupedByRefId)
            {
                var buyIns = group.Where(t => t.Description.Contains("Buy-In")).ToList();
                var payouts = group.Where(t => t.Description.Contains("Cashout/Payout")).ToList();

                // DEBUG: Log para cada grupo
                Console.WriteLine($"Grupo {group.Key}: {buyIns.Count} buy-ins, {payouts.Count} payouts");

                if (!buyIns.Any())
                {
                    Console.WriteLine($"AVISO: Grupo {group.Key} sem buy-in. Ignorando.");
                    continue;
                }

                // Pega o valor absoluto do primeiro buy-in
                decimal firstBuyInAmount = Math.Abs(buyIns.First().CashAmount);
                var buyInTime = TimeOnly.FromTimeSpan(buyIns.First().TransactionDate.TimeOfDay);

                // DEBUG
                Console.WriteLine($"Buy-in amount: {firstBuyInAmount}, Time: {buyInTime}");

                // Busca candidatos
                var candidates = allDefinitions
                    .Where(d => Math.Abs(d.BuyInAmount - firstBuyInAmount) < 0.01m)
                    .ToList();

                TournamentDefinition definition;

                if (candidates.Count == 0)
                {
                    Console.WriteLine($"AVISO: Nenhum candidato para buy-in ${firstBuyInAmount} às {buyInTime}");
                    
                    // Cria uma definição temporária para torneios não mapeados
                    definition = new TournamentDefinition 
                    { 
                        Name = $"TORNEIO NÃO MAPEADO (${firstBuyInAmount})", 
                        BuyInAmount = firstBuyInAmount,
                        StartTime = buyInTime
                    };
                }
                else if (candidates.Count == 1)
                {
                    definition = candidates.First();
                    Console.WriteLine($"Encontrado: {definition.Name}");
                }
                else
                {
                    // Lógica de desempate por horário
                    definition = candidates.OrderBy(def => {
                        var diff = buyInTime.ToTimeSpan() - def.StartTime.ToTimeSpan();
                        if (diff < TimeSpan.Zero) diff += TimeSpan.FromHours(24);
                        return Math.Abs(diff.TotalHours);
                    }).First();
                    
                    Console.WriteLine($"Múltiplos candidatos. Escolhido: {definition.Name}");
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

            Console.WriteLine($"Total de torneios processados: {playedTournaments.Count}");
            return playedTournaments;
        }
    }
}