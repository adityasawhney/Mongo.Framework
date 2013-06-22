using System.Linq;
using System.Net.Sockets;
using Microsoft.Practices.TransientFaultHandling;
using MongoDB.Driver;

namespace Mongo.Framework.TransientFault
{
	/// <summary>
	/// High level categorization of Mongo errors we care about
	/// </summary>
	public enum MongoError
	{
		Unknown,
		NotMaster,
		ConnectionError,
		CommunicationError
	}

	public class MongoTransientErrorDetectionStrategy : ITransientErrorDetectionStrategy
	{
		// Errors we think are transient and worth re-trying
		private static readonly MongoError[] TRANSIENT_MONGO_ERRORS = 
		{
		    MongoError.NotMaster,
			MongoError.ConnectionError
		};

		/// <summary>
		/// Determines whether the specified exception represents a transient failure that can be compensated by a retry.
		/// </summary>
		/// <param name="ex">The exception object to be verified.</param>
		/// <returns>
		/// True if the specified exception is considered as transient, otherwise false.
		/// </returns>
		public bool IsTransient(System.Exception ex)
		{
			return ex != null && (CheckIsTransient(ex) || (ex.InnerException != null && CheckIsTransient(ex.InnerException)));
		}

		private static bool CheckIsTransient(System.Exception exception)
		{
			// TODO You might want to check and ignore app level (i.e. non-DB) exception

			// Check for .Net runtime generated exceptions
			var socketFault = exception as SocketException;
			if (socketFault != null)
			{
				return socketFault.SocketErrorCode == SocketError.TimedOut;
			}

			// Check for Mongo C# driver generated exceptions
			if (exception is MongoConnectionException)
				return true;

			var queryException = exception as MongoQueryException;
			if (queryException != null)
			{
				var error = queryException.GetError();
				return IsTransientMongoError(error);
			}

			var commandException = exception as MongoCommandException;
			if (commandException != null)
			{
				var error = commandException.GetError();
				return IsTransientMongoError(error);
			}

			return false;
		}

		private static bool IsTransientMongoError(MongoError error)
		{
			return TRANSIENT_MONGO_ERRORS.Contains(error);
		}
	}
}
