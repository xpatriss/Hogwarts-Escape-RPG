using projektRPG.CONTROLLER;
using projektRPG.Infrastructure.Config;
using projektRPG.Infrastructure.LogStorage;
using projektRPG.MODEL;
using projektRPG.VIEW;
using System;
using System.Text;

namespace projektRPG
{
    /// <summary>
    /// Application entry point for Hogwarts Escape — a multiplayer console RPG.
    /// Supports two launch modes: game server (host) and game client (player).
    /// The server must be running before any client can connect.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Configures the console, loads settings, and starts either the server or the client.
        /// Can be launched interactively or via command-line arguments (-server / -client).
        /// </summary>
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.CursorVisible = false;
            Console.Clear();

            // Singleton pattern: ConfigurationManager provides a single shared config instance for the whole app.
            ConfigurationManager.Instance.LoadConfig();
            var data = ConfigurationManager.Instance.Data;

            // --- Command-line launch (non-interactive) ---
            if (args.Length > 0)
            {
                if (args[0] == "-server")
                {
                    int port = args.Length > 1 ? int.Parse(args[1]) : 5555;
                    RunServer(port, data);
                    return;
                }
                else if (args[0] == "-client")
                {
                    string ip = "127.0.0.1";
                    int port = 5555;

                    if (args.Length > 1)
                    {
                        var parts = args[1].Split(':');
                        ip = parts[0];
                        if (parts.Length > 1) port = int.Parse(parts[1]);
                    }

                    Console.Clear();
                    Console.WriteLine("=== JOINING THE GAME ===");
                    Console.Write("Enter your name: ");

                    string nick = Console.ReadLine();

                    if (!string.IsNullOrWhiteSpace(nick))
                    {
                        data.PlayerName = nick.Trim();
                    }
                    else
                    {
                        data.PlayerName = "Wizard";
                    }

                    RunClient(ip, port, data);
                    return;
                }
            }

            // --- Interactive launch: user picks server or client mode ---
            Console.WriteLine("=== HOGWARTS ESCAPE - MULTIPLAYER ===");
            Console.WriteLine("Select launch mode:");
            Console.WriteLine("[S] - Launch as SERVER (Host)");
            Console.WriteLine("[K] - Launch as CLIENT (Player)");
            Console.WriteLine();
            Console.WriteLine("Note: Start the SERVER first in a separate window.");
            Console.WriteLine("      The CLIENT cannot connect unless a server is already running.");

            ConsoleKey key;
            do
            {
                key = Console.ReadKey(true).Key;
            } while (key != ConsoleKey.S && key != ConsoleKey.K);

            Console.Clear();

            if (key == ConsoleKey.S)
            {
                RunServer(5555, data);
            }
            else if (key == ConsoleKey.K)
            {
                Console.Clear();
                PrintClientPrerequisite();
                Console.WriteLine("=== JOINING THE GAME ===");
                Console.Write("Enter your name: ");

                string nick = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(nick))
                {
                    data.PlayerName = nick.Trim();
                }
                else
                {
                    data.PlayerName = "Wizard";
                }
                RunClient("127.0.0.1", 5555, data);
            }
        }

        /// <summary>
        /// Reminds the player that a running server is required before launching a client.
        /// </summary>
        static void PrintClientPrerequisite()
        {
            Console.WriteLine("Make sure the game SERVER is already running, then press Enter to continue.");
            Console.WriteLine("(Starting a client without a server will result in a connection error.)");
            Console.WriteLine();
            Console.Write("Press Enter when the server is ready...");
            Console.ReadLine();
            Console.Clear();
        }

        /// <summary>
        /// Starts the game server: owns the authoritative game state and processes all player actions.
        /// Pattern: MVC — ServerController acts as the controller, GameModel holds game logic.
        /// </summary>
        static void RunServer(int port, ConfigData data)
        {
            Console.Title = "Hogwarts Escape - SERVER";
            Console.WriteLine($"[SERVER] Initializing on port {port}...");

            // Singleton pattern: Logger writes server events to a shared log file.
            Logger.Instance.SetStorage(new FileLogStorage(data.LogFilePath, "SERVER"));

            var serverController = new ServerController(port, data);
            serverController.Run();
        }

        /// <summary>
        /// Starts a game client: renders the UI and forwards keyboard input to the server.
        /// Pattern: MVC — ClientController (controller), LocalStateModel (local state), GameView (view).
        /// The client never modifies the global game state; it only displays DTOs received from the server.
        /// </summary>
        static void RunClient(string ip, int port, ConfigData data)
        {
            Console.Title = "Hogwarts Escape - CLIENT";
            Console.WriteLine($"[CLIENT] Connecting to server {ip}:{port} as {data.PlayerName}...");

            // Each client gets its own log file named after the player.
            Logger.Instance.SetStorage(new FileLogStorage("clients_logs", data.PlayerName));

            var localModel = new LocalStateModel();
            var view = new GameView();

            try
            {
                var clientController = new ClientController(ip, port, data, localModel, view);
                clientController.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine("[CLIENT] Connection failed.");
                Console.WriteLine("Start the SERVER first, then launch the client again.");
                Console.WriteLine($"Details: {ex.Message}");
            }
        }
    }
}
