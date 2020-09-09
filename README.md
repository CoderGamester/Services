# Services Package

The purpose of this package is to provide a set of services to ease the development of a basic game architecture.

## System Requirements

- [Unity](http://unity3d.com/) 2019.4 or higher. Feel free to try older version.

## Service List

The services provided by this package are:

- [CommandService](#CommandService) - to create an seamless abstraction layer of execution between the game logic and any other part of the code by invoking commands
- [CoroutineService](#CoroutineService) - to control all coroutines in a non destroyable object with the possibility to have an end callback
- [DataService](#DataService) - to help loading and saving persistent game Data on the running platform
- [MainInstaller](#MainInstaller) - to provide a simple dependency injection binding installer framework
- [MessageBrokerService](#MessageBrokerService) - to help decoupled modules/systems to communicate with each other while maintaining the inversion of control principle
- [NetworkService](#NetworkService) - to provide the possibility to process any network code or to relay backend logic code to a game server running online
- [PoolService](#PoolService) - to control all object pools by type and allows to create independent self management object pools
- [TickService](#TickService) - to provide a single control point on Unity update cycle
- [TimeService](#TimeService) - to provide a precise control on the game's time (Unix, Unity or DateTime)

## Package Structure

```none
<root>
  ├── package.json
  ├── README.md
  ├── CHANGELOG.md
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
      │   ├── MainInstallerTest.cs
      │   ├── MessageBrokerServiceTest.cs
      │   ├── NetworkServiceTest.cs
      │   ├── PoolServiceTest.cs
      │   ├── MessageBrokerServiceTest.cs
      │   ├── TickServiceTest.cs
      │   └── TimeServiceTest.cs
      └── Runtime
           ├── GameLovers.Services.Tests.asmdef
           └── CoroutineServiceTest.cs
```

<a name="CommandService"></a>
### Command Service

Creates a seamless abstraction layer of execution between the game logic and any other part of the code by invoking commands

```csharp
// Example TODO
```

<a name="CoroutineService"></a>
### Coroutine Service

Controls all coroutines in a non destroyable object with the possibility to have an end callback

```csharp
// Example TODO
```

<a name="DataService"></a>
### Data Service

Helps loading and saving persistent game Data on the running platform

```csharp
// Example TODO
```

<a name="MainInstaller"></a>
### Main Installer

Provides a simple dependency injection binding installer framework

```csharp
// Example TODO
```

<a name="MessageBrokerService"></a>
### Message Broker Service

Helps decoupled modules/systems to communicate with each other while maintaining the inversion of control principle

```csharp
// Example TODO
```

<a name="NetworkService"></a>
### Network Service

Provides the possibility to process any network code or to relay backend logic code to a game server running online

```csharp
// Example TODO
```

<a name="PoolService"></a>
### Pool Service

Controls all object pools by type and allows to create independent self management object pools

```csharp
// Example TODO
```

<a name="TickService"></a>
### Tick Service

Provides a single control point on Unity update cycle

```csharp
// Example TODO
```

<a name="TimeService"></a>
### Time Service

Provides a precise control on the game's time (Unix, Unity or DateTime)

```csharp
// Example TODO
```
