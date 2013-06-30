using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mongo.FaultInjection;
using Mongo.FaultInjection.Repository;
using Mongo.Framework.Example.DataAccess;

namespace Mongo.Framework.Example.Test
{
	public abstract class PostFaultTestBase : PostTestBase, IFaultInjectionTest
	{
		protected static readonly int MAX_ATTEMPTS = 5;
		private IPostDataAccess postDataAccessOverriden;

		public IFaultInjectionTestScenario TestScenario { get; set; }
		protected override IPostDataAccess PostAccess { get { return postDataAccessOverriden; } }

		[TestInitialize]
		public void SetupFaultInjectionMagic()
		{
			var connectionInfo = ConnectionInfo.FromAppConfig("PostsDB");
			postDataAccessOverriden = new PostDataAccess(connectionInfo);
			MongoRepositoryHelper.DynamicOverride(postDataAccessOverriden, OnFault);
		}

		[TestCleanup]
		public void TeardownFaultInjectionMagic()
		{
			MongoRepositoryHelper.DynamicRestore(postDataAccessOverriden);
		}

		private void OnFault(FaultEvent fault)
		{
			if (TestScenario != null)
			{
                TestScenario.Events.Enqueue(fault);
                Console.WriteLine("Got fault event: {0}", fault);
			}
		}
	}
}
