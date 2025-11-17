// Salvar como: MonstersSA.Web/Services/IReportsService.cs
using MonstersSA.Web.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MonstersSA.Web.Services
{
    public interface IReportsService
    {
        Task<List<TournamentPerformanceDto>> GetPerformanceSummaryAsync();
    }
}