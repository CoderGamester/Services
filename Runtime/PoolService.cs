using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

// ReSharper disable once CheckNamespace

namespace GameLovers.Services
{
	/// <summary>
	/// This service allows to manage multiple pools of different types.
	/// The service can only a single pool of the same type. 
	/// </summary>
	public interface IPoolService
	{
		/// <summary>
		/// Initializes a new pool with the given <paramref name="initialSize"/>
		/// It invokes the <paramref name="instantiator"/> function every time a new entity is created in the pool
		/// </summary>
		void InitPool<T>(int initialSize, Func<T> instantiator) where T : new();
		
		/// <summary>
		/// Initializes a new pool with the given <paramref name="initialSize"/> and a sample entity given back in the <paramref name="instantiator"/>
		/// It invokes the <paramref name="instantiator"/> function every time a new entity is created in the pool
		/// </summary>
		void InitPool<T>(int initialSize, T sampleEntity, Func<T, T> instantiator) where T : Object;

		/// <summary>
		/// Checks if exists a pool of the given type already exists or needs to be initialized with
		/// <seealso cref="InitPool{T}(int,System.Func{T})"/>  before calling <seealso cref="Spawn{T}"/>
		/// </summary>
		bool HasPool<T>();

		/// <inheritdoc cref="HasPool{T}"/>
		bool HasPool(Type type);
		
		/// <inheritdoc cref="IObjectPool{T}.Spawn"/>
		T Spawn<T>();
		
		/// <inheritdoc cref="IObjectPool{T}.Despawn"/>
		void Despawn<T>(T entity);

		/// <inheritdoc cref="IObjectPool{T}.DespawnAll"/>
		void DespawnAll<T>();

		/// <inheritdoc cref="IObjectPool{T}.Clear"/>
		void Clear<T>();
	}
	
	/// <inheritdoc />
	public class PoolService : IPoolService
	{
		private readonly Dictionary<Type, IObjectPool> _pools = new Dictionary<Type, IObjectPool>();
		
		/// <inheritdoc />
		public void InitPool<T>(int initialSize, Func<T> instantiator) where T : new()
		{
			_pools.Add(typeof(T), new ObjectPool<T>(initialSize, instantiator));
		}

		/// <inheritdoc />
		public void InitPool<T>(int initialSize, T sampleEntity, Func<T, T> instantiator) where T : Object
		{
			_pools.Add(typeof(T), new GameObjectPool<T>(initialSize, sampleEntity, instantiator));
		}

		/// <inheritdoc />
		public bool HasPool<T>()
		{
			return HasPool(typeof(T));
		}

		/// <inheritdoc />
		public bool HasPool(Type type)
		{
			return _pools.ContainsKey(type);
		}

		/// <inheritdoc />
		public T Spawn<T>()
		{
			return GetPool<T>().Spawn();
		}

		/// <inheritdoc />
		public void Despawn<T>(T entity)
		{
			GetPool<T>().Despawn(entity);
		}

		/// <inheritdoc />
		public void DespawnAll<T>()
		{
			GetPool<T>().DespawnAll();
		}

		/// <inheritdoc />
		public void Clear<T>()
		{
			GetPool<T>().Clear();
			_pools.Remove(typeof(T));
		}

		private IObjectPool<T> GetPool<T>()
		{
			if (!_pools.TryGetValue(typeof(T), out IObjectPool pool))
			{
				throw new ArgumentException("The pool was not initialized for the type " + typeof(T));
			}

			return pool as IObjectPool<T>;
		}
	}
	
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
	/// This interface allows pooled objects to be notified when they are cleared from the pool
	/// </summary>
	public interface IPoolEntityCleared
	{
		/// <summary>
		/// Invoked when the entity is cleared
		/// </summary>
		void OnCleared();
	}

	/// <summary>
	/// Simple object pool implementation that can handle any type of entity objects
	/// </summary>
	public interface IObjectPool
	{
		/// <summary>
		/// Clears the pool
		/// This function does not clear the entity. For that, have the entity implement <see cref="IPoolEntityCleared"/> or do it externally
		/// </summary>
		void Clear();
		
		/// <summary>
		/// Despawns all active spawned entities and returns them back to the pool to be used again later
		/// This function does not reset the entity. For that, have the entity implement <see cref="IPoolEntityDespawn"/> or do it externally
		/// </summary>
		void DespawnAll();
	}
	
	/// <inheritdoc />
	public interface IObjectPool<T> : IObjectPool
	{
		/// <summary>
		/// Spawns and returns an entity of the given type <typeparamref name="T"/>
		/// This function does not initialize the entity. For that, have the entity implement <see cref="IPoolEntitySpawn"/> or do it externally
		/// This function throws a <exception cref="StackOverflowException" /> if the pool is empty
		/// </summary>
		T Spawn();
		
		/// <summary>
		/// Despawns the given <paramref name="entity"/> and returns it back to the pool to be used again later
		/// This function does not reset the entity. For that, have the entity implement <see cref="IPoolEntityDespawn"/> or do it externally
		/// </summary>
		void Despawn(T entity);
	}

	/// <inheritdoc />
	public abstract class ObjectPoolBase<T> : IObjectPool<T>
	{
		private readonly Stack<T> _stack = new Stack<T>();
		private readonly IList<T> _spawnedEntities = new List<T>();
		private readonly Func<T, T> _instantiator;
		private readonly T _sampleEntity;
		
		protected ObjectPoolBase(int initSize, T sampleEntity, Func<T, T> instantiator)
		{
			_sampleEntity = sampleEntity;
			_instantiator = instantiator;
			
			for (var i = 0; i < initSize; i++)
			{
				_stack.Push(instantiator.Invoke(sampleEntity));
			}
		}
		
		/// <inheritdoc />
		public void Clear()
		{
			for (var i = 0; i < _stack.Count; i++)
			{
				var entity =_stack.Pop() as IPoolEntityCleared;
				
				entity?.OnCleared();
			}
			
			_spawnedEntities.Clear();
		}

		/// <inheritdoc />
		public T Spawn()
		{
			var entity = _stack.Count == 0 ? _instantiator.Invoke(_sampleEntity) : _stack.Pop();
			var poolEntity = entity as IPoolEntitySpawn;
			
			_spawnedEntities.Add(entity);
			poolEntity?.OnSpawn();

			return entity;
		}

		/// <inheritdoc />
		public void Despawn(T entity)
		{
			var poolEntity = entity as IPoolEntityDespawn;

			_stack.Push(entity);
			_spawnedEntities.Remove(entity);
			poolEntity?.OnDespawn();
		}

		/// <inheritdoc />
		public void DespawnAll()
		{
			for (var i = 0; i < _spawnedEntities.Count; i++)
			{
				Despawn(_spawnedEntities[i]);
			}

			_spawnedEntities.Clear();
		}
	}

	/// <inheritdoc />
	public class ObjectPool<T> : ObjectPoolBase<T> where T : new()
	{
		public ObjectPool(int initSize, Func<T> instantiator) : base(initSize, instantiator(), newEntity => instantiator.Invoke())
		{
		}
	}

	/// <inheritdoc />
	/// <remarks>
	/// <see cref="IObjectPool"/> implementation for objects of type <see cref="Object"/>
	/// </remarks>
	public class GameObjectPool<T> : ObjectPoolBase<T> where T : Object
	{
		public GameObjectPool(int initSize, T sampleEntity, Func<T, T> instantiator) : base(initSize, sampleEntity, instantiator)
		{
		}
	}
}