using Mongo.Framework.Command;
using Mongo.Framework.Example.Model;

namespace Mongo.Framework.Example.Command
{
	class CreatePostCommand : BaseMongoCommand
	{
		public Post PostToCreate { get; set; }

		protected override object ExecuteInternal()
		{
			GetRepository<Post>().Add(PostToCreate);

			return RETURN_CODE_SUCCESS;
		}
	}
}
