using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace projektRPG.MODEL.Network
{
    /// <summary>
    /// Two-dimensional matrix of tile DTOs transmitted to synchronize the visible board layout.
    /// Pattern: DTO.
    /// </summary>
    public class GameMapDTO
    {
        public FieldDTO[][] Grid { get; set; }
    }
}