using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using projektRPG.Infrastructure.LogStorage;
using projektRPG.MODEL.Board.Map;
using projektRPG.MODEL.Players;
using projektRPG.VIEW;

namespace projektRPG.CONTROLLER.InputHandlers
{
    /// <summary>
    /// Terminal fallback handler in the input responsibility chain.
    /// Handles application termination on Escape or notifies the user of unrecognized input.
    /// Pattern: Chain of Responsibility (ConcreteHandler / Tail).
    /// </summary>
    public class DefaultHandler : InputHandler
    {
        private ClientController controller;

        public DefaultHandler(ClientController controller) => this.controller = controller;

        /// <summary>
        /// Intercepts Escape to quit the game or sets an "UNKNOWN KEY" notification in the local model.
        /// </summary>
        public override bool Handle(ConsoleKeyInfo key, ClientController controller)
        {
            if (key.Key == ConsoleKey.Escape)
            {
                Logger.Instance.Log("Exiting game");
                controller.Stop();
                return false;
            }

            controller.LocalModel.MessageToDisplay = "UNKNOWN KEY";
            Logger.Instance.Log("Unknown key pressed");
            return true;
        }
    }
}