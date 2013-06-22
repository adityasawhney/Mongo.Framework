
namespace Mongo.Framework.Generator
{
	/// <summary>
	/// Type of hash generator
	/// </summary>
	public enum HashType
	{
		Md5,
		Murmur2
	}

	/// <summary>
	/// Interface to generate hash code for given input
	/// </summary>
	public interface IHashGenerator
	{
		string GenerateHash(string input);
	}

	/// <summary>
	/// Factory to get the generator instance for given type
	/// </summary>
	public interface IMongoHashGeneratorFactory
	{
		IHashGenerator Create(HashType type);
	}
}
