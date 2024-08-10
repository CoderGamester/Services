using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

// ReSharper disable once CheckNamespace

namespace GameLovers.Services
{
	/// <summary>
	/// This interface provides the access to the player's save persistent data 
	/// </summary>
	public interface IDataProvider
	{
		/// <summary>
		/// Requests the player's data of <typeparamref name="T"/> type
		/// </summary>
		T GetData<T>() where T : class;

		/// <summary>
		/// Requests if the service has the given player's data of <typeparamref name="T"/> type
		/// </summary>
		bool HasData<T>() where T : class;
	}

	/// <summary>
	/// This service allows to manage all the persistent data in the game.
	/// Data are strictly reference types to guarantee that there is no boxing/unboxing and lost of referencing when changing it's data. 
	/// </summary>
	public interface IDataService : IDataProvider
	{
		/// <summary>
		/// Saves the game's given <typeparamref name="T"/> data to disk
		/// </summary>
		void SaveData<T>() where T : class;

		/// <summary>
		/// Saves all game's data to disk
		/// </summary>
		void SaveAllData();

		/// <summary>
		/// Loads the game's given <typeparamref name="T"/> data from disk and returns it
		/// </summary>
		T LoadData<T>() where T : class;

		/// <summary>
		/// Adds or replaces the given <paramref name="data"/> to be maintained in memory.
		/// </summary>
		void AddOrReplaceData<T>(T data) where T : class;
	}

	/// <inheritdoc />
	public class DataService : IDataService
	{
		private readonly IDictionary<Type, object> _data = new Dictionary<Type, object>();

		/// <inheritdoc />
		public bool HasData<T>() where T : class
		{
			return _data.ContainsKey(typeof(T));
		}

		/// <inheritdoc />
		public T GetData<T>() where T : class
		{
			return _data[typeof(T)] as T;
		}

		/// <inheritdoc />
		public void SaveData<T>() where T : class
		{
			var type = typeof(T);

			PlayerPrefs.SetString(type.Name, JsonConvert.SerializeObject(_data[type]));
			PlayerPrefs.Save();
			OnDataSaved(type.Name, _data[type], type);
		}

		/// <inheritdoc />
		public void SaveAllData()
		{
			foreach (var data in _data)
			{
				PlayerPrefs.SetString(data.Key.Name, JsonConvert.SerializeObject(data.Value));
				OnDataSaved(data.Key.Name, data.Value, data.Key);
			}
			
			PlayerPrefs.Save();
		}

		/// <inheritdoc />
		public T LoadData<T>() where T : class
		{
			var json = PlayerPrefs.GetString(typeof(T).Name, "");
			var instance = string.IsNullOrEmpty(json) ? Activator.CreateInstance<T>() : JsonConvert.DeserializeObject<T>(json);

			AddOrReplaceData(instance);

			return instance;
		}

		/// <inheritdoc />
		public void AddOrReplaceData<T>(T data) where T : class
		{
			if(HasData<T>())
			{
				_data[typeof(T)] = data;
			}
			else
			{
				_data.Add(typeof(T), data);
			}
		}

		protected virtual void OnDataSaved(string key, object data, Type type)
		{
		}
	}
}