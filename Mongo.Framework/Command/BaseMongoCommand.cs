using System;
using System.Diagnostics;
using System.Text;
using Mongo.Framework.Exception;
using Mongo.Framework.Generator;
using Mongo.Framework.Repository;
using Mongo.Framework.TransientFault;
using MongoDB.Driver;

namespace Mongo.Framework.Command
{
	/// <summary>
	/// Abstract command class which provides most of DbCommand functionality
	/// so that actual Command implementations have to do minimal work.
	/// </summary>
	public abstract class BaseMongoCommand : IMongoCommand
	{
		public const int RETURN_CODE_SUCCESS = 0;

		// IMongoCommand properties
		public ConnectionInfo ConnectionInfo { protected get; set; }
		public MongoCollectionSettings SettingsOverride { protected get; set; }
		public int CommandTimeout { protected get; set; }
		public IMongoRepositoryFactory RepositoryFactory { protected get; set; }
		public IMongoErrorHandlingFactory ErrorHandlingFactory { protected get; set; }
		public IMongoIdGeneratorFactory IdGeneratorFactory { protected get; set; }
		public IMongoHashGeneratorFactory HashGeneratorFactory { protected get; set; }

		#region Main external methods

		// Main execute operation
		public virtual TResult Execute<TResult>()
		{
			object result = ExecuteInternal();

			// If the request type and the command result type dont match then
			// error so that deserialization exceptions can be easily detected.
			if ((result != null) && !(result is TResult))
			{
				throw new TypeMismatchException(typeof(TResult), result.GetType(),
						"Mongo ORM/Deserialization error. DB document format and command result type are different.");
			}

			return (TResult)result;
		}

		public void Validate()
		{
			if (!this.ConnectionInfo.IsValid())
			{
				throw new InvalidParameterException(string.Format("MongoCommand '{0}' has invalid connection info {1}",
																  this.GetType().Name,
																  this.ConnectionInfo));
			}

			if (this.RepositoryFactory == null)
			{
				throw new InvalidParameterException(string.Format("MongoCommand '{0}' has invalid repository factory.", this.GetType().Name));
			}

			if (this.ErrorHandlingFactory == null)
			{
				throw new InvalidParameterException(string.Format("MongoCommand '{0}' has invalid error handling factory.", this.GetType().Name));
			}

			if (this.IdGeneratorFactory == null)
			{
				throw new InvalidParameterException(string.Format("MongoCommand '{0}' has invalid id generator factory.", this.GetType().Name));
			}

			if (this.HashGeneratorFactory == null)
			{
				throw new InvalidParameterException(string.Format("MongoCommand '{0}' has invalid hash generator factory.", this.GetType().Name));
			}

			ValidateInternal();
		}

		#endregion

		#region Main internal utility methods

		// Template method which does the heavy lifting
		protected virtual void ValidateInternal() { }
		protected abstract object ExecuteInternal();

		protected IMongoRepository<T> GetRepository<T>() where T : class, new()
		{
			return this.RepositoryFactory.Create<T>(this.SettingsOverride);
		}

		protected IMongoRepository<T> GetRepository<T>(MongoCollectionSettings settings) where T : class, new()
		{
			return this.RepositoryFactory.Create<T>(settings);
		}

		protected T GenerateId<T>(IdType idType)
		{
			var generator = this.IdGeneratorFactory.Create(idType);
			return (T)generator.GenerateId(null, null);
		}

		protected string GenerateHash(HashType hashType, string input)
		{
			var generator = this.HashGeneratorFactory.Create(hashType);
			return generator.GenerateHash(input);
		}

		// Compose query expression for embedded documents, like "shares.fic.sicd"
		protected string ComposeQueryOperator(params string[] tokens)
		{
			return String.Join(".", tokens);
		}

		// Compose update expression for embedded array with position operator "shares.$.status"
		// The position operator($) represents the array index for element which matched the query
		protected string ComposePositionOperator(string[] left, params string[] right)
		{
			Debug.Assert(left.Length > 0);

			StringBuilder b = new StringBuilder();
			foreach (var token in left)
			{
				b.Append(token);
				b.Append(".");
			}
			b.Append("$");
			foreach (var token in right)
			{
				b.Append(".");
				b.Append(token);
			}
			return b.ToString();
		}

		protected string ComposePositionOperator(string left, params string[] right)
		{
			return ComposePositionOperator(new[] { left }, right);
		}

		// Utility method to keep retrying an operation until it succeeds or timeouts
		protected void RetryWithTimeout(string message, Func<bool> action)
		{
			bool stopTrying = false;
			DateTime timeoutTime = DateTime.UtcNow + TimeSpan.FromSeconds(CommandTimeout);

			while (!stopTrying)
			{
				if (DateTime.UtcNow > timeoutTime)
				{
					throw new TimeoutException(message);
				}

				stopTrying = action();
			}
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			// Get rid of any resources that need to be disposed
		}

		#endregion
	}
}
