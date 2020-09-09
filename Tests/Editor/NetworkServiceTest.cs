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
			_networkLayerMockup = Substitute.For<INetworkLayer>();
			_networkService = new NetworkService(_networkLayerMockup);
		}

		[Test]
		public void SendCommand_Successfully()
		{
			var payload = 1;
			
			_networkService.SendCommand(new CommandMockup { Payload = payload });

			_networkLayerMockup.Received()
			                   .SendMessageRequest(Arg.Is(nameof(CommandMockup)), 
			                                       Arg.Is<IDictionary<string, object>>(dic => (int) dic[nameof(CommandMockup.Payload)] == payload));
		}
	}
}