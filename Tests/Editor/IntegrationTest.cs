using System.Collections.Generic;
using GameLovers.Services;
using NSubstitute;
using NUnit.Framework;

// ReSharper disable once CheckNamespace

namespace GameLoversEditor.Services.Tests
{
	[TestFixture]
	public class IntegrationTest
	{
		private CommandService<IGameLogicMockup> _commandService;
		private NetworkService _networkService;
		private IGameLogicMockup _gameLogicMockup;
		private ICallMockup _callMockup;

		// ReSharper disable MemberCanBePrivate.Global
		public interface IGameLogicMockup
		{
			void CallMockup(int payload);
		}
		public interface ICallMockup
		{
			void SendMessageRequestMockCall(string name, IDictionary<string, object> payload);
		}

		private struct CommandMockup : IGameCommand<IGameLogicMockup>
		{
			public int Payload;
			
			public void Execute(IGameLogicMockup gameLogic)
			{
				gameLogic.CallMockup(Payload);
			}
		}

		private class GameNetworkService : NetworkService
		{
			private readonly ICallMockup _callMockup;

			public GameNetworkService(ICallMockup callMockup)
			{
				_callMockup = callMockup;
			}	
			
			protected override void SendMessageRequest(string name, IDictionary<string, object> payload)
			{
				_callMockup.SendMessageRequestMockCall(name, payload);
			}
		}

		[SetUp]
		public void Init()
		{
			_gameLogicMockup = Substitute.For<IGameLogicMockup>();
			_callMockup = Substitute.For<ICallMockup>();
			_networkService = new GameNetworkService(_callMockup);
			_commandService = new CommandService<IGameLogicMockup>(_gameLogicMockup, _networkService);
		}

		[Test]
		public void ExecuteCommand_Successfully()
		{
			var payload = 1;
			
			_commandService.ExecuteCommand(new CommandMockup { Payload = payload });
			
			_gameLogicMockup.Received().CallMockup(Arg.Is(payload));
			_callMockup.Received().SendMessageRequestMockCall(Arg.Is(nameof(CommandMockup)), 
			                                                  Arg.Is<IDictionary<string, object>>(dic => (int) dic[nameof(CommandMockup.Payload)] == payload));
		}
	}
}