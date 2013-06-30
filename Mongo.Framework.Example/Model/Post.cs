using System;
using System.Collections.Generic;
using Mongo.Framework.Model;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Options;

namespace Mongo.Framework.Example.Model
{
	[MongoCollectionName(COLLECTION_NAME)]
	public class Post
	{
		#region Field names

		public const string COLLECTION_NAME = "post";
		public const string FN_TITLE = "title";
		public const string FN_BODY = "body";
		public const string FN_TAGS = "tags";
		public const string FN_COMMENTS = "comments";
		public const string FN_COMMENT_TIME = "time";
		public const string FN_COMMENT_EMAIL = "email";
		public const string FN_COMMENT_BODY = "body";

		#endregion

		#region Data model

		public ObjectId Id { get; set; }
		public string Title { get; set; }
		public string Body { get; set; }
		public IList<string> Tags { get; set; }
		public IList<Comment> Comments { get; set; }

		#endregion

		#region Object mapping

		static Post()
		{
			// Setup the mapping from collection columns to model data members
			BsonClassMap.RegisterClassMap<Post>(cm =>
			{
				cm.AutoMap();
				cm.SetIgnoreExtraElements(true); // For backward compatibility
				cm.SetIdMember(cm.GetMemberMap(c => c.Id));
				cm.GetMemberMap(c => c.Title)
				  .SetElementName(FN_TITLE);
				cm.GetMemberMap(c => c.Body)
				  .SetElementName(FN_BODY);
				cm.GetMemberMap(c => c.Tags)
				  .SetElementName(FN_TAGS);
				cm.GetMemberMap(c => c.Comments)
				  .SetElementName(FN_COMMENTS);
			});

			BsonClassMap.RegisterClassMap<Comment>(cm =>
			{
				cm.AutoMap();
				cm.SetIgnoreExtraElements(true); // For backward compatibility
				cm.GetMemberMap(c => c.TimePosted)
				  .SetElementName(FN_COMMENT_TIME)
				  .SetSerializationOptions(new DateTimeSerializationOptions(DateTimeKind.Utc));
				cm.GetMemberMap(c => c.Email)
				  .SetElementName(FN_COMMENT_EMAIL);
				cm.GetMemberMap(c => c.Body)
				  .SetElementName(FN_COMMENT_BODY);
			});
		}

		#endregion
	}

	public class Comment
	{
		public DateTime TimePosted { get; set; }
		public string Email { get; set; }
		public string Body { get; set; }
	}
}
