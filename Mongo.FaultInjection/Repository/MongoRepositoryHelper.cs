using System.Linq;
using System.Reflection;
using Mongo.Framework.DataAccess;
using Mongo.Framework.Repository;

namespace Mongo.FaultInjection.Repository
{
	public static class MongoRepositoryHelper
	{
		public static class Method
		{
			public const string ADD = "Mongo.FaultInjection.Repository.TestMongoRepository<T>.Add(T)";
			public const string SINGLE = "Mongo.FaultInjection.Repository.TestMongoRepository<T>.Single(System.Linq.Expressions.Expression<System.Func<T,System.Boolean>>)";
			public const string FIND_AS = "Mongo.FaultInjection.Repository.TestMongoRepository<T>.FindAs(MongoDB.Driver.IMongoQuery)";
			public const string UPDATE_CAS = "Mongo.FaultInjection.Repository.TestMongoRepository<T>.Update(MongoDB.Driver.IMongoQuery, MongoDB.Driver.IMongoUpdate, MongoDB.Driver.UpdateFlags)";
		}

		public static void DynamicOverride(object dataAccess, FaultHandler handler)
		{
			// Goofy code to change MongoDataAccess to use TestMongoRepositoryFactory implementation instead of default
			// TestMongoRepositoryFactory is a decorator over the default implementation and includes fault injection logic
			PropertyInfo repoFactoryInfo;
			object factories;
			var repoFactory = ExtractRepoFactory(dataAccess, out repoFactoryInfo, out factories);
			var newRepoFactory = new TestMongoRepositoryFactory(repoFactory, handler);
			repoFactoryInfo.SetValue(factories, newRepoFactory, null);
		}

		public static void DynamicRestore(object dataAccess)
		{
			// Change it back to use DefaultMongoRepositoryFactory
			PropertyInfo repoFactoryInfo;
			object factories;
			var repoFactory = ExtractRepoFactory(dataAccess, out repoFactoryInfo, out factories);
			var testRepoFactory = (TestMongoRepositoryFactory)repoFactory;
			repoFactoryInfo.SetValue(factories, testRepoFactory.UnderlyingFactory, null);
		}

		public static IMongoRepositoryFactory GetRepoFactory(object dataAccess)
		{
			PropertyInfo repoFactoryInfo;
			object factories;
			return ExtractRepoFactory(dataAccess, out repoFactoryInfo, out factories);
		}

		private static IMongoRepositoryFactory ExtractRepoFactory(object dataAccess,
																  out PropertyInfo repoFactoryInfo,
																  out object factories)
		{
			const BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
			factories = typeof(MongoDataAccess).GetFields(bindFlags)
											   .First(f => f.FieldType == typeof(IMongoDataAccessAbstractFactory))
											   .GetValue(dataAccess);
			repoFactoryInfo = typeof(DefaultMongoAbstractFactory).GetProperties(bindFlags)
																 .First(f => f.PropertyType == typeof(IMongoRepositoryFactory));
			return (IMongoRepositoryFactory)repoFactoryInfo.GetValue(factories, null);
		}
	}
}
