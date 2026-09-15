using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace projektRPG.MODEL.Network
{
    /// <summary>
    /// Confidential player data dispatched exclusively to the owning client session.
    /// Contains complete RPG attributes, wallet status, inventory, equipped weapons, and combat event logs.
    /// Pattern: DTO.
    /// </summary>
    public class PlayerPrivateDTO
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Health { get; set; }
        public int Strength { get; set; }
        public int Luck { get; set; }
        public int Dexterity { get; set; }
        public int Agression { get; set; }
        public int Wisdom { get; set; }

        public int Coins { get; set; }
        public int Gold { get; set; }

        public List<string> InventoryDisplay { get; set; }
        public string LeftHandDisplay { get; set; }
        public string RightHandDisplay { get; set; }

        public List<string> NewLogs { get; set; }
    }
}