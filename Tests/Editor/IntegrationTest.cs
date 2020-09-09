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
		private INetworkLayer _networkLayerMockup;

		// ReSharper disable once MemberCanBePrivate.Global
		public interface IGameLogicMockup
		{
			void CallMockup(int payload);
		}

		private struct CommandMockup : IGameCommand<IGameLogicMockup>
		{
			public int Payload;
			
			public void Execute(IGameLogicMockup gameLogic)
			{
				gameLogic.CallMockup(Payload);
			}
		}

		[SetUp]
		public void Init()
		{
			_gameLogicMockup = Substitute.For<IGameLogicMockup>();
			_networkLayerMockup = Substitute.For<INetworkLayer>();
			_networkService = new NetworkService(_networkLayerMockup);
			_commandService = new CommandService<IGameLogicMockup>(_gameLogicMockup, _networkService);
		}

		[Test]
		public void ExecuteCommand_Successfully()
		{
			var payload = 1;
			var command = new CommandMockup { Payload = payload };
			
			_commandService.ExecuteCommand(command);
			
			_gameLogicMockup.Received().CallMockup(Arg.Is(payload));
			_networkLayerMockup.Received()
			                   .SendMessageRequest(Arg.Is(nameof(CommandMockup)), 
			                                       Arg.Is<IDictionary<string, object>>(dic => (int) dic[nameof(CommandMockup.Payload)] == payload));
		}
	}
}