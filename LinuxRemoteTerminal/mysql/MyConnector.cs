using MySqlConnector;

namespace LinuxRemoteTerminal.mysql
{
    public class MyConnector
    {
        private static string Server = "82.208.16.150";
        private static string Port = "3306";
        private static string Database = "admin_lrt";
        private static string Username = "lrt_mp2024";
        private static string Password = "%665Khky5";
        
        private static string _ConnectionString = $"Server={Server};Port={Port};Database={Database};Uid={Username};Pwd={Password};";

        private MySqlConnection _connection;
        private MySqlCommand _command;
        private void InitConnection()
        {
            _connection = new MySqlConnection(_ConnectionString);
            try
            {
                _connection.Open();
            }
            catch (MySqlException e)
            {
                _connection.Close();
            }
        }
        public bool ValidateLogIn(string username, string password)
        {
            InitConnection();
            var query = $"SELECT * FROM users WHERE username = '{username}' AND password = '{password}';";
            _command = new MySqlCommand(query);
            _command.Connection = _connection;
            var reader = _command.ExecuteReader();
            if (reader.HasRows)
            {
                reader.Close();
                _connection.Close();
                return true;
            }
            reader.Close();
            _connection.Close();
            return false;
        }
        
        public bool CheckIfUserExists(string username)
        {
            InitConnection();
            var query = $"SELECT * FROM users WHERE username = '{username}';";
            _command = new MySqlCommand(query);
            _command.Connection = _connection;
            var reader = _command.ExecuteReader();
            if (reader.HasRows)
            {
                reader.Close();
                _connection.Close();
                return true;
            }
            reader.Close();
            _connection.Close();
            return false;
        }
        
        public void WriteRegister(string username, string password)
        {
            InitConnection();
            var query = $"INSERT INTO users (username, password) VALUES ('{username}', '{password}');";
            _command = new MySqlCommand(query);
            _command.Connection = _connection;
            _command.ExecuteNonQuery();
            _connection.Close();
        }
    }
}