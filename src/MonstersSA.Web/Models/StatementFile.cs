// Salvar como: MonstersSA.Web/Models/StatementFile.cs
using System;
using System.Collections.Generic;

namespace MonstersSA.Web.Models
{
    // Modelo que representa os arquivos.xlsx que foram processados
    public class StatementFile
    {
        public int StatementFileId { get; set; }
        public string OriginalFileName { get; set; } = null!;
        public DateTime UploadDateUtc { get; set; }
        public DateTime PeriodStartDate { get; set; } // Do cabeçalho do arquivo [1]
        public DateTime PeriodEndDate { get; set; } // Do cabeçalho do arquivo [1]

        public int PlayerId { get; set; }
        public virtual Player Player { get; set; } = null!; // PN: 1 arquivo.xlsx pertence a 1 player
        
        public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>(); // PN: 1 arquivo.xlsx possui N linhas de registro
    }
}