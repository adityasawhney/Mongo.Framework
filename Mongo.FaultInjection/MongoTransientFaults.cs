using System;
using System.Net.Sockets;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Mongo.FaultInjection
{
	public class MongoTransientFaults
	{
		private static readonly Random RANDOM = new Random();

		private static readonly Exception[] TRANSIENT_EXCEPTIONS = new Exception[]
		{
			new MongoConnectionException("Network failure"),
 			new SocketException((int)SocketError.TimedOut),
			new MongoQueryException("QueryFailure flag was not master and slaveOk=false", 
									BsonDocument.Parse("{ \"$err\" : \"not master and slaveOk=false\", \"code\" : 13435 }")), 
			new MongoCommandException("command failed", new CommandResult( 
									  BsonDocument.Parse("{ \"$err\" : \"socket exception\", \"code\" : 11002 }"))),
			new MongoQueryException("dbclient error communicating with server:", 
									BsonDocument.Parse("{ \"$err\" : \"dbclient error communicating with server:\", \"code\" : 14827 }")), 
			new MongoCommandException("command failed", new CommandResult( 
									  BsonDocument.Parse("{ \"$err\" : \"not master\", \"code\" : 10054 }"))),
		};

		public static Exception GetRandomTransientFault()
		{
			return TRANSIENT_EXCEPTIONS[RANDOM.Next(TRANSIENT_EXCEPTIONS.Length - 1)];
		}
	}
}
