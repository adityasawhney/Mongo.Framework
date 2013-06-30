using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Test.FaultInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mongo.FaultInjection;
using Mongo.FaultInjection.Repository;
using Mongo.FaultInjection.Trigger;
using Mongo.Framework.Example.Model;

namespace Mongo.Framework.Example.Test
{
	[TestClass]
	public class PostFaultInjectionTests : PostFaultTestBase
	{
		[TestMethod]
		public void TestComputeTagsWithFault()
		{
			// Create few Post to enter into the database.
			var post1 = new Post()
			{
				Title = "My First Post",
				Body = "This isn't a very long post.",
				Tags = new List<string>(),
				Comments = new List<Comment>
				{
					{ new Comment() { TimePosted = new DateTime(2013,1,1), 
									  Email = "jake@gmail.com", 
									  Body = "This article is too short!" } },
					{ new Comment() { TimePosted = new DateTime(2013,1,2), 
									  Email = "sara@gmail.com", 
									  Body = "I agree with Jake." } }
				}
			};

			var post2 = new Post()
			{
				Title = "My Second Post",
				Body = "This isn't a very long post either.",
				Tags = new List<string>(),
				Comments = new List<Comment>
				{
					{ new Comment() { TimePosted = new DateTime(2013,1,3), 
									  Email = "jackie@gmail.com", 
									  Body = "Why are you wasting your time!" } },
				}
			};

			// Insert the posts into the DB
			PostAccess.CreatePost(post1);
			PostAccess.CreatePost(post2);

			// Compute tags for posts
			using (new FaultInjection.FaultInjection(new RandomlyFaultQueryAndUpdatesTestScenario(), this))
			{
				PostAccess.ComputeTags(new[] { post1.Title });
			}

			// Get the post
			var postsWithTag = PostAccess.GetPosts();

			// Verify the acceptance criteria
			Assert.AreEqual(2, postsWithTag.Count());
			Assert.IsTrue(postsWithTag.First(p => p.Title == post1.Title).Tags.Count > 0);
			Assert.IsTrue(postsWithTag.First(p => p.Title == post2.Title).Tags.Count == 0);
		}

		class RandomlyFaultQueryAndUpdatesTestScenario : BaseFaultInjectionTestScenario
		{
			private readonly System.Exception expectedFault = MongoTransientFaults.GetRandomTransientFault();

			public override void SetupRules()
			{
				int maxQueryFailures = RANDOM.Next(1, MAX_ATTEMPTS);
				int maxUpdateFailures = MAX_ATTEMPTS - maxQueryFailures;
				Console.WriteLine("Max UPDATE fault failures: " + maxUpdateFailures);
				Console.WriteLine("Max QUERY fault failures: " + maxQueryFailures);

				Rules.Add(new FaultRule(MongoRepositoryHelper.Method.UPDATE_CAS,
										CustomConditions.TriggerRandomlyWithMax(maxUpdateFailures),
										BuiltInFaults.ThrowExceptionFault(expectedFault)));
				Rules.Add(new FaultRule(MongoRepositoryHelper.Method.SINGLE,
										CustomConditions.TriggerRandomly(1, maxQueryFailures),
										BuiltInFaults.ThrowExceptionFault(expectedFault)));
			}

			public override void AssertFaults()
			{
				Assert.IsNotNull(Events);
				Assert.IsTrue(Events.Count > 0, "Make sure there are no false positives");
				Console.WriteLine("Total UPDATE fault events: " + Events.Count(e => e.MethodName == MongoRepositoryHelper.Method.UPDATE_CAS));
				Console.WriteLine("Total QUERY fault events: " + Events.Count(e => e.MethodName == MongoRepositoryHelper.Method.SINGLE));
				Assert.IsTrue(Events.All(e => e.IsException));
				Assert.IsTrue(Events.All(e => e.Exception == expectedFault));
			}
		}
	}
}
