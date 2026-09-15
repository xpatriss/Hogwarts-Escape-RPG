using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using projektRPG.CONTROLLER;
using projektRPG.CONTROLLER.Commands;
using projektRPG.MODEL.Board.Map;
using projektRPG.MODEL.Players;

namespace projektRPG.MODEL.GameStates
{
    /// <summary>
    /// Default exploration state when moving around the dungeon grid.
    /// Pattern: State (ConcreteState).
    /// </summary>
    public class BoardState : IGameState
    {
        Dictionary<ConsoleKey, (string, Action<ClientController>)> actions = new Dictionary<ConsoleKey, (string, Action<ClientController>)>();
        public string GetName() => "BOARD";

        public BoardState()
        {
            actions.Add(GameCommands.MoveUp, ("Move Up", c => Actions.Move(c, 0, -1)));
            actions.Add(GameCommands.MoveDown, ("Move Down", c => Actions.Move(c, 0, 1)));
            actions.Add(GameCommands.MoveLeft, ("Move Left", c => Actions.Move(c, -1, 0)));
            actions.Add(GameCommands.MoveRight, ("Move Right", c => Actions.Move(c, 1, 0)));

            actions.Add(GameCommands.Inventory, ("Open Inventory", c => Actions.OpenInventory(c)));
            actions.Add(GameCommands.Logs, ("Open Logs", c => Actions.OpenLogs(c)));
        }

        public Dictionary<ConsoleKey, (string, Action<ClientController>)> GetAvailableActions()
        {
            return actions;
        }
    }
}