using System;
using System.Collections.Generic;

// ReSharper disable once CheckNamespace

namespace GameLovers.Services
{
	/// <summary>
	/// Main injector to use in the game
	/// It only allows to bind interfaces
	/// </summary>
	/// <remarks>
	/// Follows the "Inversion of Control" principle <see cref="https://en.wikipedia.org/wiki/Inversion_of_control"/>
	/// </remarks>
	public static class MainInstaller
	{
		private static readonly Dictionary<Type, object> _bindings = new Dictionary<Type, object>();

		/// <summary>
		/// Binds the interface <typeparamref name="T"/> to the given <paramref name="instance"/>
		/// </summary>
		/// <exception cref="ArgumentException">
		/// Thrown if the given <paramref name="instance"/> doesn't implement <typeparamref name="T"/> interface
		/// </exception>
		public static void Bind<T>(T instance) where T : class
		{
			var type = typeof(T);

			if (!type.IsInterface)
			{
				throw new ArgumentException($"Cannot bind {instance} because {type} is not an interface");
			}

			_bindings.Add(type, instance);
		}

		/// <summary>
		/// Requests the instance binded to the type <typeparamref name="T"/>
		/// </summary>
		/// <exception cref="ArgumentException">
		/// Thrown if the given <typeparamref name="T"/> type was not yet binded
		/// </exception>
		public static T Resolve<T>()
		{
			if (!_bindings.TryGetValue(typeof(T), out object instance))
			{
				throw new ArgumentException($"The type {typeof(T)} is not binded");
			}

			return (T) instance;
		}

		/// <summary>
		/// Cleans all the bindings of the installer
		/// Useful in case of resetting the game state
		/// </summary>
		public static void Clean()
		{
			_bindings.Clear();
		}
	}
}