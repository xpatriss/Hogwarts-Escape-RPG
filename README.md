# Hogwarts Escape — Multiplayer Console RPG

A multiplayer turn-based console RPG implemented in C# (.NET 8) featuring procedural dungeon generation, authoritative client-server network architecture, and extensive use of Object-Oriented Design Patterns (GoF).

> **Note:** This project serves as a functional foundation and architectural showcase, with further improvements and feature expansions planned for the future.
---

## Architecture Overview

The system is built around an authoritative server model with a clear separation of concerns (MVC), ensuring that clients only render state representations (DTOs) and dispatch action intent without directly manipulating the global game state.

```
+-------------------------------------------------------------+
|                     Authoritative Server                    |
|  +--------------------+             +--------------------+  |
|  |    GameModel       | <---------+ |  ServerController  |  |
|  | (Authoritative)    |             |  (Main Tick Loop)  |  |
|  +--------------------+             +--------------------+  |
|           |                                    ^            |
|           v                                    |            |
|  +-------------------------------------------------------+  |
|  |            ServerNetworkManager (TCP Sockets)         |  |
+--+-------------------------------------------------------+--+
                                |  ^
              JSON DTO Snapshots|  | JSON ActionDTOs
                                v  |
+-------------------------------------------------------------+
|                         Game Client                         |
|  +-------------------------------------------------------+  |
|  |            ClientNetworkManager (TCP Sockets)         |  |
|  +-------------------------------------------------------+  |
|           |                                    ^            |
|           v                                    |            |
|  +--------------------+             +--------------------+  |
|  |  ClientController  | ----------> |  LocalStateModel   |  |
|  +--------------------+             +--------------------+  |
|           |                                                 |
|           v                                                 |
|  +--------------------+                                     |
|  |     GameView       | (Console ANSI / Pastel rendering)   |
|  +--------------------+                                     |
+-------------------------------------------------------------+
```

---

## Design Patterns Showcase

* **MVC (Model-View-Controller)**: Strict separation between data modeling (`GameModel`, `LocalStateModel`), presentation (`GameView`), and input/state coordination (`ServerController`, `ClientController`).
* **State Pattern**: Dynamically alters permissible player actions and UI keybindings based on contextual state (`BoardState`, `AttackState`, `FieldState`, `InventoryState`, `LogState`).
* **Chain of Responsibility**: Processes raw console keystrokes down a pipeline (`InputHandler` -> `BoardHandler` -> `DefaultHandler`), decoupling input reading from command execution.
* **Visitor Pattern**: Resolves combat formulas dynamically. Attack visitors (`BasicAttack`, `MagicAttack`, `SneakyAttack`) visit concrete weapon types (`HeavyWeapon`, `LightWeapon`, `MagicWeapon`) to compute damage and defense ratings against player stats.
* **Decorator Pattern**: Dynamically augments item and weapon attributes at runtime (`ItemDecorator`, `StrongModifier`, `HealthyModifier`, `LuckyModifier`, `UnluckyModifier`).
* **Abstract Factory Pattern**: Produces cohesive families of themed dungeon entities (items, weapons, currencies, enemies, generation strategy) via the `ITheme` interface (`HogwartsDungeonsTheme`, `LibraryTheme`, `TriwizardMazeTheme`).
* **Builder & Director**: Procedurally constructs dungeon rooms, corridors, and obstacle boundaries (`LabyrinthBuilder`) while simultaneously assembling introductory narrative lore (`DescriptionBuilder`) under a single construction pipeline (`Director`).
* **Strategy Pattern**:
  * **World Generation**: `IDungeonStrategy` (`DungeonsStrategy`, `LibraryStrategy`, `MazeStrategy`) encapsulates room and corridor generation parameters.
  * **Enemy AI**: `IEnemyBehavior` (`FollowPlayerBehavior`, `FleePlayerBehavior`, `FollowSoundBehavior`, `FleeSoundBehavior`, `RandomMoveBehavior`) controls enemy pathfinding and combat engagement based on sensory inputs.
