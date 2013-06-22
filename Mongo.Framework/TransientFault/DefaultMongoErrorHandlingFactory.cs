using Microsoft.Practices.TransientFaultHandling;

namespace Mongo.Framework.TransientFault
{
	/// <summary>
	/// Default Error handling factory implementation based on 10gen driver
	/// </summary>
	public class DefaultMongoErrorHandlingFactory : IMongoErrorHandlingFactory
	{
		public ITransientErrorDetectionStrategy CreateErrorDetectionStrategy()
		{
			return new MongoTransientErrorDetectionStrategy();
		}
	}
}
