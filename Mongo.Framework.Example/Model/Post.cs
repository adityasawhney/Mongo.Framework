using System;
using System.Collections.Generic;
using Mongo.Framework.Model;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Mongo.Framework.Example.Model
{
	[MongoCollectionName("post")]
	public class Post
	{
		[BsonId]
		public ObjectId Id { get; private set; }
		public string Title { get; set; }
		public string Body { get; set; }
		public IList<Comment> Comments { get; set; }
	}

	public class Comment
	{
		[BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
		public DateTime TimePosted { get; set; }
		public string Email { get; set; }
		public string Body { get; set; }
	}
}
