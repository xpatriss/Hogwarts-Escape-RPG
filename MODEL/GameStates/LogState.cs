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
    /// Event log inspection state locking movement until the screen is dismissed.
    /// Pattern: State (ConcreteState).
    /// </summary>
    public class LogState : IGameState
    {
        Dictionary<ConsoleKey, (string, Action<ClientController>)> actions = new Dictionary<ConsoleKey, (string, Action<ClientController>)>();
        public string GetName() => "LOGS";

        public LogState()
        {
            actions.Add(GameCommands.Exit, ("Exit Inventory", c => Actions.ExitLogs(c)));
        }

        public Dictionary<ConsoleKey, (string, Action<ClientController>)> GetAvailableActions()
        {
            return actions;
        }
    }
}