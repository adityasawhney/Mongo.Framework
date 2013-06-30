using System.Linq;
using Mongo.Framework.Command;
using Mongo.Framework.Example.Model;

namespace Mongo.Framework.Example.Command
{
	class GetPostsCommand : BaseMongoCommand
	{
		protected override object ExecuteInternal()
		{
			return GetRepository<Post>().All().ToArray();
		}
	}
}
