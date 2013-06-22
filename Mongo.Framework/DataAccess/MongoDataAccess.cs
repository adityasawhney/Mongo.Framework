using System;
using Microsoft.Practices.TransientFaultHandling;
using Mongo.Framework.Command;
using Mongo.Framework.TransientFault;

namespace Mongo.Framework.DataAccess
{
	public class MongoDataAccess
	{
		// TODO Configure these are per your needs
		private const int TIMEOUT_IN_SEC = 60;
		private const int MAX_ATTEMPTS = 10;
		private static readonly TimeSpan ATTEMPT_DELAY = TimeSpan.FromSeconds(3);

		private readonly ConnectionInfo connectionInfo;
		private readonly IMongoDataAccessAbstractFactory factories;
		private readonly RetryPolicy retryPolicy;

		public MongoDataAccess(ConnectionInfo connectionInfo, IMongoDataAccessAbstractFactory factories)
		{
			this.connectionInfo = connectionInfo;
			this.factories = factories;
			this.retryPolicy = CreateRetryPolicy(factories.ErrorHandlingFactory);
		}

		/// <summary>
		/// Executes the command.
		/// </summary>
		/// <typeparam name="TResult">The type of the result.</typeparam>
		/// <param name="command">The command.</param>
		/// <returns>Result of command execution</returns>
		public TResult ExecuteCommand<TResult>(IMongoCommand command)
		{
			int retryCount = 0;

			// Inject command dependencies
			command.ConnectionInfo = this.connectionInfo;
			command.RepositoryFactory = this.factories.RepositoryFactory;
			command.ErrorHandlingFactory = this.factories.ErrorHandlingFactory;
			command.IdGeneratorFactory = this.factories.IdGeneratorFactory;
			command.HashGeneratorFactory = this.factories.HashGeneratorFactory;
			command.CommandTimeout = TIMEOUT_IN_SEC;

			// Validate the command before we execute it
			command.Validate();

			// Create the retry event handler
			EventHandler<RetryingEventArgs> retryHandler = (s, e) =>
			{
				retryCount++;
			};

			bool unsubscribeRetryEventHandler = false;
			TResult result;

			try
			{
				this.retryPolicy.Retrying += retryHandler;
				unsubscribeRetryEventHandler = true;

				result = this.retryPolicy.ExecuteAction(() => command.Execute<TResult>());
			}
			catch (System.Exception ex)
			{
				if (retryCount == MAX_ATTEMPTS)
				{
					// TODO You might to do something special when max attempts exceed
				}

				throw;
			}
			finally
			{
				// Unsubscribe otherwise we will leak memory big time
				if (unsubscribeRetryEventHandler)
				{
					this.retryPolicy.Retrying -= retryHandler;
				}
			}

			return result;
		}

		private static RetryPolicy CreateRetryPolicy(IMongoErrorHandlingFactory errorHandlingFactory)
		{
			var errorDetectionStrategy = errorHandlingFactory.CreateErrorDetectionStrategy();
			var retryStrategy = new FixedInterval(MAX_ATTEMPTS, ATTEMPT_DELAY);
			return new RetryPolicy(errorDetectionStrategy, retryStrategy);
		}
	}
}
