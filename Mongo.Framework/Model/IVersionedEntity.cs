
namespace Mongo.Framework.Model
{
	/// <summary>
	/// Marker interface to denote that a model/document is versioned
	/// </summary>
	public interface IVersionedEntity
	{
		int Version { get; set; }
	}
}
