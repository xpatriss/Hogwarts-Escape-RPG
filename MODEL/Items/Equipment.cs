using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pastel;
using projektRPG.Infrastructure;
using projektRPG.Infrastructure.LogStorage;

namespace projektRPG.MODEL.Items
{
    /// <summary>
    /// Manages the player's internal inventory capacity and item storage collection.
    /// </summary>
    public class Equipment
    {
        private List<Item> equipmentList = new List<Item>();
        public IReadOnlyList<Item> EquipmentList => equipmentList;

        public bool IsFull => equipmentList.Count >= Consts.EquipmentLimit;

        public bool AddItem(Item item)
        {
            if (IsFull) return false;
            equipmentList.Add(item);
            return true;
        }

        public void RemoveItemAt(int index)
        {
            if (index >= 0 && index < equipmentList.Count)
            {
                equipmentList.RemoveAt(index);
            }
        }

        public void RemoveItem(Item item)
        {
            equipmentList.Remove(item);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            if (EquipmentList.Count == 0)
                return "";
            int i = 1;
            foreach (var item in EquipmentList)
            {
                sb.Append($"{item.ToString()}\n");
                i++;
            }

            return sb.ToString();
        }
    }
}