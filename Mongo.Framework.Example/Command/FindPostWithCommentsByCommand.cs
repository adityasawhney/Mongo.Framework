using System.Linq;
using Mongo.Framework.Command;
using Mongo.Framework.Example.Model;
using Mongo.Framework.Exception;

namespace Mongo.Framework.Example.Command
{
	class FindPostWithCommentsByCommand : BaseMongoCommand
	{
		public string CommentsBy { get; set; }

		protected override void ValidateInternal()
		{
			if (string.IsNullOrEmpty(CommentsBy))
			{
				throw new InvalidParameterException("CommentsBy value is not specified");
			}
		}

		protected override object ExecuteInternal()
		{
			var posts = from p in GetRepository<Post>().All()
						where p.Comments.Any(c => c.Email.StartsWith(CommentsBy))
						select p.Title;

			return posts.ToArray();
		}
	}
}
