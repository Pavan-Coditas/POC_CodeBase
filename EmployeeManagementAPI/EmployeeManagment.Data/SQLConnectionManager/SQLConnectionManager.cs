using System.Data.SqlClient;

namespace EmployeeManagement
{
    public sealed class SQLConnectionManager
    {
        private static SQLConnectionManager? _instance;
        private static readonly object _lockObject = new object();
        private readonly Dictionary<string, SqlConnection> _connections;

        private SQLConnectionManager()
        {
            _connections = new Dictionary<string, SqlConnection>();
        }

        public static SQLConnectionManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lockObject)
                    {
                        if (_instance == null)
                        {
                            _instance = new SQLConnectionManager();
                        }
                    }
                }
                return _instance;
            }
        }

        public SqlConnection GetConnection(string connectionString)
        {
            lock (_lockObject)
            {
                if (_connections.ContainsKey(connectionString))
                {
                    return _connections[connectionString];
                }
                else
                {
                    var connection = new SqlConnection(connectionString);
                    _connections[connectionString] = connection;
                    return connection;
                }
            }
        }
    }
}
