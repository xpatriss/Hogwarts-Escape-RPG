using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace projektRPG.MODEL.Network
{
    /// <summary>
    /// Lightweight network snapshot of an enemy entity for client rendering.
    /// Pattern: DTO — shields authoritative AI logic and state from the client.
    /// </summary>
    public class EnemyDTO
    {
        public char Symbol { get; set; }
        public string Color { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public string Name { get; set; }
        public int HP { get; set; }
        public int Attack { get; set; }
        public int Armor { get; set; }
    }
}