using Pastel;
using projektRPG.CONTROLLER.Commands;
using projektRPG.Infrastructure;
using projektRPG.Infrastructure.LogStorage;
using projektRPG.MODEL;
using projektRPG.MODEL.GameStates;
using projektRPG.MODEL.Items.Weapons.Visitor;
using projektRPG.MODEL.Network;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;

namespace projektRPG.VIEW
{
    /// <summary>
    /// Renders the client-side console interface, including the world map, HUD panels,
    /// player attributes, backpack items, combat menus, and full-screen event logs.
    /// Pattern: MVC — acts as the View layer; renders data from GameStateDTO and LocalStateModel.
    /// </summary>
    public class GameView
    {
        private (int x, int y) BoardPosition;
        private (int x, int y) AttributesPosition;
        private (int x, int y) InventoryPosition;
        private (int x, int y) FieldInfoPosition;
        private (int x, int y) HandsPosition;

        private (int x, int y) MessagePosition;
        private (int x, int y) InstructionsPosition;
        private (int x, int y) AttacksPosition;
        private (int x, int y) LogsPosition;

        public GameView()
        {
            BoardPosition = (Console.WindowWidth / 2 - Consts.MapWidth / 2, 4);
            InventoryPosition = (Console.WindowWidth - 47, 1);
            HandsPosition = (InventoryPosition.x, InventoryPosition.y + Consts.EquipmentLimit + 6);
            AttacksPosition = (HandsPosition.x, HandsPosition.y + 9);
            FieldInfoPosition = (AttacksPosition.x, AttacksPosition.y + 7);

            AttributesPosition = (3, 1);
            InstructionsPosition = (AttributesPosition.x, AttributesPosition.y + 15);

            LogsPosition = (InstructionsPosition.x, InstructionsPosition.y + 18);

            MessagePosition = (BoardPosition.x, BoardPosition.y + Consts.MapHeight + 3);
        }

        /// <summary>
        /// Performs a complete redraw of all HUD frames, map tiles, attributes, and messages.
        /// </summary>
        public void Render(GameStateDTO state, LocalStateModel local, IAttackVisitor[] attacks)
        {
            DrawMap(state);
            DrawAttributes(state);
            DrawRecentLogs();

            DrawInventory(state, local.SelectedItemIndex, local.CurrentState);
            DrawHands(state);

            DrawFieldInfo(state, local.SelectedItemIndex, local.CurrentState);

            DrawAttacks(attacks, local.SelectedItemIndex, local.CurrentState);
            DrawInstructions(local.CurrentState);

            // Display pending status message if set
            if (!string.IsNullOrEmpty(local.MessageToDisplay))
            {
                PrintMessage(local.MessageToDisplay);
                local.MessageToDisplay = "";
            }
        }

        /// <summary>
        /// Partially updates HUD panels affected by local UI state changes without repainting the entire screen.
        /// </summary>
        public void RenderChanges(GameStateDTO state, LocalStateModel local, IAttackVisitor[] availableAttacks)
        {
            DrawInventory(state, local.SelectedItemIndex, local.CurrentState);
            DrawHands(state);

            DrawFieldInfo(state, local.SelectedItemIndex, local.CurrentState);

            DrawAttacks(availableAttacks, local.SelectedItemIndex, local.CurrentState);
            DrawInstructions(local.CurrentState);
        }

        /// <summary>
        /// Draws a styled border frame with an embedded title header.
        /// </summary>
        public void DrawFrame(int width, int height, int startX, int startY, string title)
        {
            char tl = '┌', tr = '┐', bl = '└', br = '┘', v = '│', h = '─';

            int visualTitleLen = 0;
            var enumerator = StringInfo.GetTextElementEnumerator(title);
            while (enumerator.MoveNext()) visualTitleLen++;

            int totalTextLen = visualTitleLen + 2;

            int titleStart = (width - totalTextLen) / 2 - 1;
            if (titleStart < 1) titleStart = 1;

            Console.SetCursorPosition(startX, startY);

            string leftH = new string(h, titleStart);

            int rightHCount = width - titleStart - totalTextLen - 2;
            if (rightHCount < 0) rightHCount = 0;
            string rightH = new string(h, rightHCount);

            Console.Write(tl + leftH + " " + title + " " + rightH + tr);

            for (int i = 1; i < height - 1; i++)
            {
                Console.SetCursorPosition(startX, startY + i);
                Console.Write(v);
                Console.SetCursorPosition(startX + width - 1, startY + i);
                Console.Write(v);
            }

            Console.SetCursorPosition(startX, startY + height - 1);
            Console.Write(bl + new string(h, width - 2) + br);
        }

