using System.ComponentModel.DataAnnotations.Schema;

namespace MonstersSA.Web.Models
{
    // Tabela preenchida pelo data seeding do contexto que conecta um valor de buy-in ao nome de um torneio
    public class TournamentDefinition
    {
        public int TournamentDefinitionId { get; set; }
        public string Name { get; set; } = null!; // "$2000 LOW ROLLER"
        public decimal BuyInAmount { get; set; } // Ex: 16.5
        public TimeOnly StartTime { get; set; } // Horário de início do torneio
    }
}