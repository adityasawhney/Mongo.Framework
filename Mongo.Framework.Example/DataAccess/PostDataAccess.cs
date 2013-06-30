using System;
using System.Collections.Generic;
using System.Linq;
using Mongo.Framework.DataAccess;
using Mongo.Framework.Example.Command;
using Mongo.Framework.Example.Model;

namespace Mongo.Framework.Example.DataAccess
{
	internal delegate IEnumerable<string> ComputeTagsDelegate(string body);

	class PostDataAccess : MongoDataAccess, IPostDataAccess
	{
		public PostDataAccess(ConnectionInfo connectionInfo, IMongoDataAccessAbstractFactory factories) 
			: base(connectionInfo, factories)
		{
		}

		public PostDataAccess(ConnectionInfo connectionInfo)
			: this(connectionInfo, new DefaultMongoAbstractFactory(connectionInfo))
		{
		}

		public void CreatePost(Post post)
		{
			var command = new CreatePostCommand() { PostToCreate = post };
			ExecuteCommand<int>(command);
		}

		public Post[] GetPosts()
		{
			return ExecuteCommand<Post[]>(new GetPostsCommand());
		}

		public string[] FindPostWithCommentsBy(string commenter)
		{
			var command = new FindPostWithCommentsByCommand() {CommentsBy = commenter};
			return ExecuteCommand<string[]>(command);
		}

		public void ComputeTags(string[] postTitles)
		{
			var command = new ComputeTagsCommand()
			{
				PostTitles = postTitles, 
				ComputeTagsFromBody = ComputeTagsFromBodyDelegate
			};
			ExecuteCommand<int>(command);
		}

		public void DeleteAllPost()
		{
			ExecuteCommand<int>(new DeleteAllPostCommand());
		}

		private static IEnumerable<string> ComputeTagsFromBodyDelegate(string body)
		{
			// Randomly pick 3 words as tags
			Random rnd = new Random();
			string[] words = body.Split(' ');
			return words.OrderBy(x => rnd.Next()).Take(3);
		}
	}
}