* **Observer Pattern**:
  * **Acoustic Detection**: `INoisePublisher` notifies `INoiseObserver` (enemies) when loud items are dropped or picked up within hearing radius.
  * **Species Reaction**: `IDeathPublisher` (`SpeciesGroup`) notifies surviving monsters of the same species when one is defeated (triggering rage, fear, or indifference).
* **Singleton Pattern**: Ensures a single centralized instance for runtime configuration (`ConfigurationManager`) and event logging (`Logger`).
* **Data Transfer Object (DTO)**: Serializes network communication across TCP sockets (`GameStateDTO`, `ActionDTO`, `PlayerPrivateDTO`, `PlayerPublicDTO`), hiding server internals from clients.

---

## Game Mechanics & Features

### 1. Themed Dungeons (Abstract Factory)
The game world dynamically adapts its lore, artifacts, loot tables, and enemy encounters based on the configured theme (`ITheme`):
* **Hogwarts Dungeons**: Damp, dark catacombs housing Dementors, Death Eaters, and Slytherin students, with **The Elder Wand** as its hidden relic.
* **Hogwarts Library**: A labyrinth of bookshelves guarded by Trolls, Goblins, and rival students, hiding the legendary **Invisibility Cloak**.
* **Triwizard Maze**: Dangerous overgrown hedges patrolled by hostile creatures, centering around the **Triwizard Tournament Cup**.

### 2. Items, Currency & Runtime Modifiers
Every object lying in the dungeon affects player progression and emits sound upon interaction:
* **Passive Currencies**: Galleons and raw Gold pieces that directly increase the player's fortune upon pickup.
* **Magical Consumables & Relics**: Themed artifacts (e.g., *Butterbeer*, *Felix Felicis*, *Time-Turner*, *Golden Snitch*) that modify base stats.
* **Dynamic Modifiers (Decorator Pattern)**: Items can spawn with magical affixes that alter their properties:
  * `(Healthy)`: Increases the player's maximum health bonus.
  * `(Lucky)` / `(Unlucky)`: Alters critical hit chances and luck-dependent calculations.
  * `(Strong)`: Adds flat bonus damage to weapons.

### 3. Weapons & Attack Strategies (Visitor Pattern)
Combat efficiency depends on the synergy between the equipped weapon category, player attributes, and the chosen attack stance:

