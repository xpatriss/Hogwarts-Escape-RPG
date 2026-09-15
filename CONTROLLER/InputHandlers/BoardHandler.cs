using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using projektRPG.MODEL.Board.Map;
using projektRPG.MODEL.Players;
using projektRPG.VIEW;

namespace projektRPG.CONTROLLER.InputHandlers
{
    /// <summary>
    /// Concrete handler that maps keystrokes against actions permitted by the current UI state.
    /// Pattern: Chain of Responsibility (ConcreteHandler) combined with State Pattern (evaluates CurrentState actions).
    /// </summary>
    public class BoardHandler : InputHandler
    {
        private ClientController controller;

        public BoardHandler(ClientController controller) => this.controller = controller;

        /// <summary>
        /// Executes the action associated with the pressed key in the active game state,
        /// or passes execution to the next handler if no mapping exists.
        /// </summary>
        public override bool Handle(ConsoleKeyInfo key, ClientController controller)
        {
            var actions = controller.LocalModel.CurrentState.GetAvailableActions();

            if (actions.ContainsKey(key.Key))
            {
                actions[key.Key].Action.Invoke(controller);
                return true;
            }

            return base.Handle(key, controller);
        }
    }
}