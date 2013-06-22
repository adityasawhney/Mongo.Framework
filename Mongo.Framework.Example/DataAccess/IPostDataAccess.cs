using Mongo.Framework.Example.Model;

namespace Mongo.Framework.Example.DataAccess
{
	interface IPostDataAccess
	{
		void CreatePost(Post post);

		string[] FindPostWithCommentsBy(string commenter);

		void DeleteAllPost();
	}
}