* **Weapon Classes**:
  * **Light Weapons** (*Basilisk Fang*, *Quill*, *Golden Egg*): Low noise generation; damage scales with **Dexterity** and **Luck**. Require 1 hand.
  * **Heavy Weapons** (*Troll's Club*, *Sword of Gryffindor*, *Dagger (2H)*): High damage and high noise signature; damage scales with **Strength** and **Aggression**. May require 1 or 2 hands.
  * **Magic Weapons** (*Elder Wand*, *Book of Spells*, *Cedric's Wand*): Arcane offensive tools scaling primarily with **Wisdom**.
* **Combat Stances (`IAttackVisitor`)**:
  * **Basic Attack**: Balanced strike utilizing weapon base damage and primary physical attributes.
  * **Sneaky Attack**: High-risk, high-reward stance that doubles the effectiveness of Light Weapons while severely penalizing Heavy Weapons.
  * **Magic Attack**: Channels player Wisdom to penetrate armor and project strong arcane shields when wielding Magic Weapons.

### 4. Reactive Enemy AI (Strategy & Observer Patterns)
Enemies are not static obstacles; they listen to the dungeon environment and adapt dynamically:
* **Sensory Awareness**:
  * **Line of Sight**: Enemies detect approaching players across clear horizontal/vertical corridors up to their vision range.
  * **Acoustic Tracking (Observer)**: Picking up or dropping loud heavy weapons propagates noise through dungeon corridors; nearby monsters calculate path distance and track the disturbance.
* **Archetypes**:
  * **Brave Enemies** (*Death Eaters*): Relentlessly chase players on sight or sound. Witnessing the death of a species companion throws them into a rage, boosting their Attack and Armor.
  * **Weak Enemies** (*Dementors*): Timid creatures that flee from sight or sounds. Watching a companion die weakens their resolve, dropping their combat attributes.
  * **Neutral Enemies** (*Trolls, Slytherin Students*): Passive roamers that ignore players until attacked. Once provoked, they aggressively retaliate while above 50% HP, or turn to flee when critically wounded.

---

## Gameplay & Controls

### Objective
Explore procedural dungeons inspired by Hogwarts locations, scavenge legendary equipment and potions, engage dangerous creatures in turn-based combat, and survive the trials together with or against other wizards.

### Keybindings

| Key | Action | Context |
| :--- | :--- | :--- |
| `W`, `S`, `A`, `D` | Move Up / Down / Left / Right | Exploration (`BoardState`, `FieldState`, `AttackState`) |
| `E` | Pick up item from ground | Ground interaction (`FieldState`) |
| `I` | Open Inventory | Exploration / Combat |
| `Q` | Drop selected item | Inventory (`InventoryState`) |
| `R` | Equip selected item to hand | Inventory (`InventoryState`) |
| `F` | Free hand (unequip weapon) | Inventory (`InventoryState`) |
| `↑` / `↓` | Cycle item or attack selection | Menus (`FieldState`, `InventoryState`, `AttackState`) |
| `Enter` | Execute selected attack | Combat (`AttackState`) |
| `J` | Open full event log | All states |
| `Esc` | Exit menu / Quit game | Menus / Global |

---

## Getting Started

### Prerequisites
* [.NET 8.0 SDK](https://dotnet.microsoft.com/download) or higher.
* Terminal with UTF-8 support (Windows Terminal, PowerShell 7+, Linux/macOS Terminal).

### Build
```bash
dotnet build
```

### Running the Game

The server must be running before any client attempts to connect.

#### Option 1: Interactive Launch
Launch the application executable in two separate terminal windows:
```bash
dotnet run
```
* In the **first terminal**, choose `[S]` to start the **Server** on port `5555`.
* In the **second terminal**, choose `[K]` to connect as a **Client**, enter your nickname, and press Enter.
* Additional players can join by launching more client instances.

#### Option 2: CLI Arguments
* **Start Server**:
  ```bash
  dotnet run -- -server 5555
  ```
* **Start Client**:
  ```bash
  dotnet run -- -client 127.0.0.1:5555
  ```

#### Option 3: Playing on Two Different Machines (Local Network / LAN)

1. **Host (Machine 1 - Server):**
   * Find your local IPv4 address:
     * **Windows:** Open terminal, run `ipconfig` and note the `IPv4 Address` (e.g. `192.168.1.45`).
     * **Linux/macOS:** Run `ip a` or `ifconfig`.
   * Start the server on port `5555`:
     ```bash
     dotnet run -- -server 5555
     ```
   * *(Note: Ensure port `5555` is allowed through your OS firewall for incoming connections).*
   * You can also join as a player locally on this machine:
     ```bash
     dotnet run -- -client 127.0.0.1:5555
     ```

2. **Guest (Machine 2 - Client):**
   * Connect to the host using Machine 1's local IP address and port:
     ```bash
     dotnet run -- -client 192.168.1.45:5555
     ```
   * Enter your player nickname when prompted. Both players will appear on the shared dungeon map.

---

## Configuration (`config.json`)

The server and client load parameters dynamically from `config.json`:

```json
{
  "PlayerName": "Harry",
  "DungeonTheme": "HogwartsDungeons",
  "LogFilePath": "logs"
}
```

* **`DungeonTheme` options**: `HogwartsDungeons`, `TriwizardMaze`, `Library`.

---

## Tech Stack
* **Language**: C# 12 / .NET 8
* **Networking**: TCP Sockets (`TcpListener`, `TcpClient`, `NetworkStream`)
* **Serialization**: `System.Text.Json`
* **Concurrency**: `System.Threading.Tasks`, `ConcurrentQueue`
* **Terminal UI**: ANSI escape sequences via [Pastel](https://github.com/silkfire/Pastel)