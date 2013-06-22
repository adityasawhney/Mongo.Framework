using Microsoft.Practices.TransientFaultHandling;

namespace Mongo.Framework.TransientFault
{
	/// <summary>
	/// Factory to instantiate error handling related artifacts.
	/// </summary>
	public interface IMongoErrorHandlingFactory
	{
		/// <summary>
		/// Creates the error detection strategy.
		/// </summary>
		/// <returns></returns>
		ITransientErrorDetectionStrategy CreateErrorDetectionStrategy();
	}
}
