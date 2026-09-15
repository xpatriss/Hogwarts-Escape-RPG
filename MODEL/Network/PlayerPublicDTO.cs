using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace projektRPG.MODEL.Network
{
    /// <summary>
    /// Publicly visible player data broadcast to other participants in a multiplayer session.
    /// Omits private attributes, full inventory details, and personal logs.
    /// Pattern: DTO.
    /// </summary>
    public class PlayerPublicDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public char Symbol { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int HP { get; set; }
    }
}