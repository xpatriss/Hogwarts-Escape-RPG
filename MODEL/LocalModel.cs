using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using projektRPG.MODEL.GameStates;

namespace projektRPG.MODEL
{
    /// <summary>
    /// Encapsulates local client-side UI state, including active menu state,
    /// focused selection indices, and transient HUD notification banners.
    /// Pattern: MVC — acts as the local Model managed by ClientController.
    /// Pattern: State — maintains the active <see cref="IGameState"/> context.
    /// </summary>
    public class LocalStateModel
    {
        /// <summary>Index of the currently highlighted list element (item, inventory slot, or attack action).</summary>
        public int SelectedItemIndex { get; set; } = -1;

        /// <summary>
        /// Current UI state determining active key actions and view rendering mode.
        /// Pattern: State context reference.
        /// </summary>
        public IGameState CurrentState { get; set; } = new BoardState();

        /// <summary>One-off feedback message displayed in the UI notification area.</summary>
        public string MessageToDisplay { get; set; } = "";
    }
}