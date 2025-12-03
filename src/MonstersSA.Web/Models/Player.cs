using System.Collections.Generic;

namespace MonstersSA.Web.Models
{
    public class Player
    {
        // Modelo que representa o jogador
        public int PlayerId { get; set; }
        public string Name { get; set; } = null!; // "Iago Carvalho"

        public virtual ICollection<StatementFile> StatementFiles { get; set; } = new List<StatementFile>(); // PN: 1 jogador pode dar upload de N arquivos
        public virtual ICollection<PlayedTournament> PlayedTournaments { get; set; } = new List<PlayedTournament>(); // PN: 1 jogador pode ter N torneios jogados
    }
}