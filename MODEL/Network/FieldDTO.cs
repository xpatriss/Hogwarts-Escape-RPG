using projektRPG.MODEL.Items;
using projektRPG.MODEL.Players.Enemies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace projektRPG.MODEL.Network
{
    /// <summary>
    /// Snapshot of an individual dungeon tile containing render symbols, present enemies, and ground items.
    /// Pattern: DTO.
    /// </summary>
    public class FieldDTO
    {
        public char Symbol { get; set; }
        public bool HasEnemy { get; set; }
        public int ItemsCount { get; set; }
        public bool IsAvailable { get; }

        public EnemyDTO? Enemy { get; set; }
        public List<ItemDTO> Items { get; set; } = new List<ItemDTO>();

        public string InfoText { get; set; }
    }
}