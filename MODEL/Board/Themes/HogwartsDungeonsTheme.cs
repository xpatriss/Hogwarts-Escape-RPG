using projektRPG.MODEL.Board.Strategy;
using projektRPG.MODEL.Items;
using projektRPG.MODEL.Items.Currency;
using projektRPG.MODEL.Items.Decorator;
using projektRPG.MODEL.Items.Things.HogwartsDungeons;
using projektRPG.MODEL.Items.Weapons.HogwartsDungeons;
using projektRPG.MODEL.Players.Enemies;
using projektRPG.MODEL.Players.Events;

namespace projektRPG.MODEL.Board.Themes
{
    /// <summary>
    /// Dark castle dungeons theme — Dementors, Death Eaters, Slytherin students.
    /// Uses Decorator to wrap themed items with stat modifiers (Healthy, Lucky, Strong).
    /// Ensures at least MinPerType of each enemy species before randomizing spawns.
    /// </summary>
    public class HogwartsDungeonsTheme : ITheme
    {
        private static readonly Random Rand = Random.Shared;

        private readonly SpeciesGroup weakEnemies = new SpeciesGroup("Dementors");
        private readonly SpeciesGroup braveEnemies = new SpeciesGroup("DeathEaters");
        private readonly SpeciesGroup neutralEnemies = new SpeciesGroup("SlytherinStudents");

        private int neutralCreated = 0;
        private int weakCreated = 0;
        private int braveCreated = 0;
        private const int MinPerType = 2;

        public string ThemeName => "Hogwart's Dungeons";
        public string WelcomeMessage() =>
            "The smell of dampness hangs in the air; terrifying darkness is all around you...";

        public string ArtifactName => "The Elder Wand";
        public string ItemNames => "Butterbeer, Felix Felicis, Time-Turner";
        public string WeaponNames => "Basilisk Fang, Troll's Club";
        public string EnemyNames => "Slytherin Students, Death Eaters, and Dementors";
        public IDungeonStrategy BuildStrategy => new DungeonsStrategy();

        public Item Artifact() => new ElderWand();

        public Item CreateItem()
        {
            int choice = Rand.Next(3);

            return choice switch
            {
                0 => new HealthyModifier(new ButterBeer()),
                1 => new LuckyModifier(new FelixFelicis()),
                2 => new TimeTurner(),
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
            int choice = Rand.Next(2);

            return choice switch
            {
                0 => new StrongModifier(new BasiliskFang()),
                1 => new TrollsClub(),
                _ => new Coin()
            };
        }

        public Enemy CreateEnemy(INoisePublisher noisePublisher, IGameEventRecorder eventRecorder)
        {
            if (weakCreated < MinPerType)
            {
                weakCreated++;
                return new WeakEnemy(weakEnemies, noisePublisher, eventRecorder, "Dementor");
            }
            if (braveCreated < MinPerType)
            {
                braveCreated++;
                return new BraveEnemy(braveEnemies, noisePublisher, eventRecorder, "DeathEater");
            }
            if (neutralCreated < MinPerType)
            {
                neutralCreated++;
                return new NeutralEnemy(neutralEnemies, noisePublisher, eventRecorder, "SlytherinStudent");
            }

            int choice = Rand.Next(3);

            return choice switch
            {
                0 => new WeakEnemy(weakEnemies, noisePublisher, eventRecorder, "Dementor"),
                1 => new BraveEnemy(braveEnemies, noisePublisher, eventRecorder, "DeathEater"),
                2 => new NeutralEnemy(neutralEnemies, noisePublisher, eventRecorder, "SlytherinStudent"),
                _ => new WeakEnemy(weakEnemies, noisePublisher, eventRecorder, "Dementor")
            };
        }
    }
}
