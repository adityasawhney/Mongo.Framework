using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mongo.Framework.Example.DataAccess;

namespace Mongo.Framework.Example.Test
{
	public abstract class PostTestBase
	{
		private IPostDataAccess postDataAccess;
		protected static readonly Random RANDOM = new Random();

		protected virtual IPostDataAccess PostAccess { get { return postDataAccess; } }

		[TestInitialize]
		public void Setup()
		{
			// Instantiate the data access for posts
			var connectionInfo = ConnectionInfo.FromAppConfig("PostsDB");
			postDataAccess = new PostDataAccess(connectionInfo);
		}

		[TestCleanup]
		public void Teardown()
		{
			// Make the test repeatable by deleting all the posts
			postDataAccess.DeleteAllPost();
		}
	}
}
