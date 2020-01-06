using System;
using System.Collections.Generic;

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
	/// Simple pool implementation that can handle any type of entity objects
	/// </summary>
	public interface IPoolService
	{
		/// <summary>
		/// Initializes a new pool with the given <paramref name="initialSize"/>
		/// It invokes the <paramref name="instantiator"/> function every time a new entity is created in the pool
		/// </summary>
		void InitPool<T>(int initialSize, Func<T> instantiator);
		
		/// <summary>
		/// Initializes a new pool with the given <paramref name="initialSize"/> and a sample entity given back in the <paramref name="instantiator"/>
		/// It invokes the <paramref name="instantiator"/> function every time a new entity is created in the pool
		/// </summary>
		void InitPool<T>(int initialSize, T sampleEntity, Func<T, T> instantiator);

		/// <summary>
		/// Checks if exists a pool of the given type already exists or needs to be initialized with
		/// <seealso cref="InitPool{T}(int,System.Func{T})"/>  before calling <seealso cref="Spawn{T}"/>
		/// </summary>
		bool HasPool<T>();

		/// <inheritdoc cref="HasPool{T}"/>
		bool HasPool(Type type);
		
		/// <summary>
		/// Spawns and returns an entity of the given type <typeparamref name="T"/>
		/// This function does not initialize the entity. For that, have the entity implement <see cref="IPoolEntitySpawn"/> or do it externally
		/// This function throws a <exception cref="StackOverflowException" /> if the pool is empty
		/// </summary>
		T Spawn<T>();
		
		/// <summary>
		/// Despawns the given <paramref name="entity"/> and returns it back to the pool to be used again later
		/// This function does not reset the entity. For that, have the entity implement <see cref="IPoolEntityDespawn"/> or do it externally
		/// </summary>
		void Despawn<T>(T entity);

		/// <summary>
		/// Despawns all active spawned entities of the given type <typeparamref name="T"/> and returns them back to the pool to be used again later
		/// This function does not reset the entity. For that, have the entity implement <see cref="IPoolEntityDespawn"/> or do it externally
		/// </summary>
		void DespawnAll<T>();

		/// <summary>
		/// Clears the pool of the given type <typeparamref name="T"/>
		/// It calls <paramref name="clearAction"/> with every entity remaining in the pool and managed by the pool
		/// </summary>
		void Clear<T>(Action<T> clearAction);
	}
	
	/// <inheritdoc />
	public class PoolService : IPoolService
	{
		private readonly Dictionary<Type, IPoolStack> pools = new Dictionary<Type, IPoolStack>();
		
		/// <inheritdoc />
		public void InitPool<T>(int initialSize, Func<T> instantiator)
		{
			InitPool(initialSize, instantiator.Invoke(), newEntity => instantiator.Invoke());
		}

		/// <inheritdoc />
		public void InitPool<T>(int initialSize, T sampleEntity, Func<T, T> instantiator)
		{
			if (pools.ContainsKey(typeof(T)))
			{
				throw new InvalidOperationException($"The pool of type {typeof(T)} was already initialized");
			}
			
			var pool = new PoolStack<T>
			{
				Stack = new Stack<T>(), 
				SpawnedEntities = new List<T>(),
				SampleEntity = sampleEntity, 
				Instatiator = instantiator
			};
			
			pools.Add(typeof(T), pool);
			
			for (int i = 0; i < initialSize; i++)
			{
				pool.Stack.Push(instantiator.Invoke(sampleEntity));
			}
		}

		/// <inheritdoc />
		public bool HasPool<T>()
		{
			return HasPool(typeof(T));
		}

		/// <inheritdoc />
		public bool HasPool(Type type)
		{
			return pools.ContainsKey(type);
		}

		/// <inheritdoc />
		public T Spawn<T>()
		{
			var pool = GetPool<T>();
			T entity = pool.Stack.Count == 0 ? pool.Instatiator.Invoke(pool.SampleEntity) : pool.Stack.Pop();
			var poolEntity = entity as IPoolEntitySpawn;
			
			pool.SpawnedEntities.Add(entity);
			poolEntity?.OnSpawn();
			
			return entity;
		}

		/// <inheritdoc />
		public void Despawn<T>(T entity)
		{
			var pool = GetPool<T>();
			var poolEntity = entity as IPoolEntityDespawn;

			pool.Stack.Push(entity);
			pool.SpawnedEntities.Remove(entity);
			poolEntity?.OnDespawn();
		}

		/// <inheritdoc />
		public void DespawnAll<T>()
		{
			var pool = GetPool<T>();
			
			foreach (T entity in pool.SpawnedEntities)
			{
				var poolEntity = entity as IPoolEntityDespawn;

				pool.Stack.Push(entity);
				poolEntity?.OnDespawn();
			}
			
			pool.SpawnedEntities.Clear();
		}

		/// <inheritdoc />
		public void Clear<T>(Action<T> clearAction)
		{
			var pool = GetPool<T>();

			for (var i = 0; i < pool.Stack.Count; i++)
			{
				T entity = pool.Stack.Pop();
				
				clearAction?.Invoke(entity);
			}
			
			pool.SpawnedEntities.Clear();
			pools.Remove(typeof(T));
		}

		private PoolStack<T> GetPool<T>()
		{
			if (!pools.TryGetValue(typeof(T), out IPoolStack poolStack))
			{
				throw new ArgumentException("The pool was not initialized for the type " + typeof(T));
			}

			if (poolStack is PoolStack<T> pool)
			{
				return pool;
			}
			
			throw new ArgumentException("The pool was not properly initialized for the type " + typeof(T));
		}
		
		private interface IPoolStack {}

		private class PoolStack<T> : IPoolStack
		{
			public Stack<T> Stack;
			public List<T> SpawnedEntities;
			public Func<T, T> Instatiator;
			public T SampleEntity;
		}
	}
}