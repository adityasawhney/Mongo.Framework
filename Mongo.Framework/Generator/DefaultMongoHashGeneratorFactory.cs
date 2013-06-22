
namespace Mongo.Framework.Generator
{
	/// <summary>
	/// Factory interface implementation
	/// </summary>
	public class DefaultMongoHashGeneratorFactory : IMongoHashGeneratorFactory
	{
		public IHashGenerator Create(HashType type)
		{
			switch (type)
			{
				case HashType.Md5:
					return new Md5HashGenerator();
				case HashType.Murmur2:
					return new Murmur2HashGenerator();
			}
			return null;
		}
	}
}
