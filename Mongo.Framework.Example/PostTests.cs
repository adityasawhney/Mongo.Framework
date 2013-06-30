using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mongo.Framework.Example.DataAccess;
using Mongo.Framework.Example.Model;

namespace Mongo.Framework.Example
{
	[TestClass]
	public class PostTests
	{
		private IPostDataAccess postDataAccess;

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

		[TestMethod]
		public void TestPostFunctionality()
		{
			// Create few Post to enter into the database.
			const string title = "My First Post";
			var post1 = new Post()
			{
				Title = title,
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
			postDataAccess.CreatePost(post1);
			postDataAccess.CreatePost(post2);

			// Get a post
			var posts = postDataAccess.FindPostWithCommentsBy("sara");

			// Verify the acceptance criteria
			Assert.AreEqual(1, posts.Count());
			Assert.AreEqual(title, posts.First());

			// Compute tags for posts
			postDataAccess.ComputeTags(new [] { post1.Title });

			// Get the post
			var postsWithTag = postDataAccess.GetPosts();

			// Verify the acceptance criteria
			Assert.AreEqual(2, postsWithTag.Count());
			Assert.IsTrue(postsWithTag.First(p => p.Title == post1.Title).Tags.Count > 0);
			Assert.IsTrue(postsWithTag.First(p => p.Title == post2.Title).Tags.Count == 0);
		}
	}
}
