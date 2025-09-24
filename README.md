# GameLovers Services

[![Unity Version](https://img.shields.io/badge/Unity-6000.0%2B-blue.svg)](https://unity3d.com/get-unity/download)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![Version](https://img.shields.io/badge/version-0.15.1-green.svg)](CHANGELOG.md)

A comprehensive collection of services designed to streamline Unity game development by providing a robust, modular architecture foundation. This package offers essential services for command execution, data persistence, object pooling, messaging, and more.

## Table of Contents

- [Key Features](#key-features)
- [System Requirements](#system-requirements)
- [Installation](#installation)
- [Quick Start](#quick-start)
- [Services Documentation](#services-documentation)
  - [Command Service](#command-service)
  - [Coroutine Service](#coroutine-service)
  - [Data Service](#data-service)
  - [Main Installer](#main-installer)
  - [Message Broker Service](#message-broker-service)
  - [Network Service](#network-service)
  - [Pool Service](#pool-service)
  - [Tick Service](#tick-service)
  - [Time Service](#time-service)
- [Package Structure](#package-structure)
- [Dependencies](#dependencies)
- [Contributing](#contributing)
- [Support](#support)
- [License](#license)

## Key Features

- **🏗️ Dependency Injection** - Simple DI framework with MainInstaller
- **📨 Message Broker** - Decoupled communication system
- **🎮 Command Pattern** - Seamless command execution layer
- **🔄 Object Pooling** - Efficient memory management
- **💾 Data Persistence** - Cross-platform save/load system
- **⏱️ Time Management** - Precise game time control
- **🔄 Coroutine Management** - Enhanced coroutine control
- **🌐 Network Abstraction** - Extensible network service base
- **⚡ Tick Service** - Centralized Unity update cycle management

## System Requirements

- **Unity** 2022.3 or higher
- **Git** (for installation via Package Manager)

## Installation

### Via Unity Package Manager (Recommended)

1. Open Unity Package Manager (`Window` → `Package Manager`)
2. Click the `+` button and select `Add package from git URL`
3. Enter the following URL:
   ```
   https://github.com/CoderGamester/com.gamelovers.services.git
   ```

### Via manifest.json

Add the following line to your project's `Packages/manifest.json`:

```json
{
  "dependencies": {
    "com.gamelovers.services": "https://github.com/CoderGamester/com.gamelovers.services.git"
  }
}
```

## Quick Start

Here's a simple example showing how to set up and use the core services:

```csharp
using UnityEngine;
using GameLovers.Services;

public class GameManager : MonoBehaviour
{
    private IMessageBrokerService _messageBroker;
    private ITickService _tickService;
    
    void Start()
    {
        // Initialize services using MainInstaller
        var installer = MainInstaller.Instance;
        installer.Bind<IMessageBrokerService>().ToSingle<MessageBrokerService>();
        installer.Bind<ITickService>().ToSingle<TickService>();
        
        // Resolve services
        _messageBroker = installer.Resolve<IMessageBrokerService>();
        _tickService = installer.Resolve<ITickService>();
        
        // Subscribe to game events
        _messageBroker.Subscribe<PlayerSpawnMessage>(OnPlayerSpawn);
        
        // Start tick service
        _tickService.Add(this);
    }
    
    void OnPlayerSpawn(PlayerSpawnMessage message)
    {
        Debug.Log($"Player spawned at position: {message.Position}");
    }
    
    public void OnTick(float deltaTime, double time)
    {
        // Game logic updated via tick service
    }
}

// Example message
public struct PlayerSpawnMessage : IMessage
{
    public Vector3 Position;
    public int PlayerId;
}
```

## Services Documentation

<a name="command-service"></a>
### Command Service

Creates a seamless abstraction layer of execution between game logic and other code parts by invoking commands.

**Key Features:**
- Type-safe command execution
- Async command support
- Built-in message broker integration
- Command queuing and batching

**Basic Usage:**

```csharp
// Define a command
public struct MovePlayerCommand : ICommand
{
    public int PlayerId;
    public Vector3 Direction;
    public float Speed;
}

// Create command service
var commandService = new CommandService(messageBrokerService);

// Execute command
await commandService.ExecuteCommand(new MovePlayerCommand 
{
    PlayerId = 1,
    Direction = Vector3.forward,
    Speed = 5f
});

// Execute command without awaiting
commandService.ExecuteCommand(new MovePlayerCommand 
{
    PlayerId = 2,
    Direction = Vector3.right,
    Speed = 3f
});
```

**Advanced Usage:**

```csharp
// Command with return value
public struct GetPlayerHealthCommand : ICommand<int>
{
    public int PlayerId;
}

// Execute and get result
int health = await commandService.ExecuteCommand<GetPlayerHealthCommand, int>(
    new GetPlayerHealthCommand { PlayerId = 1 });
```

<a name="coroutine-service"></a>
### Coroutine Service

Controls all coroutines in a non-destroyable object with callback support and state management.

**Key Features:**
- Centralized coroutine management
- End callbacks
- Coroutine state tracking
- Delayed execution support

**Basic Usage:**

```csharp
var coroutineService = new CoroutineService();

// Start a coroutine with callback
coroutineService.StartCoroutine(MyCoroutine(), () => 
{
    Debug.Log("Coroutine completed!");
});

// Start delayed execution
coroutineService.StartDelayCall(2f, () => 
{
    Debug.Log("Executed after 2 seconds");
});

IEnumerator MyCoroutine()
{
    yield return new WaitForSeconds(1f);
    Debug.Log("Coroutine step completed");
}
```

**Advanced Usage:**

```csharp
// Get coroutine reference for state checking
var asyncCoroutine = coroutineService.StartCoroutine(LongRunningTask());

// Check state
if (asyncCoroutine.IsRunning)
{
    // Stop if needed
    coroutineService.StopCoroutine(asyncCoroutine);
}
```

<a name="data-service"></a>
### Data Service

Provides cross-platform persistent data storage with automatic serialization support.

**Key Features:**
- Cross-platform data persistence
- Automatic JSON serialization
- Type-safe data operations
- Async loading/saving

**Basic Usage:**

```csharp
// Define data structure
[Serializable]
public class PlayerData
{
    public string Name;
    public int Level;
    public float Experience;
}

var dataService = new DataService();

// Save data
var playerData = new PlayerData 
{ 
    Name = "Hero", 
    Level = 10, 
    Experience = 1500f 
};
dataService.AddOrReplaceData("player", playerData);
await dataService.SaveData();

// Load data
await dataService.LoadData();
var loadedData = dataService.GetData<PlayerData>("player");
Debug.Log($"Loaded player: {loadedData.Name}, Level: {loadedData.Level}");
```

<a name="main-installer"></a>
### Main Installer

Provides a simple dependency injection framework for managing object instances and dependencies.

**Key Features:**
- Singleton and transient bindings
- Interface to implementation binding
- Multiple interface binding
- Compile-time relationship checking

**Basic Usage:**

```csharp
// Bind services
var installer = MainInstaller.Instance;
installer.Bind<IMessageBrokerService>().ToSingle<MessageBrokerService>();
installer.Bind<IDataService>().ToSingle<DataService>();

// Bind with multiple interfaces
installer.Bind<ITickService, IUpdatable>().ToSingle<TickService>();

// Resolve dependencies
var messageBroker = installer.Resolve<IMessageBrokerService>();
var dataService = installer.Resolve<IDataService>();
```

<a name="message-broker-service"></a>
### Message Broker Service

Enables decoupled communication between game systems using the Message Broker pattern.

**Key Features:**
- Type-safe messaging
- No direct references required
- Safe chain subscription handling
- High-performance publishing

**Basic Usage:**

```csharp
var messageBroker = new MessageBrokerService();

// Define messages
public struct GameStartMessage : IMessage
{
    public GameMode Mode;
    public int PlayerCount;
}

public struct PlayerDeathMessage : IMessage
{
    public int PlayerId;
    public Vector3 DeathPosition;
}

// Subscribe to messages
messageBroker.Subscribe<GameStartMessage>(OnGameStart);
messageBroker.Subscribe<PlayerDeathMessage>(OnPlayerDeath);

// Publish messages
messageBroker.Publish(new GameStartMessage 
{ 
    Mode = GameMode.Multiplayer, 
    PlayerCount = 4 
});

void OnGameStart(GameStartMessage message)
{
    Debug.Log($"Game started with {message.PlayerCount} players");
}

void OnPlayerDeath(PlayerDeathMessage message)
{
    Debug.Log($"Player {message.PlayerId} died at {message.DeathPosition}");
}

// Clean up
messageBroker.Unsubscribe<GameStartMessage>(OnGameStart);
```

**Safe Publishing for Chain Subscriptions:**

```csharp
// Use PublishSafe when subscribers might subscribe/unsubscribe during publishing
messageBroker.PublishSafe(new GameStartMessage 
{ 
    Mode = GameMode.SinglePlayer, 
    PlayerCount = 1 
});
```

<a name="network-service"></a>
### Network Service

Provides an extensible base for network operations and backend communication.

**Key Features:**
- Abstract network layer
- Backend communication support
- Extensible for custom implementations
- Integration with command service

**Basic Usage:**

```csharp
// Extend NetworkService for your needs
public class GameNetworkService : NetworkService
{
    protected override void ProcessNetworkLogic()
    {
        // Custom network processing
    }
    
    public async Task<PlayerData> GetPlayerData(int playerId)
    {
        // Custom backend call implementation
        return await FetchPlayerFromServer(playerId);
    }
}

// Usage
var networkService = new GameNetworkService();
var playerData = await networkService.GetPlayerData(123);
```

<a name="pool-service"></a>
### Pool Service

Manages object pools by type, providing efficient memory management and reuse.

**Key Features:**
- Type-based pool management
- GameObject and object pooling
- Automatic pool creation
- Spawn data support
- Independent pool access

**Basic Usage:**

```csharp
var poolService = new PoolService();

// Create pools
poolService.CreatePool<Bullet>(prefab: bulletPrefab, initialSize: 50);
poolService.CreatePool<Enemy>(prefab: enemyPrefab, initialSize: 20);

// Spawn objects
var bullet = poolService.Spawn<Bullet>();
var enemy = poolService.Spawn<Enemy>();

// Spawn with data
var powerfulBullet = poolService.Spawn<Bullet>(new BulletData 
{ 
    Damage = 100, 
    Speed = 20f 
});

// Despawn when done
poolService.Despawn(bullet);
poolService.Despawn(enemy);
```

**Advanced Pool Management:**

```csharp
// Get direct pool access
var bulletPool = poolService.GetPool<Bullet>();
var isSpawned = bulletPool.IsSpawned(bulletInstance);

// Reset pool to initial state
bulletPool.Reset();

// Clear all objects from pool
poolService.DespawnAll<Bullet>();
```

<a name="tick-service"></a>
### Tick Service

Provides centralized control over Unity's update cycle for better performance and organization.

**Key Features:**
- Centralized update management
- Multiple update frequencies
- Automatic overflow handling
- Performance optimization

**Basic Usage:**

```csharp
public class GameController : MonoBehaviour, ITickable
{
    private ITickService _tickService;
    
    void Start()
    {
        _tickService = new TickService();
        _tickService.Add(this);
    }
    
    public void OnTick(float deltaTime, double time)
    {
        // Your game logic here - called every frame
        UpdatePlayerMovement(deltaTime);
        UpdateEnemyAI(deltaTime);
    }
    
    void OnDestroy()
    {
        _tickService?.Remove(this);
    }
}
```

**Different Update Frequencies:**

```csharp
// Add with custom frequency (every 0.1 seconds)
_tickService.Add(this, 0.1f);

// Add for fixed update frequency
_tickService.AddFixed(this);
```

<a name="time-service"></a>
### Time Service

Provides precise control over game time with support for Unix timestamps, Unity time, and DateTime.

**Key Features:**
- Multiple time formats
- Precise time control
- Unix timestamp support
- DateTime integration

**Basic Usage:**

```csharp
var timeService = new TimeService();

// Get current times
var unityTime = timeService.UnityTime;
var unixTime = timeService.UnixTime;
var dateTime = timeService.DateTime;

// Time calculations
var futureTime = timeService.AddSeconds(300); // 5 minutes from now
var timeDifference = timeService.TimeDifference(futureTime, unityTime);

// Convert between formats
var unixFromDateTime = timeService.DateTimeToUnix(DateTime.Now);
var dateTimeFromUnix = timeService.UnixToDateTime(unixFromDateTime);
```

## Package Structure

```
<root>
  ├── package.json
  ├── README.md
  ├── CHANGELOG.md
  ├── LICENSE.md
  ├── Runtime
  │   ├── GameLovers.Services.asmdef
  │   ├── CommandService.cs
  │   ├── CoroutineService.cs
  │   ├── DataService.cs
  │   ├── MainInstaller.cs
  │   ├── MessageBrokerService.cs
  │   ├── NetworkService.cs
  │   ├── PoolService.cs
  │   ├── TickService.cs
  │   └── TimeService.cs
  └── Tests
      ├── Editor
      │   ├── GameLovers.Services.Editor.Tests.asmdef
      │   ├── CommandServiceTest.cs
      │   ├── DataServiceTest.cs
      │   ├── IntegrationTest.cs
      │   ├── MainInstallerTest.cs
      │   ├── MessageBrokerServiceTest.cs
      │   ├── NetworkServiceTest.cs
      │   ├── PoolServiceTest.cs
      │   ├── TickServiceTest.cs
      │   └── TimeServiceTest.cs
      └── Runtime
          ├── GameLovers.Services.Tests.asmdef
          └── CoroutineServiceTest.cs
```

## Dependencies

This package depends on:

- **[GameLovers Data Extensions](https://github.com/CoderGamester/com.gamelovers.dataextensions)** (v0.6.2) - Provides essential data structure extensions and utilities

All dependencies are automatically resolved when installing via Unity Package Manager.

## Contributing

We welcome contributions from the community! Here's how you can help:

### Reporting Issues

- Use the [GitHub Issues](https://github.com/CoderGamester/com.gamelovers.services/issues) page
- Include your Unity version, package version, and reproduction steps
- Attach relevant code samples, error logs, or screenshots

### Development Setup

1. Fork the repository on GitHub
2. Clone your fork locally
3. Create a feature branch: `git checkout -b feature/amazing-feature`
4. Make your changes and add tests if applicable
5. Commit your changes: `git commit -m 'Add amazing feature'`
6. Push to your branch: `git push origin feature/amazing-feature`
7. Submit a Pull Request

### Code Guidelines

- Follow C# naming conventions
- Add XML documentation for public APIs
- Include unit tests for new features
- Ensure backward compatibility when possible
- Update CHANGELOG.md for notable changes

### Pull Request Process

1. Ensure all tests pass
2. Update documentation if needed
3. Add changelog entry if applicable
4. Request review from maintainers

## Support

### Documentation

- **API Documentation**: See inline XML documentation
- **Examples**: Check the `Samples` folder for usage examples
- **Changelog**: See [CHANGELOG.md](CHANGELOG.md) for version history

### Getting Help

- **GitHub Issues**: [Report bugs or request features](https://github.com/CoderGamester/com.gamelovers.services/issues)
- **GitHub Discussions**: [Ask questions and share ideas](https://github.com/CoderGamester/com.gamelovers.services/discussions)

### Community

- Follow [@CoderGamester](https://github.com/CoderGamester) for updates
- Star the repository if you find it useful
- Share your projects using this package

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details.

---

**Made with ❤️ for the Unity community**

*If this package helps your project, consider giving it a star ⭐ on GitHub!*
