namespace projektRPG.MODEL.Board.Map
{
    /// <summary>
    /// Impassable wall tile forming the outer border and dungeon layout.
    /// </summary>
    public class Wall : Field
    {
        public override bool IsAvailable => false;
        public override char Symbol => '█';

        public override string Info() => "";
    }
}
