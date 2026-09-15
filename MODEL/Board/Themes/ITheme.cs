using projektRPG.MODEL.Board.Strategy;
using projektRPG.MODEL.Items;
using projektRPG.MODEL.Players.Enemies;
using projektRPG.MODEL.Players.Events;

namespace projektRPG.MODEL.Board.Themes
{
    /// <summary>
    /// Factory interface for a complete dungeon theme.
    /// Pattern: Abstract Factory — each theme creates its own items, weapons, enemies, and map strategy.
    /// Used by LabyrinthBuilder (map content) and DescriptionBuilder (intro text).
    /// </summary>
    public interface ITheme
    {
        /// <summary>Procedural generation parameters for this theme's map layout.</summary>
        IDungeonStrategy BuildStrategy { get; }

        string ArtifactName { get; }
        string ItemNames { get; }
        string WeaponNames { get; }
        string EnemyNames { get; }

        string ThemeName { get; }
        string WelcomeMessage();

        /// <summary>Creates the unique legendary item hidden in this dungeon.</summary>
        Item Artifact();

        Item CreateItem();
        Item CreateCurrency();
        Item CreateWeapon();

        /// <summary>
        /// Creates an enemy and registers it with Observer publishers (noise / death events).
        /// </summary>
        Enemy CreateEnemy(INoisePublisher noisePublisher, IGameEventRecorder eventRecorder);
    }
}
