using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mongo.Framework.Example.Model;

namespace Mongo.Framework.Example.Test
{
	[TestClass]
	public class PostTests : PostTestBase
	{
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
			PostAccess.CreatePost(post1);
			PostAccess.CreatePost(post2);

			// Get a post
			var posts = PostAccess.FindPostWithCommentsBy("sara");

			// Verify the acceptance criteria
			Assert.AreEqual(1, posts.Count());
			Assert.AreEqual(title, posts.First());

			// Compute tags for posts
			PostAccess.ComputeTags(new[] { post1.Title });

			// Get the post
			var postsWithTag = PostAccess.GetPosts();

			// Verify the acceptance criteria
			Assert.AreEqual(2, postsWithTag.Count());
			Assert.IsTrue(postsWithTag.First(p => p.Title == post1.Title).Tags.Count > 0);
			Assert.IsTrue(postsWithTag.First(p => p.Title == post2.Title).Tags.Count == 0);
		}
	}
}
