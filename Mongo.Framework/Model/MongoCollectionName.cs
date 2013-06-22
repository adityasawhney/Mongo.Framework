using System;
using System.Collections.Concurrent;

namespace Mongo.Framework.Model
{
	/// <summary>
	/// Attribute used to annotate Enities with collection name to which they belong.
	/// By default, when this attribute is not specified, the classname will be used.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, Inherited = true)]
	public class MongoCollectionName : Attribute
	{
		// Cache of collection names for model objects
		private static readonly ConcurrentDictionary<Type, string> COLLECTION_NAMES = new ConcurrentDictionary<Type, string>();

		/// <summary>
		/// Gets the name of the collection.
		/// </summary>
		/// <value>The name of the collection.</value>
		public string Name { get; private set; }

		/// <summary>
		/// Initializes a new instance of the MongoCollectionName class attribute with the desired name.
		/// </summary>
		/// <param name="value">Name of the collection.</param>
		public MongoCollectionName(string value)
		{
			if (string.IsNullOrWhiteSpace(value))
			{
				throw new ArgumentException("Empty collectionname not allowed", "value");
			}

			this.Name = value;
		}

		/// <summary>
		/// Determines the collectionname for T and assures it is not empty
		/// </summary>
		/// <typeparam name="T">The type to determine the collectionname for.</typeparam>
		/// <returns>Returns the collectionname for T.</returns>
		public static string Get<T>() where T : class
		{
			return COLLECTION_NAMES.GetOrAdd(typeof(T), (entitytype) =>
			{
				// Check to see if the object (inherited from Entity) has a CollectionName attribute
				var att = GetCustomAttribute(entitytype, typeof(MongoCollectionName));
				string collectionName = att != null ? ((MongoCollectionName)att).Name : entitytype.Name;

				if (string.IsNullOrEmpty(collectionName))
				{
					throw new ArgumentException("Collection name cannot be empty for this entity");
				}
				return collectionName;
			});
		}
	}
}