        /// <summary>Renders the animated title intro screen.</summary>
        public void DrawIntro()
        {
            string gameTitle = @"
 ██╗  ██╗ ██████╗  ██████╗ ██╗    ██╗ █████╗ ██████╗ ████████╗███████╗
 ██║  ██║██╔═══██╗██╔════╝ ██║    ██║██╔══██╗██╔══██╗╚══██╔══╝██╔════╝
 ███████║██║   ██║██║  ███╗██║ █╗ ██║███████║██████╔╝   ██║   ███████╗
 ██╔══██║██║   ██║██║   ██║██║███╗██║██╔══██║██╔══██╗   ██║   ╚════██║
 ██║  ██║╚██████╔╝╚██████╔╝╚███╔███╔╝██║  ██║██║  ██║   ██║   ███████║
 ╚═╝  ╚═╝ ╚═════╝  ╚═════╝  ╚══╝╚══╝ ╚═╝  ╚═╝╚═╝  ╚═╝   ╚═╝   ╚══════╝

 ███████╗███████╗ ██████╗ █████╗ ██████╗ ███████╗
 ██╔════╝██╔════╝██╔════╝██╔══██╗██╔══██╗██╔════╝
█████╗  ███████╗██║     ███████║██████╔╝█████╗
██╔══╝  ╚════██║██║     ██╔══██║██╔═══╝ ██╔══╝
 ███████╗███████║╚██████╗██║  ██║██║     ███████╗
 ╚══════╝╚══════╝ ╚═════╝╚═╝  ╚═╝╚═╝     ╚══════╝";

            string fullContent = gameTitle.Pastel("#740001") + "\n\n\nPRESS ENTER TO CONTINUE...";

            string[] lines = fullContent.Split('\n');
            int windowWidth = Console.WindowWidth;
            int windowHeight = Console.WindowHeight;

            int startY = (windowHeight - lines.Length) / 2;
            if (startY < 0) startY = 0;

            foreach (var line in lines)
            {
                string cleanLine = System.Text.RegularExpressions.Regex.Replace(line, @"\e\[[0-9;]*m", "");

                int startX = (windowWidth - cleanLine.Length) / 2;
                if (startX < 0) startX = 0;

                if (startY < windowHeight)
                {
                    Console.SetCursorPosition(startX, startY++);
                    foreach (char c in line)
                    {
                        Console.Write(c);
                    }
                    Thread.Sleep(100);
                    Console.WriteLine();
                }
            }
        }

        /// <summary>Displays the introductory narrative storyline generated by DescriptionBuilder.</summary>
        public void DrawPlot(string description)
        {
            int windowWidth = Console.WindowWidth;
            int windowHeight = Console.WindowHeight;
            int startY = 0;
            int startX = 0;

            foreach (var line in description.Split('\n'))
            {
                startX = (windowWidth - line.Length) / 2;
                if (startX < 0) startX = 0;
                if (startX >= windowWidth || startY >= windowHeight)
                    continue;
                Console.SetCursorPosition(startX, startY++);
                Console.Write(line);
            }
        }

