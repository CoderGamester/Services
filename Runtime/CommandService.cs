// ReSharper disable once CheckNamespace

namespace GameLovers.Services
{
	/// <summary>
	/// Interface representing the command to be executed in the <see cref="ICommandService{T}"/>
	/// Implement this interface with the proper command logic
	/// </summary>
	/// <remarks>
	/// Follows the Command pattern <see cref="https://en.wikipedia.org/wiki/Command_pattern"/>
	/// </remarks>
	public interface IGameCommand<in T> where T : class
	{
		/// <summary>
		/// Executes the command logic
		/// </summary>
		void Execute(T gameLogic);
	}
	
	/// <summary>
	/// This service provides the possibility to execute a <see cref="IGameCommand{T}"/>
	/// </summary>
	public interface ICommandService<out T> where T : class
	{
		/// <summary>
		/// Executes the <paramref name="command"/>
		/// The command execution is done atomically
		/// </summary>
		void ExecuteCommand<TCommand>(TCommand command) where TCommand : struct, IGameCommand<T>;
	}
	
	/// <inheritdoc />
	public class CommandService<T> : ICommandService<T> where T : class
	{
		private readonly T _gameLogic;
		
		public CommandService(T gameLogic)
		{
			_gameLogic = gameLogic;
		}
		
		/// <inheritdoc />
		public void ExecuteCommand<TCommand>(TCommand command) where TCommand : struct, IGameCommand<T>
		{
			command.Execute(_gameLogic);
		}
	}
}