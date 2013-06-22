using System;

namespace Mongo.Framework.Exception
{
	/// <summary>
	/// Exception for invalid command parameters
	/// </summary>
	[Serializable]
	public class InvalidParameterException : System.Exception
	{
		private const string MESSAGE_FORMAT = "Invalid parameter {0}: '{1}' is not a valid {2}.";

		public InvalidParameterException(string message) : base(message)
		{
		}

		public InvalidParameterException(string parameterName, string dataValue, string dataType)
			: base(ComposeMessage(parameterName, dataValue, dataType))
		{
		}

		private static string ComposeMessage(string parameterName, string dataValue, string dataType)
		{
			return String.Format(MESSAGE_FORMAT, parameterName, dataValue, dataType);
		}
	}
}
