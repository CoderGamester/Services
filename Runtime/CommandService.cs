// ReSharper disable once CheckNamespace

namespace GameLovers.Services
{
	/// <summary>
	/// Tags the interface as a <see cref="IGameCommand{TGameLogic}"/>
	/// </summary>
	public interface IGameCommandBase {}

	/// <summary>
	/// Contract for the command to be executed in the <see cref="ICommandService{TGameLogic}"/>.
	/// Implement this interface if you want logic to be executed ont he server
	/// </summary>
	/// <remarks>
	/// Follows the Command pattern <see cref="https://en.wikipedia.org/wiki/Command_pattern"/>
	/// </remarks>
	public interface IGameServerCommand<in TGameLogic> : IGameCommandBase where TGameLogic : class
	{
		/// <summary>
		/// Executes the command logic defined by the implemention of this interface
		/// </summary>
		void ExecuteLogic(TGameLogic gameLogic);
	}

	/// <summary>
	/// Interface representing the command to be executed in the <see cref="ICommandService{TGameLogic}"/>.
	/// Implement this interface with the proper command logic
	/// </summary>
	/// <remarks>
	/// Follows the Command pattern <see cref="https://en.wikipedia.org/wiki/Command_pattern"/>
	/// </remarks>
	public interface IGameCommand<in TGameLogic> : IGameCommandBase where TGameLogic : class
	{
		/// <summary>
		/// Executes the command logic defined by the implemention of this interface
		/// </summary>
		void Execute(TGameLogic gameLogic, IMessageBrokerService messageBroker);
	}
	
	/// <summary>
	/// This service provides the possibility to execute a <see cref="IGameCommand{TGameLogic}"/>.
	/// It allows to create an seamless abstraction layer of execution between the game logic and any other part of the code 
	/// </summary>
	public interface ICommandService<out TGameLogic> where TGameLogic : class
	{
		/// <summary>
		/// Executes the given <paramref name="command"/>
		/// </summary>
		/// <remarks>
		/// IMPORTANT: Defines the <paramref name="command"/> as a class object if logic execution is asynchronous.
		/// Define as a struct if togic logic execution is non waitable.
		/// </remarks>
		void ExecuteCommand<TCommand>(TCommand command) where TCommand : IGameCommand<TGameLogic>;
	}
	
	/// <inheritdoc />
	public class CommandService<TGameLogic> : ICommandService<TGameLogic> where TGameLogic : class
	{
		private readonly TGameLogic _gameLogic;
		private readonly IMessageBrokerService _messageBroker;

		public CommandService(TGameLogic gameLogic, IMessageBrokerService messageBroker)
		{
			_gameLogic = gameLogic;
			_messageBroker = messageBroker;
		}

		/// <inheritdoc />
		public void ExecuteCommand<TCommand>(TCommand command) where TCommand : IGameCommand<TGameLogic>
		{
			command.Execute(_gameLogic, _messageBroker);
		}
	}
}