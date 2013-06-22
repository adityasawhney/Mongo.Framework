using MongoDB.Driver;

namespace Mongo.Framework.Repository
{
	/// <summary>
	/// Factory to instantiate Mongo Repository instances of given type and settings
	/// </summary>
	public interface IMongoRepositoryFactory
	{
		/// <summary>
		/// Creates this instance of repository access of given type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		IMongoRepository<T> Create<T>() where T : class, new();

		/// <summary>
		/// Creates this instance of repository access of given type.
		/// 
		/// Use this when you want to override the default read preference 
		/// and write concern defined in connection string.
		/// </summary>
		/// <typeparam name="T">Type of repository</typeparam>
		/// <param name="settings">The settings to override.</param>
		/// <returns></returns>
		IMongoRepository<T> Create<T>(MongoCollectionSettings settings) where T : class, new();
	}
}
