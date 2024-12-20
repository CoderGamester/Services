using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameLovers.Services;
using NSubstitute;
using NUnit.Framework;

// ReSharper disable once CheckNamespace

namespace GameLoversEditor.Services.Tests
{
	public class ObjectPoolTest
	{
		private ObjectPool<IMockEntity> _pool;
		private IMockEntity _mockEntity;
		private uint _initialSize = 5;

		public interface IMockEntity : IPoolEntitySpawn, IPoolEntityDespawn, IPoolEntityObject<IMockEntity>, IPoolEntitySpawn<object> { }
		public class MockEntity : IMockEntity
		{
			private IObjectPool<IMockEntity> _pool;

			public void Init(IObjectPool<IMockEntity> pool) => _pool = pool;

			public bool Despawn() => _pool.Despawn(this);
			public void OnDespawn()	{}

			public void OnSpawn() {}
			public void OnSpawn(object data) {}
		}

		[SetUp]
		public void Init()
		{
			_mockEntity = Substitute.For<IMockEntity>();
			_pool = new ObjectPool<IMockEntity>(_initialSize, () => _mockEntity);
		}

		[Test]
		public void Spawn_Successfully()
		{
			var newEntity = _pool.Spawn();
			
			newEntity.Received().OnSpawn();
			
			Assert.AreSame(_mockEntity, newEntity);
		}

		[Test]
		public void Spawn_WithData_Successfully()
		{
			var obj = new object();
			var newEntity = _pool.Spawn(obj);

			newEntity.Received().OnSpawn(obj);

			Assert.AreSame(_mockEntity, newEntity);
		}

		[Test]
		public void Spawn_ZeroInitialSize_Successfully()
		{
			var pool = new ObjectPool<IMockEntity>(0, () => _mockEntity);
			var newEntity = pool.Spawn();

			newEntity.Received().OnSpawn();

			Assert.AreSame(_mockEntity, newEntity);
		}

		[Test]
		public void Despawn_Successfully()
		{
			_pool.Spawn();

			Assert.IsTrue(_pool.Despawn(_mockEntity));
			_mockEntity.Received().OnDespawn();
		}

		/* Uncomment when finding someone that can help fix this interface
		[Test]
		public void EntityDespawn_Successfully()
		{
			var pool = Substitute.For<IObjectPool<IMockEntity>>();
			var entity = new MockEntity();

			entity.Init(pool);

			Assert.IsTrue(entity.Despawn());
			pool.Received().Despawn(entity);
		}*/

		[Test]
		public void Despawn_NotSpawnedObject_ReturnsFalse()
		{
			Assert.IsFalse(_pool.Despawn(_mockEntity));
			_mockEntity.DidNotReceive().OnDespawn();
		}

		[Test]
		public void DespawnAll_Successfully()
		{
			var newEntity1 = _pool.Spawn();
			var newEntity2 = _pool.Spawn();
			
			_pool.DespawnAll();

			newEntity1.Received().OnDespawn();
			newEntity2.Received().OnDespawn();
		}

		[Test]
		public void Clear_Successfully()
		{
			var clearedEntities = _pool.Clear();

			Assert.AreEqual(_initialSize, clearedEntities.Count);
		}
	}
}