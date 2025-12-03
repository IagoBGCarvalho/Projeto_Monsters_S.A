using Microsoft.EntityFrameworkCore;
using MonstersSA.Web.Data;
using MonstersSA.Web.Models;

namespace MonstersSA.Web.Services;

public class ReportsService
{
    // Serviço responsável por gerar o relatório a partir dos dados extraídos do processamento do .xlsx. Utiliza métodos LINQ que são convetidos pelo ef-core como comandos SQL que geram a view de relatório
      private readonly ApplicationDbContext _context;
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

    public ReportsService(IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<List<TournamentPerformanceDto>> GetPerformanceSummaryAsync()
    {
        // Consulta LINQ para agrupar por nome de torneio, resumir os resultados e gerar a view
        using var context = await _contextFactory.CreateDbContextAsync();

        return await context.PlayedTournaments
          .Include(pt => pt.TournamentDefinition) // Inclui os dados do Torneio
          .GroupBy(pt => pt.TournamentDefinition.Name)
          .Select(group => new TournamentPerformanceDto
            {
                TournamentName = group.Key,
                TotalEntries = group.Count(),
                TotalNetResult = group.Sum(pt => pt.NetResult),
                TotalBuyIn = group.Sum(pt => pt.TotalBuyIn),
                // Calcula o Retorno Sobre Investimento (ROI)
                ROI = (group.Sum(pt => pt.TotalBuyIn) == 0)? 0 : (group.Sum(pt => pt.NetResult) / group.Sum(pt => pt.TotalBuyIn)) * 100
            })
          .OrderByDescending(dto => dto.ROI)  // Ordena a partir dos torneios com melhor retorno sobre investimento
          .ToListAsync();
    }
}