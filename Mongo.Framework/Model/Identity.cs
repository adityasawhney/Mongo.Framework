using Mongo.Framework.Generator;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace Mongo.Framework.Model
{
	[MongoCollectionName("identity")]
	public class Identity
	{
		#region Field names

		public const string FN_ID = "_id";
		public const string FN_SEQ = "seq";

		#endregion

		#region Data model

		public IdType Id { get; set; }
		public int SeqNum { get; set; }

		#endregion

		#region Object mapping configuration

		static Identity()
		{
			// Setup the mapping from collection columns to model data members
			BsonClassMap.RegisterClassMap<Identity>(cm =>
			{
				cm.AutoMap();
				cm.SetIgnoreExtraElements(true); // For backward compatibility
				cm.SetIdMember(cm.GetMemberMap(c => c.Id));
				cm.GetMemberMap(c => c.Id)
				  .SetRepresentation(BsonType.Int32);
				cm.GetMemberMap(c => c.SeqNum)
				  .SetElementName(FN_SEQ);
			});
		}

		#endregion
	}
}
