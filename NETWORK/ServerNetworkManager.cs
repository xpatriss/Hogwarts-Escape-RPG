using projektRPG.MODEL;
using projektRPG.MODEL.Network;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace projektRPG.NETWORK
{
    /// <summary>
    /// Tracks active connection details and session identification for a connected player.
    /// </summary>
    public class ClientSession
    {
        public int PlayerId { get; set; }
        public TcpClient Connection { get; set; }
    }

    /// <summary>
    /// Authoritative server networking manager.
    /// Accepts incoming TCP client connections, reads serialized commands into a thread-safe queue 
    /// (Producer-Consumer pattern), and handles broadcast state fan-out.
    /// </summary>
    public class ServerNetworkManager
    {
        private TcpListener _listener;
        private volatile bool _isRunning;
        private List<ClientSession> _sessions = new List<ClientSession>();
        private int _nextPlayerId = 1;
        private readonly object _sessionsLock = new object();
        private GameModel _globalModel;

        /// <summary>
        /// Thread-safe queue of incoming client action commands waiting to be processed by ServerController.
        /// Pattern: Producer-Consumer (action buffer).
        /// </summary>
        public ConcurrentQueue<ActionDTO> ActionQueue { get; private set; } = new ConcurrentQueue<ActionDTO>();

        public ServerNetworkManager(int port, GameModel globalModel)
        {
            _listener = new TcpListener(IPAddress.Any, port);
            _globalModel = globalModel;
        }

        /// <summary>Starts listening for incoming client connection requests on a background worker.</summary>
        public void Start()
        {
            _isRunning = true;
            _listener.Start();
            Console.WriteLine("[SERVER] Awaiting player connections...");

            Task.Run(() => AcceptClientsAsync());
        }

        /// <summary>Closes listener socket and cleanly disconnects all active client sessions.</summary>
        public void Stop()
        {
            Console.WriteLine("[SERVER] Initiating server shutdown sequence...");

            _isRunning = false;
            _listener.Stop();

            // Disconnect all connected clients
            lock (_sessionsLock)
            {
                foreach (var session in _sessions)
                {
                    try
                    {
                        session.Connection.Close();
                    }
                    catch { /* Suppress individual socket close errors */ }
                }
                _sessions.Clear();
            }

            Console.WriteLine("[SERVER] Server terminated successfully.");
        }

        /// <summary>Asynchronously accepts incoming connections while respecting capacity limits.</summary>
        private async Task AcceptClientsAsync()
        {
            while (_isRunning)
            {
                try
                {
                    TcpClient client = await _listener.AcceptTcpClientAsync();

                    ClientSession newSession;
                    lock (_sessionsLock)
                    {
                        if (_sessions.Count >= 9)
                        {
                            client.Close();
                            continue;
                        }

                        newSession = new ClientSession
                        {
                            PlayerId = _nextPlayerId++,
                            Connection = client
                        };
                        _sessions.Add(newSession);
                        Console.WriteLine($"[SERVER] Player {newSession.PlayerId} joined the game!");
                    }

                    // Dispatch client handling loop on a separate worker thread
                    _ = Task.Run(() => HandleClientAsync(newSession));
                }
                catch (Exception) { /* Suppress exceptions on listener teardown */ }
            }
        }

        /// <summary>
        /// Reads player handshake information and queues incoming command packets.
        /// Cleans up state when the client disconnects.
        /// </summary>
        private async Task HandleClientAsync(ClientSession session)
        {
            NetworkStream stream = session.Connection.GetStream();
            byte[] buffer = new byte[4096];

            try
            {
                // Handshake phase: initial string payload represents chosen nickname
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead == 0) return;

                string playerName = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();

                // Spawn player in global authoritative model
                _globalModel.AddNewPlayer(session.PlayerId, playerName);
                Console.WriteLine($"[SERVER] Player {session.PlayerId} registered as {playerName}!");

                // Message processing loop
                while (_isRunning && session.Connection.Connected)
                {
                    bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break; // Client disconnected

                    string jsonString = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    // Deserialize incoming action payload
                    ActionDTO action = JsonSerializer.Deserialize<ActionDTO>(jsonString);
                    if (action != null)
                    {
                        action.PlayerId = session.PlayerId;
                        ActionQueue.Enqueue(action);
                    }
                }
            }
            catch (Exception) { }
            finally
            {
                lock (_sessionsLock)
                {
                    _sessions.Remove(session);
                }
                session.Connection.Close();

                _globalModel.RemovePlayer(session.PlayerId);
                Console.WriteLine($"[SERVER] Player {session.PlayerId} left the game.");
            }
        }

        /// <summary>
        /// Serializes and dispatches tailored state snapshots asynchronously across all active client streams.
        /// </summary>
        public async Task SendPersonalizedStatesAsync(Dictionary<int, GameStateDTO> personalizedStates)
        {
            List<Task> sendTasks = new List<Task>();

            lock (_sessionsLock)
            {
                foreach (var session in _sessions)
                {
                    if (session.Connection.Connected && personalizedStates.ContainsKey(session.PlayerId))
                    {
                        var stateForThisPlayer = personalizedStates[session.PlayerId];
                        string jsonString = JsonSerializer.Serialize(stateForThisPlayer);
                        byte[] data = Encoding.UTF8.GetBytes(jsonString);

                        NetworkStream stream = session.Connection.GetStream();
                        sendTasks.Add(stream.WriteAsync(data, 0, data.Length));
                    }
                }
            }

            await Task.WhenAll(sendTasks);
        }
    }
}