using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using projektRPG.CONTROLLER;
using projektRPG.CONTROLLER.Commands;
using projektRPG.MODEL.Board.Map;
using projektRPG.MODEL.Players;

namespace projektRPG.MODEL.GameStates
{
    /// <summary>
    /// Backpack inspection state allowing the player to equip, drop, or clear hand weapons.
    /// Pattern: State (ConcreteState).
    /// </summary>
    public class InventoryState : IGameState
    {
        public string GetName() => "INVENTORY";

        Dictionary<ConsoleKey, (string, Action<ClientController>)> actions = new Dictionary<ConsoleKey, (string, Action<ClientController>)>();

        public InventoryState()
        {
            actions.Add(GameCommands.Drop, ("Drop Item", c => Actions.ThrowItem(c)));
            actions.Add(GameCommands.Equip, ("Equip Item", c => Actions.EquipItem(c)));
            actions.Add(GameCommands.FreeHand, ("Free Hand", c => Actions.FreeHandItem(c)));

            actions.Add(GameCommands.Up, ("Select Previous", c => Actions.SelectItemInventory(c, -1)));
            actions.Add(GameCommands.Down, ("Select Next", c => Actions.SelectItemInventory(c, 1)));

            actions.Add(GameCommands.Exit, ("Exit Inventory", c => Actions.ExitInventory(c)));
            actions.Add(GameCommands.Logs, ("Open Logs", c => Actions.OpenLogs(c)));
        }

        public Dictionary<ConsoleKey, (string, Action<ClientController>)> GetAvailableActions()
        {
            return actions;
        }
    }
}