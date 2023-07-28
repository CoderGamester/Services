using System;
using System.Collections.Generic;

// ReSharper disable once CheckNamespace

namespace GameLovers.Services
{
	/// <summary>
	/// This marks the defined object to me a container of binding installers.
	/// It acts as a locator for all the binded interfaces.
	/// It only allows to bind interfaces.
	/// </summary>
	/// <remarks>
	/// Follows the "Inversion of Control" principle <see cref="https://en.wikipedia.org/wiki/Inversion_of_control"/>
	/// </remarks>
	public interface IInstaller
	{
		/// <summary>
		/// Binds the interface <typeparamref name="T"/> to the given <paramref name="instance"/>
		/// </summary>
		/// <exception cref="ArgumentException">
		/// Thrown if the given <paramref name="instance"/> doesn't implement <typeparamref name="T"/> interface
		/// </exception>
		void Bind<T>(T instance) where T : class;

		/// <summary>
		/// Tries requesting the instance binded to the type <typeparamref name="T"/>
		/// Returns true if the instance is not binded yet 
		/// </summary>
		bool TryResolve<T>(out T instance);

		/// <summary>
		/// Requests the instance binded to the type <typeparamref name="T"/>
		/// </summary>
		/// <exception cref="ArgumentException">
		/// Thrown if the given <typeparamref name="T"/> type was not yet binded
		/// </exception>
		T Resolve<T>();

		/// <summary>
		/// Cleans the binding of the given type <typeparamref name="T"/> from the installer
		/// Useful in case of resetting the game state.
		/// Returns TRUE if successfully cleaned the given type <typeparamref name="T"/>, FALSE otherwise
		/// </summary>
		bool Clean<T>() where T : class;

		/// <summary>
		/// Cleans all the bindings of the installer
		/// Useful in case of resetting the game state
		/// </summary>
		void Clean();
	}
	
	
	/// <inheritdoc />
	public class Installer : IInstaller
	{
		private readonly Dictionary<Type, object> _bindings = new Dictionary<Type, object>();

		/// <inheritdoc />
		public void Bind<T>(T instance) where T : class
		{
			var type = typeof(T);

			if (!type.IsInterface)
			{
				throw new ArgumentException($"Cannot bind {instance} because {type} is not an interface");
			}

			_bindings.Add(type, instance);
		}

		/// <inheritdoc />
		public bool TryResolve<T>(out T instance)
		{
			var ret = _bindings.TryGetValue(typeof(T), out object inst);

			instance = (T)inst;

			return ret;
		}

		/// <inheritdoc />
		public T Resolve<T>()
		{
			if (!_bindings.TryGetValue(typeof(T), out object instance))
			{
				throw new ArgumentException($"The type {typeof(T)} is not binded");
			}

			return (T) instance;
		}

		/// <inheritdoc />
		public bool Clean<T>() where T : class
		{
			return _bindings.Remove(typeof(T));
		}

		/// <inheritdoc />
		public void Clean()
		{
			_bindings.Clear();
		}
	}
}