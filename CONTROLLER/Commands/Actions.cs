using System;
using projektRPG.CONTROLLER;
using projektRPG.Infrastructure.LogStorage;
using projektRPG.MODEL.GameStates;
using projektRPG.MODEL.Network;

namespace projektRPG.CONTROLLER.Commands
{
    /// <summary>
    /// Encapsulates executable player actions invoked via keyboard handlers.
    /// Pattern: Command — decouples user input triggers from client-side UI state transitions
    /// and network payload dispatching to the authoritative server.
    /// </summary>
    public static class Actions
    {
        // --- Local UI Actions (State Pattern Transitions) ---

        /// <summary>Transitions client UI to inventory inspection mode.</summary>
        public static Action<ClientController> OpenInventory => (c => { c.ChangeState(new InventoryState()); c.LocalModel.SelectedItemIndex = 0; });

        /// <summary>Returns client UI to standard board exploration mode.</summary>
        public static Action<ClientController> ExitInventory => (c => { c.ChangeState(new BoardState()); });

        /// <summary>Transitions client UI to the full-screen event log reader.</summary>
        public static Action<ClientController> OpenLogs => (c => PrintLogs(c));

        /// <summary>Exits event log reader and restores board state.</summary>
        public static Action<ClientController> ExitLogs => (c => c.ChangeState(new BoardState()));

        /// <summary>Displays application shutdown notice.</summary>
        public static Action<ClientController> ExitGame => (c => { c.LocalModel.MessageToDisplay = "Exiting game..."; });

        /// <summary>
        /// Cycles through items lying on the current ground tile.
        /// </summary>
        /// <param name="c">The active client controller.</param>
        /// <param name="d">Direction offset (-1 for previous, 1 for next).</param>
        public static void SelectItemField(ClientController c, int d)
        {
            int myX = c.LatestState.MyState.X;
            int myY = c.LatestState.MyState.Y;

            var fieldDTO = c.LatestState.Map.Grid[myX][myY];
            int selected = c.LocalModel.SelectedItemIndex;
            int maxCount = fieldDTO.Items.Count;

            if (maxCount == 0)
            {
                c.LocalModel.SelectedItemIndex = 0;
                return;
            }

            if (selected == -1) selected = 0;
            selected += d;

            if (selected >= maxCount) selected = 0;
            else if (selected < 0) selected = maxCount - 1;

            c.LocalModel.SelectedItemIndex = selected;
        }

        /// <summary>
        /// Cycles through items stored inside the player's personal backpack.
        /// </summary>
        /// <param name="c">The active client controller.</param>
        /// <param name="d">Direction offset (-1 for previous, 1 for next).</param>
        public static void SelectItemInventory(ClientController c, int d)
        {
            int selected = c.LocalModel.SelectedItemIndex;
            int maxCount = c.LatestState.MyState.InventoryDisplay.Count;

            if (maxCount == 0)
            {
                c.LocalModel.SelectedItemIndex = 0;
                return;
            }

            if (selected == -1) selected = 0;
            selected += d;

            if (selected >= maxCount) selected = 0;
            else if (selected < 0) selected = maxCount - 1;

            c.LocalModel.SelectedItemIndex = selected;
        }

        /// <summary>
        /// Cycles through available combat visitor attack strategies.
        /// </summary>
        /// <param name="c">The active client controller.</param>
        /// <param name="d">Direction offset (-1 for previous, 1 for next).</param>
        public static void SelectAttack(ClientController c, int d)
        {
            int selected = c.LocalModel.SelectedItemIndex;
            int maxCount = c.AvailableAttacks.Length;

            if (maxCount == 0)
            {
                c.LocalModel.SelectedItemIndex = 0;
                return;
            }

            if (selected == -1) selected = 0;
            selected += d;

            if (selected >= maxCount) selected = 0;
            else if (selected < 0) selected = maxCount - 1;

            c.LocalModel.SelectedItemIndex = selected;
        }

        /// <summary>
        /// Switches to the log inspection state and renders historical logs.
        /// </summary>
        public static void PrintLogs(ClientController c)
        {
            c.ChangeState(new LogState());
            var allLogs = Logger.Instance.GetAllLogs();
            c.View.DrawFullLog(allLogs);
        }

        // --- Network Actions (Dispatched to Authoritative Server) ---

