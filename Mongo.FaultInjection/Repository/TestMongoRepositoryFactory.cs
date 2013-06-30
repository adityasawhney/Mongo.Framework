using Mongo.Framework.Repository;
using MongoDB.Driver;

namespace Mongo.FaultInjection.Repository
{
	public class TestMongoRepositoryFactory : IMongoRepositoryFactory
	{
		private readonly IMongoRepositoryFactory repositoryFactory;
		private readonly FaultHandler faultHandler;

		public IMongoRepositoryFactory UnderlyingFactory { get { return repositoryFactory; } }

		public TestMongoRepositoryFactory(IMongoRepositoryFactory repositoryFactory,
										  FaultHandler faultHandler)
		{
			this.repositoryFactory = repositoryFactory;
			this.faultHandler = faultHandler;
		}

		public IMongoRepository<T> Create<T>() where T : class, new()
		{
			return Create<T>(null);
		}

		public IMongoRepository<T> Create<T>(MongoCollectionSettings settings) where T : class, new()
		{
			var repository = new TestMongoRepository<T>(this.repositoryFactory.Create<T>(settings));
			if (faultHandler != null) repository.FaultListeners += faultHandler;
			return repository;
		}
	}
}
