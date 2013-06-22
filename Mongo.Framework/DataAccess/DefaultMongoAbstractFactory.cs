using Mongo.Framework.Generator;
using Mongo.Framework.Repository;
using Mongo.Framework.TransientFault;

namespace Mongo.Framework.DataAccess
{
	/// <summary>
	/// Default mongo abstract factory implementation based on 10gen C# driver
	/// </summary>
	public class DefaultMongoAbstractFactory : IMongoDataAccessAbstractFactory
	{
		public DefaultMongoAbstractFactory(ConnectionInfo connectionInfo)
		{
			RepositoryFactory = new DefaultMongoRepositoryFactory(connectionInfo);
			ErrorHandlingFactory = new DefaultMongoErrorHandlingFactory();
			IdGeneratorFactory = new DefaultMongoIdGeneratorFactory(connectionInfo);
			HashGeneratorFactory = new DefaultMongoHashGeneratorFactory();
		}

		public IMongoRepositoryFactory RepositoryFactory { get; private set; }
		public IMongoErrorHandlingFactory ErrorHandlingFactory { get; private set; }
		public IMongoIdGeneratorFactory IdGeneratorFactory { get; private set; }
		public IMongoHashGeneratorFactory HashGeneratorFactory { get; private set; }
	}
}
