
using System;

namespace Mongo.Framework.Exception
{
	/// <summary>
	/// Duplicate document exception
	/// </summary>
	[Serializable]
	public class AlreadyExistsException : System.Exception
	{
		public AlreadyExistsException(string message) : base(message)
		{
		}
	}
}
