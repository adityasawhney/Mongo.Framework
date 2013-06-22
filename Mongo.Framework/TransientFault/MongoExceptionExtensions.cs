using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Mongo.Framework.TransientFault
{
	/// <summary>
	/// Utility class which provides Mongo error handling related extension methods.
	/// 
	/// UGLY:
	/// MongoDB doesnt return error codes explicitly in the exception so we have
	/// to pull it out of the return error messages. Also, 10gen driver should provide
	/// this functionality of transient fault detection to begin with.
	/// </summary>
	public static class MongoExceptionExtensions
	{
		private const int UNKNOWN_MONGO_ERROR_CODE = -1;

		public static int GetErrorCode(this MongoException exception)
		{
			int errorCode = UNKNOWN_MONGO_ERROR_CODE;

			var queryException = exception as MongoQueryException;
			if (queryException != null && queryException.QueryResult != null)
			{
				errorCode = GetMongoErrorCode(queryException.QueryResult);
			}

			var commandException = exception as MongoCommandException;
			if (commandException != null &&
				commandException.CommandResult != null &&
				commandException.CommandResult.Response != null)
			{
				errorCode = GetMongoErrorCode(commandException.CommandResult.Response);
			}

			return errorCode;
		}

		public static MongoError GetError(this MongoQueryException exception)
		{
			var result = exception.QueryResult;
			return result != null ? GetMongoError(result) : MongoError.Unknown;
		}

		public static MongoError GetError(this MongoCommandException exception)
		{
			var error = MongoError.Unknown;
			var result = exception.CommandResult;

			if (result != null)
			{
				var response = result.Response;
				if (response != null)
					error = GetMongoError(response);
				if (error == MongoError.Unknown && IsErrorStringMatch(GetMongoErrorMessage(result), "not master"))
					error = MongoError.NotMaster;
			}
			return error;
		}

		private static MongoError GetMongoError(BsonDocument document)
		{
			// We got this list from Aristarkh Zagorodnikov <onyxmaster@gmail.com>
			// Refer: https://jira.mongodb.org/browse/SERVER-2400
			int errorCode = GetMongoErrorCode(document);
			switch (errorCode)
			{
				case 10009:
				case 10054:
				case 10058:
				case 13435:
					return MongoError.NotMaster;

				case 11002:
				case 14827:
					return MongoError.ConnectionError;

				case 9001:
				case 10276:
					return MongoError.CommunicationError;
			}
			return MongoError.Unknown;
		}

		private static int GetMongoErrorCode(BsonDocument document)
		{
			BsonValue code;
			return document.TryGetValue("code", out code) ? code.ToInt32() : UNKNOWN_MONGO_ERROR_CODE;
		}

		private static string GetMongoErrorMessage(CommandResult result)
		{
			if (result == null) return string.Empty;
			if (!string.IsNullOrEmpty(result.ErrorMessage)) return result.ErrorMessage;

			if (result.Response != null)
			{
				BsonValue err;
				return result.Response.TryGetValue("err", out err) ? err.ToString() : string.Empty;
			}

			return string.Empty;
		}

		private static bool IsErrorStringMatch(string exceptionErrorString, params string[] errorStrings)
		{
			return !string.IsNullOrEmpty(exceptionErrorString) && errorStrings.Any(exceptionErrorString.Contains);
		}
	}
}
