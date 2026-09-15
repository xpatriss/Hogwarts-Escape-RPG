using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace projektRPG.MODEL.Network
{
    /// <summary>
    /// Aggregated state container representing an authoritative tick snapshot dispatched to a single client.
    /// Pattern: DTO — encapsulates shared world layout, peer summaries, and personalized player stats.
    /// </summary>
    public class GameStateDTO
    {
        public GameMapDTO Map { get; set; }
        public string? Description { get; set; }
        public List<PlayerPublicDTO> OtherPlayers { get; set; } = new List<PlayerPublicDTO>();
        public PlayerPrivateDTO MyState { get; set; }
    }
}