using Mongo.Framework.Model;
using MongoDB.Driver;

namespace Mongo.Framework.Repository
{
	/// <summary>
	/// Factory implementation to instantiate DefaultMongoRepository
	/// </summary>
	public class DefaultMongoRepositoryFactory : IMongoRepositoryFactory
	{
		private readonly MongoDatabase mongoDatabase;

		public DefaultMongoRepositoryFactory(ConnectionInfo connectionInfo)
		{
			var urlBuilder = new MongoConnectionStringBuilder(connectionInfo.ConnectionString);
			var mongoClient = new MongoClient(connectionInfo.ConnectionString);
			this.mongoDatabase = mongoClient.GetServer().GetDatabase(urlBuilder.DatabaseName);
		}

		public IMongoRepository<T> Create<T>() where T : class, new()
		{
			return Create<T>(null);
		}

		public IMongoRepository<T> Create<T>(MongoCollectionSettings settings) where T : class, new()
		{
			var collection = GetCollection<T>(this.mongoDatabase, settings);
			return new DefaultMongoRepository<T>(collection);
		}

		private static MongoCollection<T> GetCollection<T>(MongoDatabase db, MongoCollectionSettings settings) where T : class
		{
			string collectionName = MongoCollectionName.Get<T>();
			// Default settings don't need special treatment as everything comes from connection string
			return settings == null ? db.GetCollection<T>(collectionName) : db.GetCollection<T>(collectionName, settings);
		}
	}
}
