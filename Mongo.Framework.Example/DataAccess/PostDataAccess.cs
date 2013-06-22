using Mongo.Framework.DataAccess;
using Mongo.Framework.Example.Command;
using Mongo.Framework.Example.Model;

namespace Mongo.Framework.Example.DataAccess
{
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

		public string[] FindPostWithCommentsBy(string commenter)
		{
			var command = new FindPostWithCommentsByCommand() {CommentsBy = commenter};
			return ExecuteCommand<string[]>(command);
		}

		public void DeleteAllPost()
		{
			ExecuteCommand<int>(new DeleteAllPostCommand());
		}
	}
}
