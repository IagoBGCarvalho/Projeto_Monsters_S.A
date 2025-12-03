using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MonstersSA.Web.Models
{
    // Tabela que representa uma linha individual do arquivo.xlsx [1]
    public class Transaction
    {
        public long TransactionId { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Description { get; set; } = null!; // "Poker Multi Table Tournament Buy-In"
        public string ReferenceId { get; set; } = null!; //"T649207919"
        public decimal CashAmount { get; set; } // Ex: -16.5 ou 8.71
        public decimal Points { get; set; }
        public int StatementFileId { get; set; }
        public required virtual StatementFile StatementFile { get; set; }
    }
}