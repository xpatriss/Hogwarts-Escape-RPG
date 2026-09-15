using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using projektRPG.CONTROLLER;
using projektRPG.MODEL.Board.Map;
using projektRPG.MODEL.Players;

namespace projektRPG.MODEL.GameStates
{
    /// <summary>
    /// Common interface for client-side UI states.
    /// Pattern: State — defines state-specific key action mappings and view identifiers.
    /// </summary>
    public interface IGameState
    {
        /// <summary>Returns the unique identifier of the state used for HUD rendering.</summary>
        string GetName();

        /// <summary>Returns key bindings and actions allowed in this state.</summary>
        Dictionary<ConsoleKey, (string Description, Action<ClientController> Action)> GetAvailableActions();
    }
}