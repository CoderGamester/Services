using System;
using GameLovers.Services;
using NSubstitute.Exceptions;
using NUnit.Framework;

// ReSharper disable once CheckNamespace

namespace GameLoversEditor.Services.Tests
{
	public class MainInstallerTest
	{
		private interface IInterface {}
		private class Implementation : IInterface {}
		
		[TearDown]
		public void CleanUp()
		{
			MainInstaller.Clean();
		}

		[Test]
		public void Bind_Resolve_Successfully()
		{
			MainInstaller.Bind<IInterface>(new Implementation());
			
			var instance = MainInstaller.Resolve<IInterface>();
			
			Assert.IsNotNull(instance);
			Assert.AreSame(typeof(Implementation), instance.GetType());
		}

		[Test]
		public void Bind_NotInterface_ThrowsException()
		{
			Assert.Throws<ArgumentException>(() => MainInstaller.Bind<Implementation>(new Implementation()));
		}

		[Test]
		public void Bind_NotImplementing_ThrowsException()
		{
			Assert.Throws<ArgumentException>(() => MainInstaller.Bind<IInterface>(new int[0]));
		}

		[Test]
		public void Resolve_NotBinded_ThrowsException()
		{
			Assert.Throws<ArgumentException>(() => MainInstaller.Resolve<IInterface>());
		}
	}
}