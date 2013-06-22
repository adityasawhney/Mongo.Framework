using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace Mongo.Framework.Generator
{
	/// <summary>
	/// Default Id Generator factory implementation
	/// </summary>
	public class DefaultMongoIdGeneratorFactory : IMongoIdGeneratorFactory
	{
		private readonly MongoDatabase mongoDatabase;

		public DefaultMongoIdGeneratorFactory(ConnectionInfo connectionInfo)
		{
			var urlBuilder = new MongoConnectionStringBuilder(connectionInfo.ConnectionString);
			var mongoClient = new MongoClient(connectionInfo.ConnectionString);
			this.mongoDatabase = mongoClient.GetServer().GetDatabase(urlBuilder.DatabaseName);
		}

		public IIdGenerator Create(IdType type)
		{
			return new MongoIntIdGenerator(type, this.mongoDatabase);
		}
	}
}
