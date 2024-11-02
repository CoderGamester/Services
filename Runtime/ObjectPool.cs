using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

// ReSharper disable once CheckNamespace

namespace GameLovers.Services
{
	/// <summary>
	/// This interface allows pooled objects to be notified when it is spawned
	/// </summary>
	public interface IPoolEntitySpawn
	{
		/// <summary>
		/// Invoked when the Entity is spawned
		/// </summary>
		void OnSpawn();
	}

	/// <inheritdoc cref="IPoolEntitySpawn"/>
	/// <remarks>
	/// This interface allows to spawn the pooled object with the given <typeparamref name="T"/> <paramref name="data"/>
	/// </remarks>
	public interface IPoolEntitySpawn<T>
	{
		/// <inheritdoc cref="IPoolEntitySpawn.OnSpawn"/>
		/// <remarks>
		/// Allows to spawn the pooled object with the given <typeparamref name="T"/> <paramref name="data"/>
		/// </remarks>
		void OnSpawn(T data);
	}

	/// <summary>
	/// This interface allows pooled objects to be notified when it is despawned
	/// </summary>
	public interface IPoolEntityDespawn
	{
		/// <summary>
		/// Invoked when the entity is despawned
		/// </summary>
		void OnDespawn();
	}

	/// <summary>
	/// This interface allows to self despawn by maintaining the reference of the pool that created it
	/// </summary>
	/// <remarks>
	/// Implemenation of this class:
	/// <code>
	/// public class MyObjectPool : IPoolEntityObject<typeparamref name="T"/>
	/// {
	///		private IObjectPool<typeparamref name="T"/> _pool;
	///		
	/// 	public void Init(IObjectPool<typeparamref name="T"/> pool)
	/// 	{
	/// 		_pool = pool;
	/// 	}
	/// 	
	/// 	public bool Despawn()
	/// 	{
	/// 		return _pool.Despawn(this);
	/// 	}	
	/// }
	/// </code>
	/// </remarks>
	public interface IPoolEntityObject<T> where T : class
	{
		/// <summary>
		/// Called by the <see cref="IObjectPool{T}"/> to initialize by the given <paramref name="pool"/>
		/// </summary>
		void Init(IObjectPool<T> pool);

		/// <summary>
		/// Despawns this pooled object
		/// </summary>
		bool Despawn();
	}

	/// <summary>
	/// Simple object pool implementation that can handle any type of entity objects
	/// </summary>
	public interface IObjectPool : IDisposable
	{
		/// <summary>
		/// Despawns all active spawned entities and returns them back to the pool to be used again later
		/// This function does not reset the entity. For that, have the entity implement <see cref="IPoolEntityDespawn"/> or do it externally
		/// </summary>
		void DespawnAll();
	}

	/// <inheritdoc />
	public interface IObjectPool<T> : IObjectPool where T : class
	{
		/// <summary>
		/// Requests the collection of already spawned elements as a read only list
		/// </summary>
		IReadOnlyList<T> SpawnedReadOnly { get; }

		/// <summary>
		/// Checks if there is an entity in the bool that matches the given <paramref name="conditionCheck"/>
		/// </summary>
		bool IsSpawned(Func<T, bool> conditionCheck);

		/// <summary>
		/// Spawns and returns an entity of the given type <typeparamref name="T"/>
		/// This function does not initialize the entity. For that, have the entity implement <see cref="IPoolEntitySpawn"/>
		/// or do it externally
		/// This function throws a <exception cref="StackOverflowException" /> if the pool is empty
		/// </summary>
		T Spawn();

		/// <inheritdoc cref="Spawn"/>
		/// <remarks>
		/// This interface allows to spawn the pooled object with the given <typeparamref name="T"/> <paramref name="data"/>
		/// </remarks>
		T Spawn<TData>(TData data);

		/// <summary>
		/// Despawns the entity that is valid with the given <paramref name="entityGetter"/> condition and returns it back to
		/// the pool to be used again later.
		/// If the given <paramref name="onlyFirst"/> is true then will only despawn one entity and not find more entities
		/// that match the given <paramref name="entityGetter"/> condition.
		/// This function does not reset the entity. For that, have the entity implement <see cref="IPoolEntityDespawn"/>
		/// or do it externally.
		/// Returns true if was able to despawn the entity back to the pool successfully, false otherwise
		/// </summary>
		bool Despawn(bool onlyFirst, Func<T, bool> entityGetter);

