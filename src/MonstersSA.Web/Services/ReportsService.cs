// Salvar como: MonstersSA.Web/Services/ReportsService.cs
using Microsoft.EntityFrameworkCore;
using MonstersSA.Web.Data;
using MonstersSA.Web.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MonstersSA.Web.Services
{
    public class ReportsService : IReportsService
    {
      // Serviço responsável por gerar o relatório a partir dos dados extraídos do processamento do .xlsx. Utiliza métodos LINQ que são convetidos pelo ef-core como comandos SQL que geram a view de relatório
      private readonly ApplicationDbContext _context;
      public ReportsService(ApplicationDbContext context) { _context = context; }

      public async Task<List<TournamentPerformanceDto>> GetPerformanceSummaryAsync()
      {
          // Consulta LINQ para agrupar por nome de torneio, resumir os resultados e gerar a view

          // DEBUG: Verificar quantos torneios existem no total
          var totalTournaments = await _context.PlayedTournaments.CountAsync();
          System.Console.WriteLine($"DEBUG: Total de torneios jogados no banco: {totalTournaments}");
          // DEBUG: Listar todos os torneios com seus nomes
          var allTournaments = await _context.PlayedTournaments
              .Include(pt => pt.TournamentDefinition)
              .Select(pt => new { pt.TournamentDefinition.Name, pt.NetResult })
              .ToListAsync();
          
          System.Console.WriteLine("DEBUG: Lista de todos os torneios encontrados:");
          foreach (var tournament in allTournaments)
          {
              System.Console.WriteLine($"  - {tournament.Name}: ${tournament.NetResult}");
          }
          
          var summary = await _context.PlayedTournaments
            .Include(pt => pt.TournamentDefinition) // Inclui os dados do Torneio
            .GroupBy(pt => pt.TournamentDefinition.Name) // Agrupa por 
            .Select(group => new TournamentPerformanceDto
              {
                  TournamentName = group.Key,
                  TotalEntries = group.Count(),
                  TotalNetResult = group.Sum(pt => pt.NetResult),
                  TotalBuyIn = group.Sum(pt => pt.TotalBuyIn),
                  // Calcula o Retorno Sobre Investimento (ROI)
                  ROI = (group.Sum(pt => pt.TotalBuyIn) == 0)? 0 : (group.Sum(pt => pt.NetResult) / group.Sum(pt => pt.TotalBuyIn)) * 100
              })
            .OrderByDescending(dto => dto.ROI) // Ordena a partir dos melhores
            .ToListAsync();  
          
          // DEBUG: Verificar resultado do agrupamento
          System.Console.WriteLine($"DEBUG: Total de grupos (torneios únicos) no relatório: {summary.Count}");
          foreach (var result in summary)
          {
              System.Console.WriteLine($"  - {result.TournamentName}: {result.TotalEntries} entradas, Resultado: ${result.TotalNetResult}, ROI: {result.ROI:F2}%");
          }

          return summary;
        }
    }
}