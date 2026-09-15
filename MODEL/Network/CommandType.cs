namespace projektRPG.MODEL.Network
{
    /// <summary>
    /// Enumeration of distinct gameplay commands transmitted from clients to the server.
    /// Maps directly to user input triggers and key bindings.
    /// </summary>
    public enum CommandType
    {
        // Movement commands
        MoveUp,       // W
        MoveDown,     // S
        MoveLeft,     // A
        MoveRight,    // D

        // Tile interactions
        PickUpItem,   // E

        // Inventory management
        DropItem,     // Q
        EquipItem,    // R
        FreeHandItem, // F

        // Combat
        Attack        // Enter
    }
}