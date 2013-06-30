using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Mongo.Framework.Command;
using Mongo.Framework.Example.DataAccess;
using Mongo.Framework.Example.Model;
using Mongo.Framework.Exception;
using Mongo.Framework.Repository;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace Mongo.Framework.Example.Command
{
	class ComputeTagsCommand : BaseMongoCommand
	{
		public string[] PostTitles { get; set; }
		public ComputeTagsDelegate ComputeTagsFromBody { get; set; }

		// Posts which have been processed out of the given list of posts to tag
		private readonly ConcurrentQueue<string> processedPosts = new ConcurrentQueue<string>();

		protected override void ValidateInternal()
		{
			if (ComputeTagsFromBody == null)
			{
				throw new InvalidParameterException("ComputeTagsFromBody", null, typeof(ComputeTagsDelegate).Name);
			}
		}

		protected override object ExecuteInternal()
		{
			var repo = GetRepository<Post>();

			// Incase of retry ignore posts which were successfully processed in previous pass
			var postsToProcess = PostTitles.Except(processedPosts);

			Parallel.ForEach(postsToProcess, (postTitle) =>
			{
				var message = string.Format("setting tags for post: {0}", postTitle);
				Console.WriteLine(message);

				RetryWithTimeout(message, () =>
				{
					var post = repo.Single(p => p.Title == postTitle);

					// STATE RETRANSMIT
					// Only retry if its not in the expected state
					if (post.Tags != null && post.Tags.Count > 0) return true;

					return UpdateTagsInPost(repo, post);
				});

				processedPosts.Enqueue(postTitle);
			});

			return RETURN_CODE_SUCCESS;
		}

		private bool UpdateTagsInPost(IMongoRepository<Post> repo, Post post)
		{
			var tags = ComputeTagsFromBody(post.Body);

			// COMPARE AND SWAP (CAS)
			// - Compare the body text to make sure its not stale
			// - Swap in the freshly computed tags
			return repo.Update(Query.And(
									Query.EQ("_id", post.Id),
									Query.EQ(Post.FN_BODY, post.Body)
							   ),
			                   Update.AddToSetEachWrapped(Post.FN_TAGS, tags),
			                   UpdateFlags.None);
		}
	}
}
