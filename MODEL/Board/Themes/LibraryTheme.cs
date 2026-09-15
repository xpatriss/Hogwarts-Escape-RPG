using projektRPG.MODEL.Board.Strategy;
using projektRPG.MODEL.Items;
using projektRPG.MODEL.Items.Currency;
using projektRPG.MODEL.Items.Decorator;
using projektRPG.MODEL.Items.Things.Library;
using projektRPG.MODEL.Items.Weapons.Library;
using projektRPG.MODEL.Players.Enemies;
using projektRPG.MODEL.Players.Events;

namespace projektRPG.MODEL.Board.Themes
{
    /// <summary>
    /// Hogwarts Library theme — quieter map with many items and fewer enemies.
    /// Artifact: Invisibility Cloak. Enemies: Slytherin students, goblins, trolls.
    /// </summary>
    public class LibraryTheme : ITheme
    {
        private static readonly Random Rand = Random.Shared;

        private readonly SpeciesGroup weakEnemies = new SpeciesGroup("Slytherin Students");
        private readonly SpeciesGroup braveEnemies = new SpeciesGroup("Goblins");
        private readonly SpeciesGroup neutralEnemies = new SpeciesGroup("Trolls");

        private int neutralCreated = 0;
        private int weakCreated = 0;
        private int braveCreated = 0;
        private const int MinPerType = 2;

        public string ThemeName => "Hogwart's Library";
        public IDungeonStrategy BuildStrategy => new LibraryStrategy();

        public string WelcomeMessage() =>
            "You hear a piercing silence; the scent of old books is everywhere...";

        public string ArtifactName => "Invisibility Cloak";
        public string ItemNames => "Book of Potions, Chocolate Frog Cards";
        public string WeaponNames => "Book of Spells, Dagger, Quill";
        public string EnemyNames => "Trolls, Slytherin Students, and Goblins";

        public Item Artifact() => new CloakOfInvisibility();

        public Item CreateItem()
        {
            int choice = Rand.Next(2);

            return choice switch
            {
                0 => new LuckyModifier(new ChocolateFrogCard()),
                1 => new BookOfPotions(),
                _ => new Coin()
            };
        }

        public Item CreateCurrency()
        {
            int choice = Rand.Next(2);
            return choice switch
            {
                0 => new Coin(),
                1 => new Gold(),
                _ => new Coin()
            };
        }

        public Item CreateWeapon()
        {
            int choice = Rand.Next(3);

            return choice switch
            {
                0 => new BookOfSpells(),
                1 => new StrongModifier(new Dagger()),
                2 => new Quill(),
                _ => new Coin()
            };
        }

        public Enemy CreateEnemy(INoisePublisher noisePublisher, IGameEventRecorder eventRecorder)
        {
            if (weakCreated < MinPerType)
            {
                weakCreated++;
                return new WeakEnemy(weakEnemies, noisePublisher, eventRecorder, "Slytherin Student");
            }
            if (braveCreated < MinPerType)
            {
                braveCreated++;
                return new BraveEnemy(braveEnemies, noisePublisher, eventRecorder, "Goblin");
            }
            if (neutralCreated < MinPerType)
            {
                neutralCreated++;
                return new NeutralEnemy(neutralEnemies, noisePublisher, eventRecorder, "Troll");
            }

            int choice = Rand.Next(3);

            return choice switch
            {
                0 => new WeakEnemy(weakEnemies, noisePublisher, eventRecorder, "Slytherin Student"),
                1 => new BraveEnemy(braveEnemies, noisePublisher, eventRecorder, "Goblin"),
                2 => new NeutralEnemy(neutralEnemies, noisePublisher, eventRecorder, "Troll"),
                _ => new WeakEnemy(weakEnemies, noisePublisher, eventRecorder, "Slytherin Student")
            };
        }
    }
}