        /// <summary>
        /// Translates coordinate deltas to a directional command and forwards it to the server.
        /// </summary>
        public static void Move(ClientController c, int dx, int dy)
        {
            CommandType cmd = CommandType.MoveUp;
            if (dx == 0 && dy == -1) cmd = CommandType.MoveUp;
            else if (dx == 0 && dy == 1) cmd = CommandType.MoveDown;
            else if (dx == -1 && dy == 0) cmd = CommandType.MoveLeft;
            else if (dx == 1 && dy == 0) cmd = CommandType.MoveRight;

            c.SendActionToServer(cmd);
        }

        /// <summary>
        /// Dispatches a pickup command for the selected item on the current tile.
        /// If multiple items are present and unselected, prompts the player first.
        /// </summary>
        public static void Pickup(ClientController c)
        {
            int myX = c.LatestState.MyState.X;
            int myY = c.LatestState.MyState.Y;
            var fieldDTO = c.LatestState.Map.Grid[myX][myY];
            int selected = c.LocalModel.SelectedItemIndex;

            if (fieldDTO.Items.Count > 1 && selected == -1)
            {
                c.LocalModel.MessageToDisplay = "USE ARROW KEYS TO SELECT AN ITEM AND PRESS THE KEY AGAIN";
                c.LocalModel.SelectedItemIndex = 0;
                return;
            }
            if (selected == -1)
            {
                c.LocalModel.SelectedItemIndex = 0;
                selected = 0;
            }

            c.SendActionToServer(CommandType.PickUpItem, selected);
            c.LocalModel.SelectedItemIndex = -1;
        }

        /// <summary>
        /// Dispatches a drop command for the selected inventory item.
        /// </summary>
        public static void ThrowItem(ClientController c)
        {
            int selectedItem = c.LocalModel.SelectedItemIndex;
            var inventory = c.LatestState.MyState.InventoryDisplay;

            if (inventory.Count > 1 && selectedItem == -1)
            {
                c.LocalModel.MessageToDisplay = "USE ARROW KEYS TO SELECT AN ITEM AND PRESS THE KEY AGAIN";
                c.LocalModel.SelectedItemIndex = 0;
                return;
            }

            if (selectedItem == -1)
            {
                c.LocalModel.SelectedItemIndex = 0;
                selectedItem = 0;
            }

            c.SendActionToServer(CommandType.DropItem, selectedItem);
            c.LocalModel.SelectedItemIndex = 0;
        }

        /// <summary>
        /// Dispatches an equip command for the selected inventory item.
        /// </summary>
        public static void EquipItem(ClientController c)
        {
            int selectedItem = c.LocalModel.SelectedItemIndex;
            var inventory = c.LatestState.MyState.InventoryDisplay;

            if (inventory.Count > 1 && selectedItem == -1)
            {
                c.LocalModel.MessageToDisplay = "USE ARROW KEYS TO SELECT AN ITEM AND PRESS THE KEY AGAIN";
                c.LocalModel.SelectedItemIndex = 0;
                return;
            }

            if (selectedItem == -1)
            {
                c.LocalModel.SelectedItemIndex = 0;
                selectedItem = 0;
            }

            c.SendActionToServer(CommandType.EquipItem, selectedItem);
            c.LocalModel.SelectedItemIndex = 0;
        }

        /// <summary>
        /// Requests the server to unequip weapons currently held in hands.
        /// </summary>
        public static void FreeHandItem(ClientController c)
        {
            c.SendActionToServer(CommandType.FreeHandItem);
            c.LocalModel.SelectedItemIndex = -1;
        }

        /// <summary>
        /// Initiates a combat turn against the enemy on the current tile using the selected attack style.
        /// </summary>
        public static void FightTurn(ClientController c)
        {
            int myX = c.LatestState.MyState.X;
            int myY = c.LatestState.MyState.Y;
            var fieldDTO = c.LatestState.Map.Grid[myX][myY];
            int selectedAttack = c.LocalModel.SelectedItemIndex;

            if (fieldDTO.HasEnemy || fieldDTO.Enemy != null)
            {
                if (selectedAttack == -1)
                {
                    c.ChangeState(new AttackState());
                    c.LocalModel.SelectedItemIndex = 0;
                    c.LocalModel.MessageToDisplay = "SELECT AN ATTACK AND PRESS ENTER";
                    return;
                }

                c.SendActionToServer(CommandType.Attack, selectedAttack);
            }
            else
            {
                c.LocalModel.MessageToDisplay = "NO ENEMY PRESENT HERE";
            }
        }
    }
}