using MongoDB.Bson.Serialization;

namespace Mongo.Framework.Generator
{
	/// <summary>
	/// Type of id generator
	/// </summary>
	public interface IdType
	{
		string Name { get; }
	}

	/// <summary>
	/// Factory to get an ID generator of given type
	/// </summary>
	public interface IMongoIdGeneratorFactory
	{
		IIdGenerator Create(IdType type);
	}
}
