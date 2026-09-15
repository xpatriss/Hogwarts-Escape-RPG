namespace projektRPG.MODEL.Board.Map
{
    /// <summary>
    /// Walkable floor tile. Players and enemies can stand here; items can be dropped on it.
    /// </summary>
    public class Empty : Field
    {
        public override bool IsAvailable => true;
        public override char Symbol => ' ';
    }
}
