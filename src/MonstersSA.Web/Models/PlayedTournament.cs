// Salvar como: MonstersSA.Web/Models/PlayedTournament.cs
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MonstersSA.Web.Models
{
    // Modelo que representa uma instância de um torneio que foi jogado
    public class PlayedTournament
    {
        public long PlayedTournamentId { get; set; }
        public string ReferenceId { get; set; } = null!; // O ID do torneio (ex: "T649207919")
        public DateTime StartDate { get; set; } // A data do primeiro Buy-In

       
        public decimal TotalBuyIn { get; set; }  // Soma de todos os Buy-Ins (ex: 16.5)

       
        public decimal TotalPayout { get; set; } // Soma de todos os Payouts (ex: 8.71)

       
        public decimal NetResult { get; set; }   // Payout - BuyIn (ex: -7.79)

        public int PlayerId { get; set; }
        public virtual Player Player { get; set; } = null!; // Propriedade de navegação, 1 torneio jogado pertence a 1 jogador

        public int TournamentDefinitionId { get; set; }
        public virtual TournamentDefinition TournamentDefinition { get; set; } = null!;// Propriedade de navegação, uma instância de torneio se trata de apenas 1 definição de torneio no banco
    }
}