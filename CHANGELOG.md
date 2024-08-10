# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [0.9.0] - 2024-08-10

**Changed**:

## [0.8.1] - 2023-08-27

- Added GitEditorProcess class to run Git commands as processes, enabling checks for valid Git repositories, retrieving current branch names, commit hashes, and diffs from given commits.
- Introduced VersionEditorUtils class for managing application versioning. This includes setting and saving the internal version before building, loading version data from disk, and generating an internal version suffix based on Git information and build settings.

**Changed**:
- Enhanced IInstaller interface with new methods for binding multiple type interfaces to a single instance, improving modularity and code organization.

## [0.8.0] - 2023-08-05

- Introduced MainInstaller, a singleton class for managing instances in the project.
- Added RngService for generating and managing random numbers.
- Implemented VersionServices to manage application version, including asynchronous loading of version data and comparison of version strings.

## [0.7.1] - 2023-07-28

**Fixed**:
- Compilation errors in various test files and the PoolService class have been fixed.

**Changed**:
- Tests have been moved to proper folders, and the package number has been updated.
- An unused namespace import has been removed from the InstallerTest class.

## [0.7.0] - 2023-07-28

- Introduced a code review process using GitHub Actions workflow.
- Added IInstaller interface and Installer implementation for binding and resolving instances.
- Updated namespaces, removed unused code, and modified method calls in test classes.

**Changed**:
- Removed dependency on ICommandNetworkService and SendCommand method in CommandService.
- Updated IDataService interface and DataService class to handle local and online data saving.
- Improved readability of MessageBrokerService class by using var for type inference.
- Removed unused network service related interfaces, classes, and methods.
- Modified calculation of overFlow in TickService to check for zero DeltaTime.

## [0.6.2] - 2020-09-10

- Made *NetworkService* abstract and removed *INetworkService* to make easier to work with
- Improved Readme documentation

## [0.6.1] - 2020-09-09

- Added connection between *NetworkService* & *CommandService*
- Added integration tests

## [0.6.0] - 2020-09-09

- Added *NetworkService*
- Improved Readme documentation

## [0.5.0] - 2020-07-10

**Changed**:
- Renamed *IDataWriter* and it's *FlushData* methods to *IDataSaver* & *SaveData* respectively to match with it's execution logic scope
- Moved the *AddData* to the *IDataService* to allow the *IDataSaver* have the single responsibility of saving data into disk  

## [0.4.1] - 2020-07-09

- Added *CommandService*

## [0.4.0] - 2020-07-09

- Added *DataService*

## [0.3.1] - 2020-02-25

**Fixed**:
- Fixed object pool despawn all elements. It was not despawning all the elements
- Fixed issue preventing to stop coroutines and thrown MissingReferenceException

## [0.3.0] - 2020-02-09

- Now the *MainInstaller* checks the object binding relationship in compile time
- The *CoroutineService* no longer fails on null coroutines
- Improved the *ObjectPools* helper classes with a now static global instatiator for game objects.

**Changed**:
- Now the *PoolService* is only a service container for objects pools and no longer creates/initializes new pools.
- Removed *Pool.Clear* functionality. Use *DespawnAll* or delete the pool instead 

## [0.2.0] - 2020-01-19

- Added new *ObjectPool* & *GameObjectPool* pools to allow to allow to use object pools independent from the *PoolService*. This allows to have different pools of the same type in the project in different object controllers
- Added new interface *IPoolEntityClear* that allows a callback method for entities when they are cleared from the pool
- Added new unit tests for the *ObjectPool*

**Changed**:
- Now the *PoolService.Clear()* does not take any action parameters. To have a callback when the entity is cleared, please have the entity implement the *IPoolEntityClear* interface

## [0.1.1] - 2020-01-06

- Added License

## [0.1.0] - 2020-01-06

- Initial submission for package distribution
