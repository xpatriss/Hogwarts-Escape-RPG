using projektRPG.MODEL.Board.Strategy;
using projektRPG.MODEL.Items;
using projektRPG.MODEL.Items.Currency;
using projektRPG.MODEL.Items.Decorator;
using projektRPG.MODEL.Items.Things.TriwizardMaze;
using projektRPG.MODEL.Items.Weapons.TriwizardMaze;
using projektRPG.MODEL.Players.Enemies;
using projektRPG.MODEL.Players.Events;

namespace projektRPG.MODEL.Board.Themes
{
    /// <summary>
    /// Triwizard Tournament hedge maze theme — goblins, Dementors, Death Eaters.
    /// Artifact: Triwizard Cup. Uses nested Decorators on the Sword of Gryffindor (Strong + Unlucky).
    /// </summary>
    public class TriwizardMazeTheme : ITheme
    {
        private static readonly Random Rand = Random.Shared;

        private readonly SpeciesGroup weakEnemies = new SpeciesGroup("Dementors");
        private readonly SpeciesGroup braveEnemies = new SpeciesGroup("DeathEaters");
        private readonly SpeciesGroup neutralEnemies = new SpeciesGroup("Goblins");

        private int neutralCreated = 0;
        private int weakCreated = 0;
        private int braveCreated = 0;
        private const int MinPerType = 2;

        public IDungeonStrategy BuildStrategy => new MazeStrategy();

        public string ThemeName => "Triwizard Tournament Maze";
        public string WelcomeMessage() =>
            "The towering hedges seem to close in around you; you have no idea what awaits beyond the next turn...";

        public string ArtifactName => "Triwizard Tournament Cup";
        public string ItemNames => "Golden Snitch, Magic Compass";
        public string WeaponNames => "Cedric Diggory's Wand, Sword of Gryffindor, Golden Egg";
        public string EnemyNames => "Goblins, Dementors, and Death Eaters";

        public Item Artifact() => new TriwizardCup();

        public Item CreateItem()
        {
            int choice = Rand.Next(2);

            return choice switch
            {
                0 => new LuckyModifier(new GoldenSnitch()),
                1 => new MagicCompass(),
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
                0 => new CedriksWand(),
                1 => new StrongModifier(new UnluckyModifier(new SwordOfGryffindor())),
                2 => new GoldenEgg(),
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
                return new NeutralEnemy(neutralEnemies, noisePublisher, eventRecorder, "Goblin");
            }

            int choice = Rand.Next(3);

            return choice switch
            {
                0 => new WeakEnemy(weakEnemies, noisePublisher, eventRecorder, "Dementor"),
                1 => new BraveEnemy(braveEnemies, noisePublisher, eventRecorder, "DeathEater"),
                2 => new NeutralEnemy(neutralEnemies, noisePublisher, eventRecorder, "Goblin"),
                _ => new WeakEnemy(weakEnemies, noisePublisher, eventRecorder, "Dementor")
            };
        }
    }
}
