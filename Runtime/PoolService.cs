using System;
using System.Collections.Generic;

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
		/// Adds the given <paramref name="pool"/> of <typeparamref name="T"/> to the service
		/// </summary>
		/// <exception cref="ArgumentException">
		/// Thrown if the service already has a pool of the given <typeparamref name="T"/> type
		/// </exception>
		void AddPool<T>(IObjectPool<T> pool) where T : class;

		/// <summary>
		/// Removes the pool of the given <typeparamref name="T"/>
		/// </summary>
		void RemovePool<T>() where T : class;

		/// <summary>
		/// Checks if exists a pool of the given type already exists or needs to be added before calling <seealso cref="Spawn{T}"/>
		/// </summary>
		bool HasPool<T>() where T : class;

		/// <inheritdoc cref="HasPool{T}"/>
		bool HasPool(Type type);

		/// <inheritdoc cref="IObjectPool{T}.IsSpawned"/>
		bool IsSpawned<T>(Func<T, bool> conditionCheck) where T : class;
		
		/// <inheritdoc cref="IObjectPool{T}.Spawn"/>
		/// <exception cref="ArgumentException">
		/// Thrown if the service does not contains a pool of the given <typeparamref name="T"/> type
		/// </exception>
		T Spawn<T>() where T : class;
		
		/// <inheritdoc cref="IObjectPool{T}.Despawn"/>
		/// <exception cref="ArgumentException">
		/// Thrown if the service does not contains a pool of the given <typeparamref name="T"/> type
		/// </exception>
		bool Despawn<T>(T entity) where T : class;

		/// <inheritdoc cref="IObjectPool{T}.DespawnAll"/>
		/// <exception cref="ArgumentException">
		/// Thrown if the service does not contains a pool of the given <typeparamref name="T"/> type
		/// </exception>
		void DespawnAll<T>() where T : class;
		
		/// <summary>
		/// Clears the contents out of this service.
		/// Returns back all pools so they can be independently disposed
		/// </summary>
		IDictionary<Type, IObjectPool> Clear();
	}
	
	/// <inheritdoc />
	public class PoolService : IPoolService
	{
		private readonly IDictionary<Type, IObjectPool> _pools = new Dictionary<Type, IObjectPool>();

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
		public bool HasPool<T>() where T : class
		{
			return HasPool(typeof(T));
		}

		/// <inheritdoc />
		public bool HasPool(Type type)
		{
			return _pools.ContainsKey(type);
		}

		/// <inheritdoc />
		public bool IsSpawned<T>(Func<T, bool> conditionCheck) where T : class
		{
			return GetPool<T>().IsSpawned(conditionCheck);
		}

		/// <inheritdoc />
		public T Spawn<T>() where T : class
		{
			return GetPool<T>().Spawn();
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
		public void Dispose()
		{
			foreach (var pool in _pools)
			{
				pool.Value.Dispose();
			}
			
			_pools.Clear();
		}

		private IObjectPool<T> GetPool<T>() where T : class
		{
			if (!_pools.TryGetValue(typeof(T), out var pool))
			{
				throw new ArgumentException("The pool was not initialized for the type " + typeof(T));
			}

			return pool as IObjectPool<T>;
		}
	}
}