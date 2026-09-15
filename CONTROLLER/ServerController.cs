using projektRPG.Infrastructure;
using projektRPG.Infrastructure.Config;
using projektRPG.Infrastructure.LogStorage;
using projektRPG.MODEL;
using projektRPG.MODEL.Board.Map;
using projektRPG.MODEL.GameStates;
using projektRPG.MODEL.Items;
using projektRPG.MODEL.Items.Weapons.Visitor;
using projektRPG.MODEL.Network;
using projektRPG.MODEL.Players;
using projektRPG.MODEL.Players.Enemies;
using projektRPG.NETWORK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace projektRPG.CONTROLLER
{
    /// <summary>
    /// Authoritative server controller that manages the central game loop.
    /// Validates client inputs, updates enemy AI, drives combat calculations, 
    /// triggers noise propagation via the Observer pattern, and broadcasts state snapshots.
    /// Pattern: MVC — acts as the authoritative Controller manipulating GameModel.
    /// </summary>
    public class ServerController
    {
        /// <summary>The authoritative world and game state.</summary>
        public GameModel GlobalModel { get; private set; }

        private readonly ServerNetworkManager _network;

        /// <summary>
        /// Combat attack strategies evaluated against weapon damage profiles.
        /// Pattern: Visitor (IAttackVisitor).
        /// </summary>
        public IAttackVisitor[] AvailableAttacks { get; } = {
            new BasicAttack(),
            new SneakyAttack(),
            new MagicAttack()
        };

        /// <summary>
        /// Initializes the game server with the specified configuration and theme.
        /// </summary>
        /// <param name="port">TCP port for incoming client connections.</param>
        /// <param name="data">Loaded runtime settings containing dungeon theme and log paths.</param>
        public ServerController(int port, ConfigData data)
        {
            GlobalModel = new GameModel(data.DungeonTheme);
            _network = new ServerNetworkManager(port, GlobalModel);
        }

        /// <summary>
        /// Starts network listening and runs the primary authoritative tick loop.
        /// Consumes action queues, ticks AI on an interval, and serializes state broadcasts.
        /// </summary>
        public void Run()
        {
            _network.Start();
            bool isRunning = true;

            var lastEnemyMove = DateTime.UtcNow;
            var enemyInterval = TimeSpan.FromSeconds(5);

            Console.WriteLine("[SERVER] Main game loop started.");
            Console.WriteLine("[SERVER] Press Ctrl+C to safely terminate the server.");

            // Intercept cancellation signal for graceful server teardown
            Console.CancelKeyPress += (sender, e) =>
            {
                e.Cancel = true;
                isRunning = false;
            };

            try
            {
                // Authoritative server tick loop
                while (isRunning)
                {
                    bool stateChanged = false;

                    // Drain and process all actions queued by connected clients
                    while (_network.ActionQueue.TryDequeue(out ActionDTO action))
                    {
                        ProcessAction(action);
                        stateChanged = true;
                    }

                    // Periodic enemy AI tick
                    if (DateTime.UtcNow - lastEnemyMove >= enemyInterval)
                    {
                        GlobalModel.UpdateEnemies();
                        LogEnemyTurnAttacks();
                        FlushServerLogs();
                        lastEnemyMove = DateTime.UtcNow;
                        stateChanged = true;
                    }

                    // Broadcast snapshot updates to all clients when changes occur
                    if (stateChanged)
                    {
                        BroadcastState();
                    }

                    Thread.Sleep(30);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SERVER] Critical error encountered in game loop: {ex.Message}");
            }
            finally
            {
                _network.Stop();
                Console.WriteLine("[SERVER] Server process terminated cleanly.");
            }
        }

        /// <summary>
        /// Validates and executes client command payloads against server business rules.
        /// Dispatches noise alerts (Observer) and queues player-specific combat/loot notifications.
        /// </summary>
        private void ProcessAction(ActionDTO action)
        {
            var player = GlobalModel.GetPlayerById(action.PlayerId);
            if (player == null) return;

            var map = GlobalModel.Map;
            List<string> personalLogs = new List<string>();

            switch (action.Command)
            {
                case CommandType.MoveUp: HandleMove(player, map, 0, -1, personalLogs); break;
                case CommandType.MoveDown: HandleMove(player, map, 0, 1, personalLogs); break;
                case CommandType.MoveLeft: HandleMove(player, map, -1, 0, personalLogs); break;
                case CommandType.MoveRight: HandleMove(player, map, 1, 0, personalLogs); break;

                case CommandType.PickUpItem:
                    var field = map.GetField(player.Position.x, player.Position.y);
                    if (action.TargetIndex >= 0 && action.TargetIndex < field.Items.Count)
                    {
                        var item = field.Items[action.TargetIndex];
                        if (player.TakeItem(item, out int pick_noise))
                        {
                            field.RemoveItemAt(action.TargetIndex);
                            personalLogs.Add($"Picked up: {item.Name}");
                            Logger.Instance.Log($"Player {action.PlayerId} picked up {item.Name}");

                            // Observer pattern: notify listening enemies within noise radius
                            if (pick_noise > 0)
                            {
                                GlobalModel.Notify((player.Position.x, player.Position.y, pick_noise, map));
                            }
                        }
                        else
                        {
                            personalLogs.Add("YOUR BACKPACK IS FULL");
                        }
                    }
                    break;

                case CommandType.DropItem:
                    var itemToDrop = player.ThrowItem(action.TargetIndex, out int drop_noise);
                    if (itemToDrop != null)
                    {
                        map.GetField(player.Position.x, player.Position.y).AddItem(itemToDrop);
                        personalLogs.Add($"Dropped: {itemToDrop.Name}");

                        // Observer pattern: notify listening enemies within noise radius
                        if (drop_noise > 0)
                        {
                            GlobalModel.Notify((player.Position.x, player.Position.y, drop_noise, map));
                        }
                    }
                    break;

                case CommandType.EquipItem:
                    if (action.TargetIndex >= 0 && action.TargetIndex < player.Inventory.EquipmentList.Count)
                    {
                        var item = player.Inventory.EquipmentList[action.TargetIndex];
                        if (item.RequiredHands > 0 && item.Equip(player))
                        {
                            personalLogs.Add($"Equipped: {item.Name}");
                        }
                        else
                        {
                            personalLogs.Add("Cannot equip item (hands occupied or item is not equippable).");
                        }
                    }
                    break;

                case CommandType.FreeHandItem:
                    if (!player.FreeHand())
                    {
                        personalLogs.Add("Backpack is full; cannot unequip weapon.");
                    }
                    else
                    {
                        personalLogs.Add("Unequipped weapon.");
                    }
                    break;

                case CommandType.Attack:
                    var targetField = map.GetField(player.Position.x, player.Position.y);

                    if (targetField.HasEnemy() && action.TargetIndex >= 0 && action.TargetIndex < AvailableAttacks.Length)
                    {
                        var attack = this.AvailableAttacks[action.TargetIndex];
                        int hpBeforeEnemy = targetField.enemy.HP;
                        var enemy = targetField.enemy;

                        // Visitor pattern: resolves weapon-specific attack logic
                        player.PerformAttack(targetField.enemy, attack);

                        personalLogs.Add($"You attack {enemy.Name} (dealt {hpBeforeEnemy - enemy.HP} dmg)");

                        if (enemy.HP > 0)
                        {
                            int damageTaken = player.ReceiveAttack(enemy.Attack, attack);
                            AddEnemyAttackLogs(personalLogs, player, enemy, damageTaken, "counterattacks");
                        }
                        else
                        {
                            personalLogs.Add($"You defeated: {enemy.Name}!");
                            targetField.RemoveEnemy();
                        }
                    }
                    break;
            }

            player.PendingLogs.AddRange(personalLogs);
            FlushServerLogs();
        }

        /// <summary>Flushes accumulated server diagnostic logs to the primary logger.</summary>
        private void FlushServerLogs()
        {
            foreach (var message in GlobalModel.PendingServerLogs)
                Logger.Instance.Log(message);

            GlobalModel.ClearServerLogs();
        }

        /// <summary>Collects and appends logs resulting from enemy turns to affected players.</summary>
        private void LogEnemyTurnAttacks()
        {
            foreach (var enemy in GlobalModel.Enemies)
            {
                if (enemy.LastAttackedPlayer == null) continue;
                AddEnemyAttackLogs(
                    enemy.LastAttackedPlayer.PendingLogs,
                    enemy.LastAttackedPlayer,
                    enemy,
                    enemy.LastAttackDamage,
                    "attacks you");
            }
        }

        private static void AddEnemyAttackLogs(
            List<string> logs, Player player, Enemy enemy,
            int damageTaken, string attackVerb)
        {
            if (damageTaken > 0)
                logs.Add($"{enemy.Name} {attackVerb}! (took {damageTaken} dmg)");
            else
                logs.Add($"{enemy.Name} {attackVerb}, but your defense deflects the strike.");

            if (player.Health <= 0)
                logs.Add($"You were defeated by {enemy.Name}!");
        }

        /// <summary>Validates wall collisions and updates player grid position.</summary>
        private void HandleMove(Player p, GameMap map, int dx, int dy, List<string> logs)
        {
            int tx = p.Position.x + dx;
            int ty = p.Position.y + dy;

            if (map.CanMoveTo(tx, ty))
            {
                p.Walk(tx, ty);
            }
            else
            {
                logs.Add("Cannot walk through walls");
            }
        }

        /// <summary>
        /// Compiles the global map state and projects personalized DTO snapshots for each connected player.
        /// Dispatches data packets asynchronously across the network layer.
        /// </summary>
        private void BroadcastState()
        {
            // Build shared map snapshot DTO
            var mapDTO = new GameMapDTO();
            int width = Consts.MapWidth + 2;
            int height = Consts.MapHeight + 2;

            mapDTO.Grid = new FieldDTO[width][];
            for (int x = 0; x < width; x++)
            {
                mapDTO.Grid[x] = new FieldDTO[height];
                for (int y = 0; y < height; y++)
                {
                    var field = GlobalModel.Map.GetField(x, y);

                    mapDTO.Grid[x][y] = new FieldDTO
                    {
                        Symbol = field.Symbol,
                        HasEnemy = field.HasEnemy(),
                        ItemsCount = field.Items.Count,
                        InfoText = field.Info(),

                        Enemy = field.enemy != null ? new EnemyDTO
                        {
                            Symbol = field.enemy.Symbol,
                            Color = field.enemy.Color,
                            X = x,
                            Y = y,
                            Name = field.enemy.Name,
                            HP = field.enemy.HP,
                            Attack = field.enemy.Attack,
                            Armor = field.enemy.Armor
                        } : null,

                        Items = field.Items.Select(i => new ItemDTO
                        {
                            Symbol = i.Symbol,
                            Color = i.Color,
                            Name = i.Name,
                            DisplayText = i.ToString()
                        }).ToList()
                    };
                }
            }

            // Create personalized state payloads for each client session
            var personalizedStates = new Dictionary<int, GameStateDTO>();

            foreach (var player in GlobalModel.Players)
            {
                var state = new GameStateDTO();
                state.Map = mapDTO;
                state.Description = GlobalModel.description;

                // Restrict private stats and log queues to the owning client
                state.MyState = new PlayerPrivateDTO
                {
                    X = player.Position.x,
                    Y = player.Position.y,
                    Health = player.Health,
                    Strength = player.Strength,
                    Luck = player.Luck,
                    Dexterity = player.Dexterity,
                    Agression = player.Agression,
                    Wisdom = player.Wisdom,

                    Coins = player.Coins,
                    Gold = player.Gold,

                    InventoryDisplay = player.Inventory.EquipmentList.Select(i => i.ToString()).ToList(),

                    LeftHandDisplay = player.LeftHand != null ? player.LeftHand.ToString() : "free",
                    RightHandDisplay = player.RightHand != null ? player.RightHand.ToString() : "free",

                    NewLogs = new List<string>(player.PendingLogs)
                };

                // Expose public coordinates and vitals of peers
                state.OtherPlayers = GlobalModel.Players
                    .Where(p => p.Id != player.Id)
                    .Select(p => new PlayerPublicDTO
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Symbol = p.Symbol,
                        X = p.Position.x,
                        Y = p.Position.y,
                        HP = p.Health
                    }).ToList();

                personalizedStates.Add(player.Id, state);
                player.PendingLogs.Clear();
            }

            // Asynchronously dispatch personalized payloads
            _ = _network.SendPersonalizedStatesAsync(personalizedStates);
        }
    }
}