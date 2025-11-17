// Salvar como: MonstersSA.Web/Models/TournamentPerformanceDto.cs
namespace MonstersSA.Web.Models
{
    // Um DTO que serve para passar os dados para a View do relatório
    public class TournamentPerformanceDto
    {
        public string TournamentName { get; set; } = null!;
        public int TotalEntries { get; set; }
        public decimal TotalNetResult { get; set; }
        public decimal TotalBuyIn { get; set; }
        public decimal ROI { get; set; }
    }
}