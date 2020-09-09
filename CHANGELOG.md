# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [0.5.0] - 2020-07-10

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
