using System;
using System.Threading;
using projektRPG.CONTROLLER.InputHandlers;
using projektRPG.Infrastructure.Config;
using projektRPG.Infrastructure.LogStorage;
using projektRPG.MODEL;
using projektRPG.MODEL.GameStates;
using projektRPG.MODEL.Items.Weapons.Visitor;
using projektRPG.MODEL.Network;
using projektRPG.NETWORK;
using projektRPG.VIEW;

namespace projektRPG.CONTROLLER
{
    /// <summary>
    /// Coordinates client-side gameplay loop, handles incoming network state DTOs, 
    /// processes user keyboard input via a responsibility chain, and requests view redraws.
    /// Pattern: MVC — acts as the client Controller; connects LocalStateModel and GameView.
    /// </summary>
    public class ClientController
    {
        private readonly ClientNetworkManager _network;

        /// <summary>Latest authoritative game state snapshot received from the server.</summary>
        public GameStateDTO? LatestState { get; private set; }

        /// <summary>Local UI state model maintaining selection indices and active HUD view state.</summary>
        public LocalStateModel LocalModel { get; private set; }

        /// <summary>Console renderer for the HUD, board, inventory, and combat panels.</summary>
        public GameView View { get; private set; }

        /// <summary>
        /// Root of the input processing chain.
        /// Pattern: Chain of Responsibility.
        /// </summary>
        private readonly InputHandler _inputChain;

        private volatile bool _needsRedraw = false;
        private bool _isRunning = true;

        /// <summary>
        /// Available attack strategies selectable by the player during combat.
        /// Pattern: Visitor (IAttackVisitor).
        /// </summary>
        public IAttackVisitor[] AvailableAttacks { get; } = {
            new BasicAttack(),
            new SneakyAttack(),
            new MagicAttack()
        };

        /// <summary>
        /// Initializes network connection, sets up the input handling chain, and configures view dependencies.
        /// </summary>
        public ClientController(string ip, int port, ConfigData config, LocalStateModel local, GameView view)
        {
            LocalModel = local;
            View = view;

            // Network setup
            _network = new ClientNetworkManager();
            _network.OnGameStateReceived += HandleNewGameState;
            _network.OnDisconnected += () => _isRunning = false;
            _network.Connect(ip, port, config.PlayerName);

            // Chain of Responsibility: State-specific handler followed by fallback default handler
            var stateHandler = new BoardHandler(this);
            var defHandler = new DefaultHandler(this);
            stateHandler.SetNext(defHandler);
            _inputChain = stateHandler;
        }

        /// <summary>
        /// Callback executed upon receiving a state update from the server.
        /// Triggers HUD redraw, transitions UI states based on current tile context, and writes server-dispatched logs.
        /// </summary>
        private void HandleNewGameState(GameStateDTO newState)
        {
            LatestState = newState;
            _needsRedraw = true;

            var currentField = LatestState.Map.Grid[LatestState.MyState.X][LatestState.MyState.Y];

            // Context-driven state transitions (State pattern)
            if (currentField.HasEnemy)
            {
                if (LocalModel.CurrentState.GetName() != "ATTACK" && LocalModel.CurrentState.GetName() != "INVENTORY")
                    ChangeState(new AttackState());
            }
            else if (currentField.ItemsCount > 0)
            {
                if (LocalModel.CurrentState.GetName() != "FIELD" && LocalModel.CurrentState.GetName() != "INVENTORY")
                    ChangeState(new FieldState());
            }
            else
            {
                if (LocalModel.CurrentState.GetName() == "FIELD" || LocalModel.CurrentState.GetName() == "ATTACK")
                    ChangeState(new BoardState());
            }

            // Flush client-directed combat and environmental logs dispatched by the server
            if (LatestState.MyState != null && LatestState.MyState.NewLogs.Count > 0)
            {
                foreach (var log in LatestState.MyState.NewLogs)
                {
                    Logger.Instance.Log(log);
                }
            }
        }

        /// <summary>
        /// Runs the client execution loop: renders scenes, consumes keyboard events, and tracks player vitals.
        /// </summary>
        public void Run()
        {
            // Initial server handshake and map reception
            Console.Clear();
            Console.WriteLine("Connecting to server and retrieving world map...");
            while (LatestState == null && _isRunning)
            {
                Thread.Sleep(50);
            }

            if (!_isRunning) return;

            // Introduction sequence
            Console.Clear();
            View.DrawIntro();
            while (Console.ReadKey(true).Key != ConsoleKey.Enter) { }

            if (!string.IsNullOrEmpty(LatestState.Description))
            {
                Console.Clear();
                View.DrawPlot(LatestState.Description);
                while (Console.ReadKey(true).Key != ConsoleKey.Enter) { }
                Console.Clear();
            }

            // Initial frame render
            View.Render(LatestState, LocalModel, AvailableAttacks);

            // Main client game loop
            while (_isRunning)
            {
                if (_needsRedraw)
                {
                    View.Render(LatestState, LocalModel, AvailableAttacks);
                    _needsRedraw = false;
                }

                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo key = Console.ReadKey(true);
                    View.ClearMessage();

                    // Delegate input to the responsibility chain
                    _inputChain.Handle(key, this);

                    _needsRedraw = true;
                }

                // Check fatal health state
                if (LatestState.MyState.Health <= 0)
                {
                    Logger.Instance.Log("You died!");
                    Console.Clear();
                    View.DrawGameOver();
                    while (Console.ReadKey(true).Key != ConsoleKey.Enter) { }
                    _isRunning = false;
                }

                Thread.Sleep(30);
            }
        }

        /// <summary>
        /// Transitions the local interface to a new state and resets selection indices.
        /// Pattern: State.
        /// </summary>
        public void ChangeState(IGameState newState)
        {
            LocalModel.CurrentState = newState;
            LocalModel.SelectedItemIndex = -1;
            _needsRedraw = true;
        }

        /// <summary>
        /// Serializes an intended action into an ActionDTO and forwards it to the authoritative server.
        /// </summary>
        public void SendActionToServer(CommandType cmd, int targetIndex = 0)
        {
            var action = new ActionDTO { Command = cmd, TargetIndex = targetIndex };
            _network.SendAction(action);
        }

        /// <summary>Terminates the local client update loop.</summary>
        public void Stop() => _isRunning = false;
    }
}