        /// <summary>Draws the dungeon grid, including walls, items, enemies, and player avatars.</summary>
        public void DrawMap(GameStateDTO state)
        {
            int currentY = BoardPosition.y;
            int width = state.Map.Grid.Length;
            int height = state.Map.Grid[0].Length;

            Console.SetCursorPosition(BoardPosition.x, 2);
            Console.Write("    ℍ 𝕆 𝔾 𝕎 𝔸 ℝ 𝕋 𝕊   𝔼 𝕊 ℂ 𝔸 ℙ 𝔼".Pastel("CF931D"));

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < height; i++)
            {
                Console.SetCursorPosition(BoardPosition.x, currentY);
                for (int j = 0; j < width; j++)
                {
                    // Render local player
                    if (i == state.MyState.Y && j == state.MyState.X)
                    {
                        sb.Append(Consts.PlayerSymbol);
                    }
                    // Render remote networked players
                    else if (state.OtherPlayers.Any(p => p.X == j && p.Y == i))
                    {
                        var otherPlayer = state.OtherPlayers.First(p => p.X == j && p.Y == i);
                        sb.Append(otherPlayer.Symbol.ToString());
                    }
                    else
                    {
                        var field = state.Map.Grid[j][i];

                        if (field.HasEnemy && field.Enemy != null)
                        {
                            sb.Append(field.Enemy.Symbol.ToString().Pastel(field.Enemy.Color));
                        }
                        else if (field.ItemsCount > 0 && field.Items.Count > 0)
                        {
                            var item = field.Items[field.Items.Count - 1];
                            sb.Append(item.Symbol.ToString().Pastel(item.Color));
                        }
                        else
                        {
                            sb.Append(field.Symbol.ToString().Pastel(ConsoleColor.DarkGray));
                        }
                    }
                }
                Console.Write(sb.ToString());
                sb.Clear();
                currentY++;
            }
        }

        /// <summary>Renders available key commands based on active UI state.</summary>
        public void DrawInstructions(IGameState state)
        {
            int x = InstructionsPosition.x + 1;
            int y = InstructionsPosition.y + 1;

            int frameWidth = 36;
            int frameHeight = 12;

            DrawFrame(frameWidth, frameHeight, x - 2, y - 1, "𝐂𝐎𝐌𝐌𝐀𝐍𝐃𝐒");

            var actions = state.GetAvailableActions();

            Console.SetCursorPosition(x, y++);

            foreach (var action in actions)
            {
                Console.SetCursorPosition(x, y++);
                Console.Write($"[{action.Key}] - {action.Value.Description}".PadRight(frameWidth - 3));
            }

            for (int i = y; i < InstructionsPosition.y + frameHeight - 1; i++)
            {
                Console.SetCursorPosition(x, i);
                Console.Write("".PadRight(frameWidth - 3));
            }
        }

