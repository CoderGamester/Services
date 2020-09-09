using System.Collections.Generic;
using GameLovers.Services;
using NSubstitute;
using NUnit.Framework;

// ReSharper disable once CheckNamespace

namespace GameLoversEditor.Services.Tests
{
	[TestFixture]
	public class NetworkServiceTest
	{
		private NetworkService _networkService;
		private ICallMockup _callMockup;

		private interface IGameLogicMockup { }
		private struct CommandMockup : IGameCommand<IGameLogicMockup>
		{
			public int Payload;
			
			public void Execute(IGameLogicMockup gameLogic) { }
		}

		// ReSharper disable once MemberCanBePrivate.Global
		public interface ICallMockup
		{
			void SendMessageRequestMockCall(string name, IDictionary<string, object> payload);
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
			_callMockup = Substitute.For<ICallMockup>();
			_networkService = new GameNetworkService(_callMockup);
		}

		[Test]
		public void SendCommand_Successfully()
		{
			var payload = 1;
			
			_networkService.SendCommand(new CommandMockup { Payload = payload });
			
			_callMockup.Received().SendMessageRequestMockCall(Arg.Is(nameof(CommandMockup)), 
			                                                  Arg.Is<IDictionary<string, object>>(dic => (int) dic[nameof(CommandMockup.Payload)] == payload));
		}
	}
}