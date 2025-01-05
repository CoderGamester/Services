using System;
using System.Collections.Generic;
using UnityEngine;

// ReSharper disable once CheckNamespace

namespace GameLovers.Services
{
	/// <summary>
	/// This service allows to manage multiple pools of different types.
	/// The service can only a single pool of the same type. 
	/// </summary>
	public interface IPoolService : IDisposable
	{
		/// <summary>
		/// Retrieves the pool of objects of type <typeparamref name="T"/>.
		/// If the pool does not exist, an <see cref="ArgumentException"/> is thrown.
		/// </summary>
		/// <typeparam name="T">The type of objects in the pool.</typeparam>
		/// <returns>The pool of objects.</returns>
		/// <exception cref="ArgumentException">Thrown if the pool does not exist.</exception>
		IObjectPool<T> GetPool<T>() where T : class;

		/// <summary>
		/// Tries to retrieve the pool of objects of type <typeparamref name="T"/>.
		/// </summary>
		/// <typeparam name="T">The type of objects in the pool.</typeparam>
		/// <param name="pool">The pool of objects, or null if it does not exist.</param>
		/// <returns>True if the pool exists, false otherwise.</returns>
		bool TryGetPool<T>(out IObjectPool<T> pool) where T : class;

		/// <summary>
		/// Adds a new pool of objects of type <typeparamref name="T"/> to the service.
		/// If a pool of the same type already exists, an <see cref="ArgumentException"/> is thrown.
		/// </summary>
		/// <typeparam name="T">The type of objects in the pool.</typeparam>
		/// <param name="pool">The pool of objects to add.</param>
		/// <exception cref="ArgumentException">Thrown if a pool of the same type already exists.</exception>
		void AddPool<T>(IObjectPool<T> pool) where T : class;

		/// <summary>
		/// Removes the pool of objects of type <typeparamref name="T"/> from the service.
		/// </summary>
		/// <typeparam name="T">The type of objects in the pool.</typeparam>
		void RemovePool<T>() where T : class;

		/// <inheritdoc cref="IObjectPool{T}.Spawn"/>
		T Spawn<T>() where T : class;

		/// <inheritdoc cref="IObjectPool{T}.Spawn{TData}"/>
		T Spawn<T, TData>(TData data) where T : class, IPoolEntitySpawn<TData>;

		/// <inheritdoc cref="IObjectPool{T}.Despawn(T)"/>
		bool Despawn<T>(T entity) where T : class;

		/// <summary>
		/// Despawns all entities from the pool of type <typeparamref name="T"/>.
		/// </summary>
		/// <typeparam name="T">The type of the entities to be despawned.</typeparam>
		/// <exception cref="ArgumentException">
		/// Thrown if the service does not contain a pool of the given <typeparamref name="T"/> type.
		/// </exception>
		void DespawnAll<T>() where T : class;

		/// <summary>
		/// Clears the contents out of this service.
		/// Returns back all pools so they can be independently disposed.
		/// </summary>
		/// <returns>
		/// A dictionary containing all the pools in this service, where the key is the type of the pool and the value is the pool itself.
		/// </returns>
		IDictionary<Type, IObjectPool> Clear();

		/// <inheritdoc cref="IObjectPool{T}.Dispose(bool)"/>
		void Dispose<T>(bool disposeSampleEntity) where T : class;
	}

	/// <inheritdoc />
	public class PoolService : IPoolService
	{
		private readonly IDictionary<Type, IObjectPool> _pools = new Dictionary<Type, IObjectPool>();

		/// <inheritdoc />
		public IObjectPool<T> GetPool<T>() where T : class
		{
			if (!TryGetPool<T>(out var pool))
			{
				throw new ArgumentException("The pool was not initialized for the type " + typeof(T));
			}

			return pool;
		}

		/// <inheritdoc />
		public bool TryGetPool<T>(out IObjectPool<T> pool) where T : class
		{
			var ret = _pools.TryGetValue(typeof(T), out var innerPool);

			pool = innerPool as IObjectPool<T>;

			return ret;
		}

		/// <inheritdoc />
		public void AddPool<T>(IObjectPool<T> pool) where T : class
		{
			_pools.Add(typeof(T), pool);
		}

		/// <inheritdoc />
		public void RemovePool<T>() where T : class
		{
			_pools.Remove(typeof(T));
		}

		/// <inheritdoc />
		public T Spawn<T>() where T : class
		{
			return GetPool<T>().Spawn();
		}

		/// <inheritdoc />
		public T Spawn<T, TData>(TData data) where T : class, IPoolEntitySpawn<TData>
		{
			return GetPool<T>().Spawn(data);
		}

		/// <inheritdoc />
		public bool Despawn<T>(T entity) where T : class
		{
			return GetPool<T>().Despawn(entity);
		}

		/// <inheritdoc />
		public void DespawnAll<T>() where T : class
		{
			GetPool<T>().DespawnAll();
		}

		/// <inheritdoc />
		public IDictionary<Type, IObjectPool> Clear()
		{
			var ret = new Dictionary<Type, IObjectPool>(_pools);

			_pools.Clear();

			return ret;
		}

		/// <inheritdoc />
		public void Dispose<T>(bool disposeSampleEntity) where T : class
		{
			GetPool<T>().Dispose(disposeSampleEntity);
			RemovePool<T>();
		}

		/// <inheritdoc />
		public void Dispose()
		{
			foreach (var pool in _pools)
			{
				pool.Value.Dispose();
			}

			_pools.Clear();
		}
	}
}