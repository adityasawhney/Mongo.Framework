using System;

namespace Mongo.Framework.Exception
{
	/// <summary>
	/// Exception for case when 2 given types are expect to be same but are not
	/// </summary>
	[Serializable]
	public class TypeMismatchException : System.Exception
	{
		public TypeMismatchException(Type expectedType, Type actualType, string context = null)
			: base(ComposeMessage(expectedType, actualType, context))
		{
			
		}

		private static string ComposeMessage(Type expectedType, Type actualType, string context = null)
		{
			return String.Format("Types dont match expected: {0} actual: {1}. {2}", expectedType, actualType, context ?? "");
		}
	}
}
