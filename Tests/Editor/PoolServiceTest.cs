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

		public abstract class PoolableEntity : IPoolEntitySpawn, IPoolEntityDespawn
		{
			public abstract void OnSpawn();
			public abstract void OnDespawn();
		}

		[SetUp]
		public void Init()
		{
			_poolService = new PoolService();
			_poolableEntity = Substitute.For<PoolableEntity>();
		}

		[Test]
		public void Initialize_SameType_ThrowsException()
		{
			var initialSize = 5;

			_poolService.InitPool(initialSize, _poolableEntity, poolableEntity => Substitute.For<PoolableEntity>());
			
			Assert.Throws<InvalidOperationException>(() =>
			{
				_poolService.InitPool(initialSize, _poolableEntity, poolableEntity => Substitute.For<PoolableEntity>());
			});
		}

		[Test]
		public void Spawn_Successfully()
		{
			var initialSize = 5;
			
			_poolService.InitPool(initialSize, _poolableEntity, poolableEntity => Substitute.For<PoolableEntity>());
			
			var newEntity = _poolService.Spawn<PoolableEntity>();
			
			newEntity.Received().OnSpawn();
			
			Assert.AreNotSame(_poolableEntity, newEntity);
		}

		[Test]
		public void Spawn_EmptyPool_Successfully()
		{
			var initialSize = 0;
			
			_poolService.InitPool(initialSize, _poolableEntity, poolableEntity => Substitute.For<PoolableEntity>());
			
			var newEntity = _poolService.Spawn<PoolableEntity>();
			
			newEntity.Received().OnSpawn();
			
			Assert.AreNotSame(_poolableEntity, newEntity);
		}

		[Test]
		public void Spawn_NotInitialized_ThrowsException()
		{
			Assert.Throws<ArgumentException>(() => _poolService.Spawn<PoolableEntity>());
		}

		[Test]
		public void Despawn_Successfully()
		{
			var initialSize = 5;
			
			_poolService.InitPool(initialSize, _poolableEntity, poolableEntity => Substitute.For<PoolableEntity>());
			_poolService.Despawn(_poolableEntity);
			
			_poolableEntity.Received().OnDespawn();
		}

		[Test]
		public void Despawn_NotInitialized_ThrowsException()
		{
			Assert.Throws<ArgumentException>(() => _poolService.Despawn(_poolableEntity));
		}

		[Test]
		public void DespawnAll_Successfully()
		{
			var initialSize = 5;
			var entities = new List<PoolableEntity>();
			
			_poolService.InitPool(initialSize, _poolableEntity, poolableEntity => Substitute.For<PoolableEntity>());

			for (int i = 0; i < initialSize; i++)
			{
				entities.Add(_poolService.Spawn<PoolableEntity>());
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
			var initialSize = 5;
			
			_poolService.InitPool(initialSize, _poolableEntity, poolableEntity => Substitute.For<PoolableEntity>());
			_poolService.Clear<PoolableEntity>(poolableEntity => { });
			
			Assert.Throws<ArgumentException>(() => _poolService.Spawn<PoolableEntity>());
			Assert.Throws<ArgumentException>(() => _poolService.Despawn(_poolableEntity));
		}

		[Test]
		public void Clear_NotInitialized_ThrowsException()
		{
			Assert.Throws<ArgumentException>(() => _poolService.Clear<PoolableEntity>(null));
		}

		[Test]
		public void Clear_SameType_ThrowsException()
		{
			var initialSize = 5;
			
			_poolService.InitPool(initialSize, _poolableEntity, poolableEntity => Substitute.For<PoolableEntity>());
			_poolService.Clear<PoolableEntity>(poolableEntity => { });
			
			Assert.Throws<ArgumentException>(() => _poolService.Clear<PoolableEntity>(null));
		}
	}
}