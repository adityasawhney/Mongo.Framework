using Mongo.Framework.Example.Model;

namespace Mongo.Framework.Example.DataAccess
{
	public interface IPostDataAccess
	{
		void CreatePost(Post post);

		Post[] GetPosts();

		string[] FindPostWithCommentsBy(string commenter);

		void ComputeTags(string[] postTitles);

		void DeleteAllPost();
	}
}
