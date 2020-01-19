using System;
using System.Collections.Generic;
using GameLovers.Services;
using NSubstitute;
using NUnit.Framework;

// ReSharper disable once CheckNamespace

namespace GameLoversEditor.Services.Tests
{
	public class PoolServiceTest
	{
		private PoolService _poolService;
		private PoolableEntity _poolableEntity;
		private int initialSize = 5;

		public class PoolableEntity : IPoolEntitySpawn, IPoolEntityDespawn, IPoolEntityCleared
		{
			public void OnSpawn() {}
			public void OnDespawn() {}
			public void OnCleared() {}
		}

		[SetUp]
		public void Init()
		{
			_poolService = new PoolService();
			_poolableEntity = Substitute.For<PoolableEntity>();
			
			_poolService.InitPool(initialSize, () => Substitute.For<PoolableEntity>());
		}

		[Test]
		public void Initialize_SameType_ThrowsException()
		{
			Assert.Throws<ArgumentException>(() =>
			{
				_poolService.InitPool(initialSize, () => Substitute.For<PoolableEntity>());
			});
		}

		[Test]
		public void Spawn_Successfully()
		{
			var newEntity = _poolService.Spawn<PoolableEntity>();
			
			newEntity.Received().OnSpawn();
			
			Assert.AreNotSame(_poolableEntity, newEntity);
		}

		[Test]
		public void Spawn_EmptyPool_Successfully()
		{
			var initialSize = 0;
			var poolService = new PoolService();
			
			poolService.InitPool(initialSize, () => Substitute.For<PoolableEntity>());
			
			var newEntity = _poolService.Spawn<PoolableEntity>();
			
			newEntity.Received().OnSpawn();
			
			Assert.AreNotSame(_poolableEntity, newEntity);
		}

		[Test]
		public void Spawn_NotInitialized_ThrowsException()
		{
			var poolService = new PoolService();
			
			Assert.Throws<ArgumentException>(() => poolService.Spawn<PoolableEntity>());
		}

		[Test]
		public void Despawn_Successfully()
		{
			_poolService.Despawn(_poolableEntity);
			
			_poolableEntity.Received().OnDespawn();
		}

		[Test]
		public void Despawn_NotInitialized_ThrowsException()
		{
			var poolService = new PoolService();
			
			Assert.Throws<ArgumentException>(() => poolService.Despawn(_poolableEntity));
		}

		[Test]
		public void DespawnAll_Successfully()
		{
			var entities = new List<PoolableEntity>();

			for (int i = 0; i < initialSize; i++)
			{
				entities.Add(Substitute.For<PoolableEntity>());
			}
			
			_poolService.DespawnAll<PoolableEntity>();

			foreach (var entity in entities)
			{
				entity.Received().OnDespawn();
			}
		}

		[Test]
		public void Clear_Successfully()
		{
			_poolService.Despawn(_poolableEntity);
			_poolService.Clear<PoolableEntity>();
			
			_poolableEntity.Received().OnCleared();
			
			Assert.Throws<ArgumentException>(() => _poolService.Spawn<PoolableEntity>());
			Assert.Throws<ArgumentException>(() => _poolService.Despawn(_poolableEntity));
		}

		[Test]
		public void Clear_NotInitialized_ThrowsException()
		{
			var poolService = new PoolService();
			
			Assert.Throws<ArgumentException>(() => poolService.Clear<PoolableEntity>());
		}

		[Test]
		public void Clear_SameType_ThrowsException()
		{
			_poolService.Clear<PoolableEntity>();
			
			Assert.Throws<ArgumentException>(() => _poolService.Clear<PoolableEntity>());
		}
	}
}