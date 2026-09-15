using System;

namespace projektRPG.CONTROLLER.Commands
{
    /// <summary>
    /// Central key binding mapping for game navigation, inventory control, and actions.
    /// Provides symbolic constants decoupled from concrete key codes.
    /// </summary>
    public static class GameCommands
    {
        // Movement controls
        public const ConsoleKey MoveUp = ConsoleKey.W;
        public const ConsoleKey MoveDown = ConsoleKey.S;
        public const ConsoleKey MoveLeft = ConsoleKey.A;
        public const ConsoleKey MoveRight = ConsoleKey.D;

        // Interaction and menus
        public const ConsoleKey PickUp = ConsoleKey.E;
        public const ConsoleKey Inventory = ConsoleKey.I;

        // Inventory management
        public const ConsoleKey Drop = ConsoleKey.Q;
        public const ConsoleKey Equip = ConsoleKey.R;
        public const ConsoleKey FreeHand = ConsoleKey.F;
        public const ConsoleKey Exit = ConsoleKey.Escape;

        // Navigation controls
        public const ConsoleKey Up = ConsoleKey.UpArrow;
        public const ConsoleKey Down = ConsoleKey.DownArrow;

        // Combat and logs
        public const ConsoleKey Attack = ConsoleKey.Enter;
        public const ConsoleKey Logs = ConsoleKey.J;
    }
}