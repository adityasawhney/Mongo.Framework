using Mongo.Framework.Generator;
using Mongo.Framework.Repository;
using Mongo.Framework.TransientFault;

namespace Mongo.Framework.DataAccess
{
	/// <summary>
	/// Abstract Factory which provides access to all factories which MongoDataAccess relies on
	/// </summary>
	public interface IMongoDataAccessAbstractFactory
	{
		IMongoRepositoryFactory RepositoryFactory { get; }
		IMongoErrorHandlingFactory ErrorHandlingFactory { get; }
		IMongoIdGeneratorFactory IdGeneratorFactory { get; }
		IMongoHashGeneratorFactory HashGeneratorFactory { get; }
	}
}
