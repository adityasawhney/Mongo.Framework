using System.Linq;
using Mongo.Framework.Command;
using Mongo.Framework.Example.Model;

namespace Mongo.Framework.Example.Command
{
	class FindPostWithCommentsByCommand : BaseMongoCommand
	{
		public string CommentsBy { get; set; }

		protected override object ExecuteInternal()
		{
			var posts = from p in GetRepository<Post>().All()
						where p.Comments.Any(c => c.Email.StartsWith(CommentsBy))
						select p.Title;

			return posts.ToArray();
		}
	}
}
