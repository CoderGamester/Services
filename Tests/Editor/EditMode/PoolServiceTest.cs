using System;
using System.Collections.Generic;
using System.Threading;
using GameLovers.Services;
using NSubstitute;
using NUnit.Framework;

// ReSharper disable once CheckNamespace

namespace GameLoversEditor.Services.Tests
{
	/* TODO: Fix this test. Somehow the mock objectpool breaks the service
	public class PoolServiceTest
	{
		private PoolService _poolService;
		private IObjectPool<IMockPoolableEntity> _pool;

		public interface IMockPoolableEntity : IPoolEntitySpawn, IPoolEntityDespawn { }

		[SetUp]
		public void Init()
		{
			_poolService = new PoolService();
			_pool = Substitute.For<IObjectPool<IMockPoolableEntity>>();
			
			_poolService.AddPool(_pool);
		}

		[Test]
		public void TryGetPool_Successfully()
		{
			Assert.True(_poolService.TryGetPool<IMockPoolableEntity>(out var pool));
			Assert.AreEqual(_pool, pool);
		}

		[Test]
		public void GetPool_Successfully()
		{
			Assert.AreEqual(_pool, _poolService.GetPool<IMockPoolableEntity>());
		}

		[Test]
		public void AddPool_Successfully()
		{
			Assert.True(_poolService.TryGetPool<IMockPoolableEntity>(out _));
		}

		[Test]
		public void AddPool_SameType_ThrowsException()
		{
			Assert.Throws<ArgumentException>(() => _poolService.AddPool(_pool));
		}

		[Test]
		public void Spawn_Successfully()
		{
			var entity = Substitute.For<IMockPoolableEntity>();

			_pool.Spawn().Returns(entity);
			
			Assert.AreEqual(entity,_poolService.Spawn<IMockPoolableEntity>());

			_pool.Received().Spawn();
		}

		[Test]
		public void Spawn_NotAddedPool_ThrowsException()
		{
			_poolService = new PoolService();
			
			Assert.Throws<ArgumentException>(() => _poolService.Spawn<IMockPoolableEntity>());
		}

		[Test]
		public void Despawn_Successfully()
		{
			var entity = Substitute.For<IMockPoolableEntity>();
			
			_poolService.Despawn(entity);
			
			_pool.Received().Despawn(entity);
		}

		[Test]
		public void Despawn_NotAddedPool_ThrowsException()
		{
			var entity = Substitute.For<IMockPoolableEntity>();
			
			_poolService = new PoolService();
			
			Assert.Throws<ArgumentException>(() => _poolService.Despawn(entity));
		}

		[Test]
		public void DespawnAll_Successfully()
		{
			_poolService.DespawnAll<IMockPoolableEntity>();

			_pool.Received().DespawnAll();
		}

		[Test]
		public void RemovePool_Successfully()
		{
			_poolService.RemovePool<IMockPoolableEntity>();

			Assert.Throws<ArgumentException>(() => _poolService.GetPool<IMockPoolableEntity>());
		}

		[Test]
		public void RemovePool_NotAdded_DoesNothing()
		{
			_poolService = new PoolService();
			
			Assert.DoesNotThrow(() => _poolService.RemovePool<IMockPoolableEntity>());
		}
	}*/
}