		/// <summary>
		/// Despawns the given <paramref name="entity"/> and returns it back to the pool to be used again later.
		/// This function does not reset the entity. For that, have the entity implement <see cref="IPoolEntityDespawn"/>
		/// or do it externally.
		/// Returns true if was able to despawn the entity back to the pool successfully, false otherwise.
		/// </summary>
		bool Despawn(T entity);

		/// <summary>
		/// Clears the contents out of this pool.
		/// Returns back its pool contents so they can be independently disposed
		/// </summary>
		List<T> Clear();
	}

	/// <inheritdoc />
	public abstract class ObjectPoolBase<T> : IObjectPool<T> where T : class
	{
		public readonly T SampleEntity;

		protected readonly IList<T> SpawnedEntities = new List<T>();

		private readonly Stack<T> _stack;
		private readonly Func<T, T> _instantiator;

		/// <inheritdoc />
		public IReadOnlyList<T> SpawnedReadOnly => SpawnedEntities as IReadOnlyList<T>;

		protected ObjectPoolBase(uint initSize, T sampleEntity, Func<T, T> instantiator)
		{
			SampleEntity = sampleEntity;
			_instantiator = instantiator;
			_stack = new Stack<T>((int)initSize);

			for (var i = 0; i < initSize; i++)
			{
				_stack.Push(CallInstantiator());
			}
		}

		/// <inheritdoc />
		public bool IsSpawned(Func<T, bool> conditionCheck)
		{
			for (var i = 0; i < SpawnedEntities.Count; i++)
			{
				if (conditionCheck(SpawnedEntities[i]))
				{
					return true;
				}
			}

			return false;
		}

		/// <inheritdoc />
		public bool Despawn(bool onlyFirst, Func<T, bool> entityGetter)
		{
			var despawned = false;

			for (var i = 0; i < SpawnedEntities.Count; i++)
			{
				if (!entityGetter(SpawnedEntities[i]))
				{
					continue;
				}

				despawned = Despawn(SpawnedEntities[i]);

				if (onlyFirst)
				{
					break;
				}
			}

			return despawned;
		}

		/// <inheritdoc />
		public List<T> Clear()
		{
			var ret = new List<T>(SpawnedEntities);

			ret.AddRange(_stack);
			SpawnedEntities.Clear();
			_stack.Clear();

			return ret;
		}

		/// <inheritdoc />
		public void DespawnAll()
		{
			for (var i = SpawnedEntities.Count - 1; i > -1; i--)
			{
				Despawn(SpawnedEntities[i]);
			}
		}

		/// <inheritdoc />
		public T Spawn()
		{
			var entity = SpawnEntity();

			CallOnSpawned(entity);

			return entity;
		}

		/// <inheritdoc />
		public T Spawn<TData>(TData data)
		{
			var entity = SpawnEntity();

			CallOnSpawned(entity);
			CallOnSpawned(entity, data);

			return entity;
		}

		/// <inheritdoc />
		public bool Despawn(T entity)
		{
			if (!SpawnedEntities.Remove(entity) || entity == null || entity.Equals(null))
			{
				return false;
			}

			_stack.Push(entity);
			CallOnDespawned(entity);
			PostDespawnEntity(entity);

			return true;
		}

		public abstract void Dispose();

		protected virtual T SpawnEntity()
		{
			T entity = null;

			do
			{
				entity = _stack.Count == 0 ? CallInstantiator() : _stack.Pop();
			}
			// Need to do while loop and check as parent objects could have destroyed the entity/gameobject before it could
			// be properly disposed by pool service
			while (entity == null);

			SpawnedEntities.Add(entity);

			return entity;
		}

		protected virtual void PostDespawnEntity(T entity) { }

		protected T CallInstantiator()
		{
			var entity = _instantiator.Invoke(SampleEntity);
			var poolEntity = entity as IPoolEntityObject<T>;

			poolEntity?.Init(this);

			return entity;
		}

		protected void CallOnSpawned(T entity)
		{
			var poolEntity = entity as IPoolEntitySpawn;

			poolEntity?.OnSpawn();
		}

		protected void CallOnSpawned<TData>(T entity, TData data)
		{
			var poolEntity = entity as IPoolEntitySpawn<TData>;

			poolEntity?.OnSpawn(data);
		}

