using System;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using projektRPG.MODEL.Network;

namespace projektRPG.NETWORK
{
    /// <summary>
    /// Manages the client-side TCP connection to the game server.
    /// Handles asynchronous state stream listening and dispatches outbound serialized action packets.
    /// </summary>
    public class ClientNetworkManager
    {
        private TcpClient _client;
        private NetworkStream _stream;
        private volatile bool _isConnected;

        /// <summary>Fired when a new deserialized GameStateDTO snapshot arrives from the server.</summary>
        public event Action<GameStateDTO> OnGameStateReceived;

        /// <summary>Fired when the network socket disconnects or closes unexpectedly.</summary>
        public event Action OnDisconnected;

        /// <summary>
        /// Establishes a TCP connection with the server, sends initial player handshake, 
        /// and spins up the background receive loop.
        /// </summary>
        /// <param name="ip">Server IP address.</param>
        /// <param name="port">Server listening port.</param>
        /// <param name="playerName">Chosen player nickname sent during handshake.</param>
        public void Connect(string ip, int port, string playerName)
        {
            try
            {
                _client = new TcpClient(ip, port);
                _stream = _client.GetStream();
                _isConnected = true;

                // Transmit player nickname immediately upon establishing connection
                byte[] nameBytes = Encoding.UTF8.GetBytes(playerName);
                _stream.Write(nameBytes, 0, nameBytes.Length);

                Task.Run(() => ListenForServerUpdatesAsync());
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to connect to server: {ex.Message}");
            }
        }

        /// <summary>
        /// Serializes an intended action into JSON and transmits it over the network stream.
        /// </summary>
        public void SendAction(ActionDTO action)
        {
            if (!_isConnected || _stream == null) return;

            try
            {
                string jsonString = JsonSerializer.Serialize(action);
                byte[] data = Encoding.UTF8.GetBytes(jsonString);
                _stream.Write(data, 0, data.Length);
            }
            catch
            {
                Disconnect();
            }
        }

        /// <summary>
        /// Asynchronously listens for incoming state update payloads from the server.
        /// </summary>
        private async Task ListenForServerUpdatesAsync()
        {
            byte[] buffer = new byte[131072];

            try
            {
                while (_isConnected)
                {
                    int bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead == 0)
                    {
                        // Server closed connection gracefully
                        break;
                    }

                    string jsonString = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    var gameState = JsonSerializer.Deserialize<GameStateDTO>(jsonString);
                    if (gameState != null)
                    {
                        // Signal state change to trigger UI rerendering in ClientController
                        OnGameStateReceived?.Invoke(gameState);
                    }
                }
            }
            catch (Exception)
            {
                // Suppress socket read exceptions during intentional teardown
            }
            finally
            {
                Disconnect();
            }
        }

        /// <summary>Closes network stream, terminates the TCP socket, and triggers disconnection events.</summary>
        public void Disconnect()
        {
            if (!_isConnected) return;

            _isConnected = false;
            _stream?.Close();
            _client?.Close();

            OnDisconnected?.Invoke();
        }
    }
}