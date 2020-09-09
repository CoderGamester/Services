using System.Collections.Generic;

// ReSharper disable once CheckNamespace

namespace GameLovers.Services
{
	/// <summary>
	/// This interface provides the contract to define the communication layer between the game and the game server running online.
	/// This interface should be implemented based on the project needs.
	/// </summary>
	public interface INetworkLayer
	{
		/// <summary>
		/// Sends a message through the network with the given <paramref name="name"/> and given p<paramref name="payload"/>
		/// </summary>
		void SendMessageRequest(string name, IDictionary<string, object> payload);
	}
	
	/// <summary>
	/// This service provides the possibility to process any network code or to relay backend logic code to a game server
	/// running online.
	/// It gives the possibility to have the desired behaviour for a game to run online.
	/// </summary>
	public interface INetworkService
	{
	}

	/// <summary>
	/// This interface provides the contract to relay <see cref="IGameCommandBase"/> to the game server
	/// </summary>
	public interface ICommandNetworkService
	{
		/// <summary>
		/// Sends the given <paramref name="command"/> to be executed in the server.
		/// The command execution is done atomically
		/// </summary>
		void SendCommand<TCommand>(TCommand command) where TCommand : struct, IGameCommandBase;
	}
	
	/// <inheritdoc cref="INetworkService"/>
	public class NetworkService : INetworkService, ICommandNetworkService
	{
		private readonly INetworkLayer _networkLayer;
		
		private NetworkService() {}
		
		public NetworkService(INetworkLayer networkLayer)
		{
			_networkLayer = networkLayer;
		}
		
		/// <inheritdoc />
		public void SendCommand<TCommand>(TCommand command) where TCommand : struct, IGameCommandBase
		{
			var type = typeof(TCommand);
			var obj = command as object;
			var dictionary = new Dictionary<string, object>();
			var fields = type.GetFields();
			var properties = type.GetProperties();

			foreach (var fieldInfo in fields)
			{
				dictionary.Add(fieldInfo.Name, fieldInfo.GetValue(obj));
			}

			foreach (var propertyInfo in properties)
			{
				dictionary.Add(propertyInfo.Name, propertyInfo.GetValue(obj));
			}
			
			_networkLayer.SendMessageRequest(type.Name, dictionary);
		}
	}
}