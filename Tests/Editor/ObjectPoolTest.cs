using System;
using System.Collections.Generic;
using GameLovers.Services;
using NSubstitute;
using NUnit.Framework;

// ReSharper disable once CheckNamespace

namespace GameLoversEditor.Services.Tests
{
	public class ObjectPoolTest
	{
		private ObjectPool<PoolableEntity> _pool;
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
			_pool = new ObjectPool<PoolableEntity>(initialSize, () => Substitute.For<PoolableEntity>());
			_poolableEntity = Substitute.For<PoolableEntity>();
		}

		[Test]
		public void Spawn_Successfully()
		{
			var newEntity = _pool.Spawn();
			
			newEntity.Received().OnSpawn();
			
			Assert.AreNotSame(_poolableEntity, newEntity);
		}

		[Test]
		public void Spawn_EmptyPool_Successfully()
		{
			var pool = new ObjectPool<PoolableEntity>(initialSize, () => Substitute.For<PoolableEntity>());
			
			var newEntity = pool.Spawn();
			
			newEntity.Received().OnSpawn();
			
			Assert.AreNotSame(_poolableEntity, newEntity);
		}

		[Test]
		public void Despawn_Successfully()
		{
			_pool.Despawn(_poolableEntity);
			
			_poolableEntity.Received().OnDespawn();
		}

		[Test]
		public void DespawnAll_Successfully()
		{
			var entities = new List<PoolableEntity>();

			for (int i = 0; i < initialSize; i++)
			{
				entities.Add(Substitute.For<PoolableEntity>());
			}
			
			_pool.DespawnAll();

			foreach (var entity in entities)
			{
				entity.Received().OnDespawn();
			}
		}

		[Test]
		public void Clear_Successfully()
		{
			_pool.Despawn(_poolableEntity);
			_pool.Clear();
			
			_poolableEntity.Received().OnCleared();
			
			Assert.DoesNotThrow(() => _pool.Spawn());
			Assert.DoesNotThrow(() => _pool.Despawn(_poolableEntity));
		}

		[Test]
		public void Clear_Twice_NothingHappens()
		{
			_pool.Clear();
			
			Assert.DoesNotThrow(() => _pool.Clear());
		}
	}
}