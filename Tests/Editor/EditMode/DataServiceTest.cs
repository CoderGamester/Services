using System.Collections.Generic;
using GameLovers.Services;
using NSubstitute;
using NUnit.Framework;

// ReSharper disable once CheckNamespace

namespace GameLoversEditor.Services.Tests
{
	[TestFixture]
	public class DataServiceTest
	{
		private DataService _dataService;

		// ReSharper disable once MemberCanBePrivate.Global
		public interface IDataMockup {}

		[SetUp]
		public void Init()
		{
			_dataService = new DataService();
		}

		[Test]
		public void AddData_Successfully()
		{
			var data = Substitute.For<IDataMockup>();
			
			_dataService.AddOrReplaceData(data);

			Assert.AreSame(data, _dataService.GetData<IDataMockup>());
		}

		[Test]
		public void ReplaceData_Successfully()
		{
			var data = Substitute.For<IDataMockup>();
			var data1 = new object();

			_dataService.AddOrReplaceData(data1);
			_dataService.AddOrReplaceData(data);

			Assert.AreNotSame(data1, _dataService.GetData<IDataMockup>());
			Assert.AreSame(data, _dataService.GetData<IDataMockup>());
		}

		[Test]
		public void GetData_NotFound_ThrowsException()
		{
			Assert.Throws<KeyNotFoundException>(() => _dataService.GetData<IDataMockup>());
		}
	}
}