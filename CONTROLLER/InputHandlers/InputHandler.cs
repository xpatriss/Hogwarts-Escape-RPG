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
    /// Base handler in the user input processing pipeline.
    /// Pattern: Chain of Responsibility — passes unhandled keyboard inputs to the next handler in the sequence.
    /// </summary>
    public abstract class InputHandler
    {
        protected InputHandler? next;

        /// <summary>Sets the next successor handler in the responsibility chain.</summary>
        public void SetNext(InputHandler next) => this.next = next;

        /// <summary>
        /// Attempts to process a keystroke or delegates it down the handler chain.
        /// </summary>
        /// <param name="key">The intercepted console key info.</param>
        /// <param name="controller">Active client controller context.</param>
        /// <returns>True if the key was processed or forwarded; otherwise false.</returns>
        public virtual bool Handle(ConsoleKeyInfo key, ClientController controller)
        {
            if (next != null) return next.Handle(key, controller);
            return true;
        }
    }
}