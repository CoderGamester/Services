# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [0.2.0] - 2020-01-19

- Added new *ObjectPool* & *GameObjectPool* pools to allow to allow to use object pools independent from the *PoolService*. This allows to have different pools of the same type in the project in different object controllers
- Added new interface *IPoolEntityClear* that allows a callback method for entities when they are cleared from the pool
- Added new unit tests for the *ObjectPool*

**Changed**:
- Now the PoolService.Clear() does not take any action parameters. To have a callback when the entity is cleared, please have the entity implement the *IPoolEntityClear* interface

## [0.1.1] - 2020-01-06

- Added License

## [0.1.0] - 2020-01-06

- Initial submission for package distribution
