using System;
using System.Collections.Generic;
using GameLovers.Services;
using NUnit.Framework;

// ReSharper disable once CheckNamespace

namespace GameLoversEditor.Services.Tests
{
	[TestFixture]
	public class DataServiceTest
	{
		private DataService _dataService;
		private DataMockup _data;

		public class DataMockup
		{
			public int Int;
			public string String;
		}

		[SetUp]
		public void Init()
		{
			_dataService = new DataService();
			_data = new DataMockup();
		}

		[Test]
		public void AddData_Successfully()
		{
			_dataService.AddData(_data);

			Assert.AreSame(_data, _dataService.GetData<DataMockup>());
		}

		[Test]
		public void GetData_NotFound_ThrowsException()
		{
			Assert.Throws<KeyNotFoundException>(() => _dataService.GetData<DataMockup>());
		}
	}
}