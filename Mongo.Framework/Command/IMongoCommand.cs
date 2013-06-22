using System;
using Mongo.Framework.Exception;
using Mongo.Framework.Generator;
using Mongo.Framework.Repository;
using Mongo.Framework.TransientFault;
using MongoDB.Driver;

namespace Mongo.Framework.Command
{
	/// <summary>
	/// The Mongo command interface which corresponds to Store Procedures in SQL world
	/// </summary>
	public interface IMongoCommand : IDisposable
	{
		// Connection context in which the current command is executing
		ConnectionInfo ConnectionInfo { set; }

		// Use this to override the default settings dervied from connection string
		MongoCollectionSettings SettingsOverride { set; }

		// Used to limit the time it spends executing (relevant incase of retries)
		int CommandTimeout { set; }

		// Used to get the repository interface of required data model
		// This is factory because command might access multiple repositories
		IMongoRepositoryFactory RepositoryFactory { set; }

		// Used for sending alert emails
		IMongoErrorHandlingFactory ErrorHandlingFactory { set; }

		// Used to generate Id for Identity columns/fields in data model
		IMongoIdGeneratorFactory IdGeneratorFactory { set; }

		// Used to generate hash code for given input
		IMongoHashGeneratorFactory HashGeneratorFactory { set; }

		/// <summary>
		/// Executes the command under specified context.
		/// 
		/// IMPORTANT:
		/// Command might be executed multiple times under transient fault conditions
		/// because of the retry logic. 
		/// 
		/// Guidelines,
		/// - Be careful with implementation as command might have done its job but still reported error.
		/// - Employ some sort of "state retransmit" mechanism if command isn't idempotent.
		/// - Query operations are inherently idempotent so this is mostly relevant for commands which
		///   do mutation operations.
		/// </summary>
		/// <typeparam name="TResult">The type of the result.</typeparam>
		/// <returns>Corresponding result object</returns>
		TResult Execute<TResult>();

		/// <summary>
		/// Validates this instance.
		/// <exception cref="InvalidParameterException">If validation fails</exception>
		/// </summary>
		void Validate();
	}
}
