using Mongo.Framework.Command;
using Mongo.Framework.Example.Model;

namespace Mongo.Framework.Example.Command
{
	class DeleteAllPostCommand : BaseMongoCommand
	{
		protected override object ExecuteInternal()
		{
			GetRepository<Post>().RemoveAll();
			return RETURN_CODE_SUCCESS;
		}
	}
}
