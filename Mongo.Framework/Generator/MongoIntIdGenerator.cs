using Mongo.Framework.Model;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace Mongo.Framework.Generator
{
	/// <summary>
	/// Integer identity generator using "Identity" collection in Mongo
	/// </summary>
	public class MongoIntIdGenerator : IIdGenerator
	{
		private readonly IdType idType;
		private readonly MongoDatabase mongoDb;

		// For root level document id generation; used during the class map registration of data model 
		public MongoIntIdGenerator(IdType idType)
			: this(idType, null)
		{
		}

		// For embedded document id generation; used during MongoCommand execution
		public MongoIntIdGenerator(IdType idType, MongoDatabase mongoDb)
		{
			this.idType = idType;
			this.mongoDb = mongoDb;
		}

		/// <summary>
		/// Generates an Id for a document.
		/// </summary>
		/// <param name="container">The container of the document (will be a MongoCollection when called from the C# driver).</param>
		/// <param name="notUsed">The not used.</param>
		/// <returns>
		/// An Id.
		/// </returns>
		public object GenerateId(object container, object notUsed)
		{
			var identityCollection = GetCollection(container);

			var query = Query.EQ(Identity.FN_ID, this.idType.Name);

			return identityCollection
				.FindAndModify(query, null, Update.Inc(Identity.FN_SEQ, 1), true, true)
				.ModifiedDocument[Identity.FN_SEQ]
				.AsInt32;
		}

		/// <summary>
		/// Tests whether an Id is empty.
		/// </summary>
		/// <param name="id">The Id.</param>
		/// <returns>
		/// True if the Id is empty.
		/// </returns>
		public bool IsEmpty(object id)
		{
			return (int)id == 0;
		}

		private MongoCollection GetCollection(object container)
		{
			// Prefer the collection provided in ctor over the one given during id generation
			MongoDatabase db = this.mongoDb ?? ((MongoCollection)container).Database;
			string collectionName = MongoCollectionName.Get<Identity>();
			return db.GetCollection(collectionName);
		}
	}
}
