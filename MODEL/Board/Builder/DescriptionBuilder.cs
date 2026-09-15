using System.Text;
using projektRPG.CONTROLLER.Commands;
using projektRPG.MODEL.Board.Themes;

namespace projektRPG.MODEL.Board.Builder
{
    /// <summary>
    /// Builds the intro plot text shown to the player before the game starts.
    /// Pattern: Builder — each Director step appends the corresponding lore / control hints.
    /// Content is driven by the active ITheme (items, weapons, enemies, artifact names).
    /// </summary>
    public class DescriptionBuilder : IBuilder
    {
        private StringBuilder sb = new StringBuilder();
        private ITheme theme;

        public DescriptionBuilder(ITheme theme)
        {
            this.theme = theme;
            Reset();
        }

        public DescriptionBuilder()
        {
            Reset();
        }

        public void Reset()
        {
            sb.Clear();
            sb.AppendLine($"You play as a hero in the theme: {theme.ThemeName}.");
            sb.AppendLine(theme.WelcomeMessage());
        }

        public void AddItems(int n)
        {
            if (n > 0)
            {
                sb.AppendLine($"\nYou will find items such as: {theme.ItemNames}.");
                sb.AppendLine($"Legend says the following artifact is hidden here: {theme.ArtifactName}.");
                sb.AppendLine($"Press {GameCommands.PickUp} to pick them up into your inventory.");
                sb.AppendLine($"Press {GameCommands.Drop} to drop an item from your inventory.");
            }
        }

        public void AddRoom(int x, int y) { }

        public void AddWeapons(int n)
        {
            if (n > 0)
            {
                sb.AppendLine($"\nWeapons available for combat: {theme.WeaponNames}.");
                sb.AppendLine($"Press {GameCommands.PickUp} to pick them up into your inventory.");
                sb.AppendLine($"Press {GameCommands.Drop} to drop an item from your inventory.");
                sb.AppendLine($"To use a weapon, move it from inventory to your hand with {GameCommands.Equip}.");
                sb.AppendLine($"Press {GameCommands.FreeHand} to unequip the weapon from your hand.");
            }
        }

        public void AddMoneyAndGold(int n)
        {
            if (n > 0)
            {
                sb.AppendLine("\nAlong the way you may find gold pieces and Galleons left behind by Gringotts goblins.");
                sb.AppendLine($"Press {GameCommands.PickUp} to collect them.");
            }
        }

        public void AddEnemies(int n)
        {
            if (n > 0)
            {
                sb.AppendLine($"\nEnemies waiting for you: {theme.EnemyNames}.");
                sb.AppendLine($"To attack an enemy, equip a weapon in your hand and press {GameCommands.Attack}.");
            }
        }

        public void BuildFull() { }
        public void BuildEmpty() { }
        public void AddCorridors(int n) { }
        public void AddChambers(int n) { }

        /// <summary>Centers each line of the intro text within the console window.</summary>
        private string CenterText(string text)
        {
            int windowWidth = Console.WindowWidth;
            if (text.Length >= windowWidth) return text;

            int leftPadding = (windowWidth - text.Length) / 2;
            return new string(' ', leftPadding) + text;
        }

        /// <summary>Returns the finished intro text displayed before gameplay begins.</summary>
        public string GetResult()
        {
            sb.AppendLine("\nPRESS ENTER TO START THE GAME...");

            return CenterText(sb.ToString());
        }
    }
}