        /// <summary>Renders selectable attack visitor styles during combat encounters.</summary>
        public void DrawAttacks(IAttackVisitor[] availableAttacks, int selectedItem, IGameState state)
        {
            int x = AttacksPosition.x + 1;
            int y = AttacksPosition.y + 1;

            int frameWidth = 48;
            int frameHeight = 6;

            DrawFrame(frameWidth, frameHeight, x - 2, y - 1, "ATTACK");

            int idxE = 0;

            foreach (var attack in availableAttacks)
            {
                Console.SetCursorPosition(x, ++y);
                Console.Write("".PadRight(frameWidth - 3));
                Console.SetCursorPosition(x, y);
                if (idxE == selectedItem && state.GetName() == "ATTACK")
                {
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                Console.Write(attack.Name.PadRight(frameWidth - 3));
                idxE++;
                Console.ResetColor();
            }
        }

        /// <summary>Renders backpack contents with active selection highlight.</summary>
        public void DrawInventory(GameStateDTO state, int selectedItem, IGameState localState)
        {
            int x = InventoryPosition.x + 1;
            int y = InventoryPosition.y + 1;

            int frameWidth = 48;
            int frameHeight = 15;

            DrawFrame(frameWidth, frameHeight, x - 2, y - 1, "𝐄𝐐𝐔𝐈𝐏𝐌𝐄𝐍𝐓");

            Console.SetCursorPosition(x, ++y);

            for (int i = 0; i < Consts.EquipmentLimit; i++)
            {
                Console.SetCursorPosition(x, y);
                Console.Write($"{i + 1}.".PadRight(frameWidth - 3));
                y++;
            }

            y = InventoryPosition.y + 2;
            x += 2;

            var inventoryDisplay = state.MyState.InventoryDisplay;
            int idxE = 0;

            foreach (string line in inventoryDisplay)
            {
                if (!string.IsNullOrEmpty(line))
                {
                    Console.SetCursorPosition(x, y);
                    Console.Write("".PadRight(frameWidth - 5));
                    Console.SetCursorPosition(x, y);
                    if (idxE == selectedItem && localState.GetName() == "INVENTORY")
                    {
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.ForegroundColor = ConsoleColor.Black;
                    }
                    Console.Write(line.PadRight(frameWidth - 5));
                    y++;
                    idxE++;
                    Console.ResetColor();
                }
            }
        }

        /// <summary>Renders equipped weapon slots for left and right hands.</summary>
        public void DrawHands(GameStateDTO state)
        {
            int x = HandsPosition.x + 1;
            int y = HandsPosition.y + 1;

            int frameWidth = 48;
            int frameHeight = 8;

            DrawFrame(frameWidth, frameHeight, x - 2, y - 1, "𝐇𝐀𝐍𝐃𝐒");

            y++;
            Console.SetCursorPosition(x, y);
            Console.Write($"LEFT HAND: ".PadRight(frameWidth - 3));
            Console.SetCursorPosition(x, ++y);

            Console.Write($" {state.MyState.LeftHandDisplay}".PadRight(frameWidth - 3));

            y++;
            Console.SetCursorPosition(x, ++y);
            Console.Write($"RIGHT HAND: ".PadRight(frameWidth - 3));
            Console.SetCursorPosition(x, ++y);

            Console.Write($" {state.MyState.RightHandDisplay}".PadRight(frameWidth - 3));
        }

        /// <summary>Renders player stats, RPG attributes, and collected currency totals.</summary>
        public void DrawAttributes(GameStateDTO state)
        {
            int y = AttributesPosition.y + 1;
            int x = AttributesPosition.x + 1;

            int frameWidth = 35;
            int frameHeight = 14;

            DrawFrame(frameWidth, frameHeight, x - 2, y - 1, "𝐏𝐋𝐀𝐘𝐄𝐑");

            var my = state.MyState;

            y++;
            Console.SetCursorPosition(x, y);
            Console.Write($"POSITION: ({my.X}, {my.Y})".PadRight(frameWidth - 3));

            y++;
            y++;
            Console.SetCursorPosition(x, y);
            Console.Write($"HEALTH:    {my.Health}".PadRight(frameWidth - 3));
            y++;
            Console.SetCursorPosition(x, y);
            Console.Write($"LUCK:      {my.Luck}".PadRight(frameWidth - 3));
            y++;
            Console.SetCursorPosition(x, y);
            Console.Write($"STRENGTH:  {my.Strength}".PadRight(frameWidth - 3));

            y++;
            Console.SetCursorPosition(x, y);
            Console.Write($"AGRESSION: {my.Agression}".PadRight(frameWidth - 3));
            y++;
            Console.SetCursorPosition(x, y);
            Console.Write($"DEXTERITY: {my.Dexterity}".PadRight(frameWidth - 3));
            y++;
            Console.SetCursorPosition(x, y);
            Console.Write($"WISDOM:    {my.Wisdom}".PadRight(frameWidth - 3));

            y++;
            y++;
            Console.SetCursorPosition(x, y);
            Console.Write($"COINS: {my.Coins}".PadRight(frameWidth - 3));
            y++;
            Console.SetCursorPosition(x, y);
            Console.Write($"GOLD:  {my.Gold}".PadRight(frameWidth - 3));
        }

        /// <summary>Renders details of entities or ground items on the current tile.</summary>
        public void DrawFieldInfo(GameStateDTO state, int selectedItem, IGameState localState)
        {
            int x = FieldInfoPosition.x + 1;
            int y = FieldInfoPosition.y + 1;

            var my = state.MyState;

            int frameWidth = 48;
            int frameHeight = 6;

            DrawFrame(frameWidth, frameHeight, x - 2, y - 1, "𝐅𝐈𝐄𝐋𝐃");

            int idx = 0;
            y++;

            string eq = state.Map.Grid[state.MyState.X][state.MyState.Y].InfoText;

            if (state.OtherPlayers.Any(p => p.X == my.X && p.Y == my.Y))
            {
                var otherPlayer = state.OtherPlayers.First(p => p.X == my.X && p.Y == my.Y);
                Console.SetCursorPosition(x, y);
                Console.Write("".PadRight(frameWidth - 3));
                Console.SetCursorPosition(x, y);

                Console.Write($"Player: {otherPlayer.Name}  HP: {otherPlayer.HP}".PadRight(frameWidth - 3));
                return;
            }

            if (!string.IsNullOrEmpty(eq))
            {
                string[] lines = eq.Split('\n');
                foreach (string line in lines)
                {
                    if (!string.IsNullOrEmpty(line))
                    {
                        Console.SetCursorPosition(x, y);
                        Console.Write("".PadRight(frameWidth - 3));
                        Console.SetCursorPosition(x, y);

                        if (idx == selectedItem && localState.GetName() == "FIELD")
                        {
                            Console.BackgroundColor = ConsoleColor.White;
                            Console.ForegroundColor = ConsoleColor.Black;
                        }
                        Console.Write(line.PadRight(frameWidth - 3));
                        y++;
                        idx++;
                        Console.ResetColor();
                    }
                }
            }

            for (int i = y; i < FieldInfoPosition.y + frameHeight - 1; i++)
            {
                Console.SetCursorPosition(x, i);
                Console.Write("".PadRight(frameWidth - 3));
            }
        }

        /// <summary>Renders the defeat screen splash.</summary>
        public void GameOver()
        {
            string title = @"
 ██╗  ██╗ ██████╗  ██████╗ ██╗    ██╗ █████╗ ██████╗ ████████╗███████╗
 ██║  ██║██╔═══██╗██╔════╝ ██║    ██║██╔══██╗██╔══██╗╚══██╔══╝██╔════╝
 ███████║██║   ██║██║  ███╗██║ █╗ ██║███████║██████╔╝   ██║   ███████╗
 ██╔══██║██║   ██║██║   ██║██║███╗██║██╔══██║██╔══██╗   ██║   ╚════██║
 ██║  ██║╚██████╔╝╚██████╔╝╚███╔███╔╝██║  ██║██║  ██║   ██║   ███████║
 ╚═╝  ╚═╝ ╚═════╝  ╚═════╝  ╚══╝╚══╝ ╚═╝  ╚═╝╚═╝  ╚═╝   ╚═╝   ╚══════╝

 ███████╗███████╗ ██████╗ █████╗ ██████╗ ███████╗
 ██╔════╝██╔════╝██╔════╝██╔══██╗██╔══██╗██╔════╝
█████╗  ███████╗██║     ███████║██████╔╝█████╗
██╔══╝  ╚════██║██║     ██╔══██║██╔═══╝ ██╔══╝
 ███████╗███████║╚██████╗██║  ██║██║     ███████╗
 ╚══════╝╚══════╝ ╚═════╝╚═╝  ╚═╝╚═╝     ╚══════╝";

            string fullContent = title.Pastel("#740001") + "\n\n\nPRESS ENTER TO CONTINUE...";

            string[] lines = fullContent.Split('\n');
            int windowWidth = Console.WindowWidth;
            int windowHeight = Console.WindowHeight;

            int startY = (windowHeight - lines.Length) / 2;
            if (startY < 0) startY = 0;

            foreach (var line in lines)
            {
                string cleanLine = System.Text.RegularExpressions.Regex.Replace(line, @"\e\[[0-9;]*m", "");

                int startX = (windowWidth - cleanLine.Length) / 2;
                if (startX < 0) startX = 0;

                if (startY < windowHeight)
                {
                    Console.SetCursorPosition(startX, startY++);
                    foreach (char c in line)
                    {
                        Console.Write(c);
                    }
                    Thread.Sleep(100);
                    Console.WriteLine();
                }
            }
        }

        /// <summary>Displays fatal death prompt overlay.</summary>
        public void DrawGameOver()
        {
            Console.Clear();

            string title = "YOU DIED";
            string subtitle = "GAME OVER";

            int w = Math.Max(20, Console.WindowWidth);
            int h = Math.Max(10, Console.WindowHeight);

            string t = title;
            string s = subtitle;
            string prompt = "PRESS ENTER TO EXIT";

            int startY = h / 2 - 2;

            int tx = Math.Max(0, (w - t.Length) / 2);
            int sx = Math.Max(0, (w - s.Length) / 2);
            int px = Math.Max(0, (w - prompt.Length) / 2);

            Console.SetCursorPosition(tx, startY);
            Console.Write(t.Pastel("#A30018"));
            Console.SetCursorPosition(sx, startY + 1);
            Console.Write(s.Pastel("#740001"));
            Console.SetCursorPosition(px, startY + 3);
            Console.Write(prompt.Pastel(ConsoleColor.White));

            Console.SetCursorPosition(0, Console.WindowHeight - 1);
        }

        /// <summary>Prints transient prompt or warning messages below the board area.</summary>
        public void PrintMessage(string mess)
        {
            int returnX = Console.CursorLeft;
            int returnY = Console.CursorTop;

            string[] words = mess.Split(' ');
            List<string> lines = new List<string>();
            string currentLine = "";

            int x = MessagePosition.x;
            int y = MessagePosition.y;

            foreach (string word in words)
            {
                if ((currentLine + word).Length > Consts.MapWidth)
                {
                    lines.Add(currentLine.Trim());
                    currentLine = word + " ";
                }
                else
                {
                    currentLine += word + " ";
                }
            }

            if (!string.IsNullOrWhiteSpace(currentLine))
            {
                lines.Add(currentLine.Trim());
            }

            for (int i = 0; i < lines.Count; i++)
            {
                Console.SetCursorPosition(x, y + i);
                string paddedLine = lines[i].PadRight(Consts.MapWidth);
                Console.Write(paddedLine.Pastel(ConsoleColor.Red));
            }

            Console.SetCursorPosition(returnX, returnY);
        }

        /// <summary>Clears previously displayed message strings.</summary>
        public void ClearMessage()
        {
            int x = MessagePosition.x;
            int y = MessagePosition.y;

            Console.SetCursorPosition(x, y);
            for (int i = y; i < y + 3; i++)
            {
                Console.SetCursorPosition(x, i);
                Console.Write("".PadRight(Consts.MapWidth));
            }

            Console.SetCursorPosition(BoardPosition.x, BoardPosition.y + Consts.MapHeight + 3);
        }

        /// <summary>Renders recent log history inside the dedicated HUD panel.</summary>
        public void DrawRecentLogs()
        {
            var recentLogs = Logger.Instance.GetRecentLogs(4);
            int x = LogsPosition.x;
            int y = LogsPosition.y;

            int frameWidth = 95;
            int frameHeight = 6;

            DrawFrame(frameWidth, frameHeight, x - 2, y - 1, "LOGS");

            foreach (var log in recentLogs)
            {
                Console.SetCursorPosition(x, y++);
                Console.Write(log.PadRight(90));
            }
        }

        /// <summary>Renders the scrollable full-screen event log inspector.</summary>
        public void DrawFullLog(List<string> logs)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("=== FULL EVENT LOG ===");
            Console.ResetColor();
            Console.WriteLine();

            var displayLogs = logs.TakeLast(Console.WindowHeight - 5);

            foreach (var log in displayLogs)
            {
                Console.WriteLine(log);
            }

            Console.WriteLine($"\n[Press {GameCommands.Exit}] to return to the game...");
            while (Console.ReadKey(true).Key != ConsoleKey.Escape) { }
            Console.Clear();
        }
    }
}