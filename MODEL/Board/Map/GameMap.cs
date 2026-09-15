using projektRPG.Infrastructure;

namespace projektRPG.MODEL.Board.Map
{
    /// <summary>
    /// The playable dungeon grid. Includes a one-tile wall border around the inner map.
    /// Coordinates (0,0) is the top-left corner of the bordered grid.
    /// </summary>
    public class GameMap
    {
        private Field[,] grid = new Field[Consts.MapHeight + 2, Consts.MapWidth + 2];

        public GameMap(Field[,] completedGrid)
        {
            grid = completedGrid;
        }

        /// <summary>Returns the field at (x, y), or a virtual Wall for out-of-bounds coordinates.</summary>
        public Field GetField(int x, int y)
        {
            if (x < 0 || x >= Consts.MapWidth + 2 || y < 0 || y >= Consts.MapHeight + 2)
            {
                return new Wall();
            }
            return grid[y, x];
        }

        /// <summary>True if the tile exists, is walkable, and is not a wall.</summary>
        public bool CanMoveTo(int x, int y)
        {
            if (y < 0 || x < 0 || y >= Consts.MapHeight + 2 || x >= Consts.MapWidth + 2)
                return false;

            if (!GetField(x, y).IsAvailable)
                return false;

            return true;
        }
    }
}