		protected void CallOnDespawned(T entity)
		{
			var poolEntity = entity as IPoolEntityDespawn;

			poolEntity?.OnDespawn();
		}
	}

	/// <inheritdoc />
	/// <remarks>
	/// Useful to for pools that use object references to create new instances (ex: GameObjects)
	/// </remarks>
	public class ObjectRefPool<T> : ObjectPoolBase<T> where T : class
	{
		public ObjectRefPool(uint initSize, T sampleEntity, Func<T, T> instantiator) : base(initSize, sampleEntity, instantiator)
		{
		}

		/// <inheritdoc />
		public override void Dispose()
		{
			Clear();
		}
	}

	/// <inheritdoc />
	public class ObjectPool<T> : ObjectRefPool<T> where T : class
	{
		public ObjectPool(uint initSize, Func<T> instantiator) : base(initSize, instantiator(), entityRef => instantiator.Invoke())
		{
		}
	}

	/// <inheritdoc />
	/// <remarks>
	/// Useful to for pools that use object references to create new <see cref="GameObject"/>
	/// </remarks>
	public class GameObjectPool : ObjectPoolBase<GameObject>
	{
		/// <summary>
		/// If true then when the object is despawned back to the pool will be parented to the same as the sample entity
		/// parent transform
		/// </summary>
		public bool DespawnToSampleParent { get; set; } = true;

		public GameObjectPool(uint initSize, GameObject sampleEntity) : base(initSize, sampleEntity, Instantiator)
		{
		}

		/// <inheritdoc />
		public override void Dispose()
		{
			var content = Clear();

			foreach (var obj in content)
			{
				Object.Destroy(obj);
			}
		}

		/// <summary>
		/// Generic instantiator for <see cref="GameObject"/> pools
		/// </summary>
		public static GameObject Instantiator(GameObject entityRef)
		{
			var instance = Object.Instantiate(entityRef, entityRef.transform.parent, true);

			instance.SetActive(false);

			return instance;
		}

		protected override GameObject SpawnEntity()
		{
			var entity = base.SpawnEntity();

			entity.SetActive(true);

			return entity;
		}

		protected override void PostDespawnEntity(GameObject entity)
		{
			entity.SetActive(false);

			if (DespawnToSampleParent && SampleEntity != null)
			{
				entity.transform.SetParent(SampleEntity.transform.parent);
			}
		}
	}

	/// <inheritdoc />
	/// <remarks>
	/// Useful to for pools that use object references to create new <see cref="GameObject"/> by their component reference
	/// </remarks>
	public class GameObjectPool<T> : ObjectPoolBase<T> where T : Behaviour
	{
		/// <summary>
		/// If true then when the object is despawned back to the pool will be parented to the same as the sample entity
		/// parent transform
		/// </summary>
		public bool DespawnToSampleParent { get; set; } = true;

		public GameObjectPool(uint initSize, T sampleEntity) : base(initSize, sampleEntity, Instantiator)
		{
		}

		public GameObjectPool(uint initSize, T sampleEntity, Func<T, T> instantiator) : base(initSize, sampleEntity, instantiator)
		{
		}

		/// <inheritdoc />
		public override void Dispose()
		{
			var content = Clear();

			foreach (var obj in content)
			{
				Object.Destroy(obj.gameObject);
			}
		}

		/// <summary>
		/// Generic instantiator for <see cref="GameObject"/> pools
		/// </summary>
		public static T Instantiator(T entityRef)
		{
			// ReSharper disable once MergeConditionalExpression
			var parent = entityRef == null ? null : entityRef.transform.parent;
			var instance = Object.Instantiate(entityRef, parent, true);

			instance.gameObject.SetActive(false);

			return instance;
		}

		protected override T SpawnEntity()
		{
			T entity = null;

			while(entity == null)
			{
				entity = base.SpawnEntity();

				if(entity.gameObject == null)
				{
					SpawnedEntities.Remove(entity);

					entity = null;
				}
			}

			entity.gameObject.SetActive(true);

			return entity;
		}

		protected override void PostDespawnEntity(T entity)
		{
			entity.gameObject.SetActive(false);

			if (DespawnToSampleParent && SampleEntity is not null && !SampleEntity.Equals(null))
			{
				entity.transform.SetParent(SampleEntity.transform.parent);
			}
		}
	}
}