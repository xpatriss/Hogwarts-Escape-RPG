using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace projektRPG.MODEL.Network
{
    /// <summary>
    /// Serialized representation of a world item or inventory entry for display purposes.
    /// Pattern: DTO.
    /// </summary>
    public class ItemDTO
    {
        public char Symbol { get; set; }
        public string Color { get; set; }
        public string Name { get; set; }
        public string DisplayText { get; set; }
    }
}