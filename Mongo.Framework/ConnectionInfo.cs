
using System.Configuration;

namespace Mongo.Framework
{
	/// <summary>
	/// Captures the connection information for data access instance
	/// </summary>
	public class ConnectionInfo
	{
		public string ConnectionName { get; private set; }
		public string ConnectionString { get; private set; }

		public ConnectionInfo(string connectionName, string connectionString)
		{
			ConnectionName = connectionName;
			ConnectionString = connectionString;
		}

		public bool IsValid()
		{
			return !string.IsNullOrEmpty(ConnectionName) && !string.IsNullOrEmpty(ConnectionString);
		}

		public override string ToString()
		{
			return string.Format("ConnectionName: {0}, ConnectionString: {1}", ConnectionName, ConnectionString);
		}

		public static ConnectionInfo FromAppConfig(string configName)
		{
			var config = ConfigurationManager.ConnectionStrings[configName];
			return new ConnectionInfo(configName, config.ConnectionString);
		}
	}
}
