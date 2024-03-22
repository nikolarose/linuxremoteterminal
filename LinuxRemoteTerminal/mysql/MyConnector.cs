using System.Windows.Forms;
using MySqlConnector;

namespace LinuxRemoteTerminal.mysql
{
    public class MyConnector
    {
        private static string _server = "mysql.thenarbox.cloud";
        private static string _port = "3306";
        private static string _database = "maturita";
        private static string _username = "maturita";
        private static string _password = "etzqsS/@2e@kl0MA";
        
        private static string _connectionString = $"Server={_server};Port={_port};Database={_database};Uid={_username};Pwd={_password};";

        private MySqlConnection _connection;
        private MySqlCommand _command;
        private void InitConnection()
        {
            _connection = new MySqlConnection(_connectionString);
            try
            {
                _connection.Open();
            }
            catch (MySqlException e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        public void PasswordSet(string username, string password)
        {
            InitConnection();
            var query = $"UPDATE users SET password = '{password}' WHERE username = '{username}';";
            _command = new MySqlCommand(query);
            _command.Connection = _connection;
            _command.ExecuteNonQuery();
            _connection.Close();
        }


        public bool PasswordReset(string username, string password, string recovery_code) { 
            InitConnection();
            var query = $"SELECT * FROM users WHERE username = '{username}' AND recovery_code = '{recovery_code}';";
            _command = new MySqlCommand(query);
            _command.Connection = _connection;
            var reader = _command.ExecuteReader();
            if (reader.HasRows)
            {
                PasswordSet(username, password);
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
        
        public void WriteRegister(string username, string password, int recovery_code)
        {
            InitConnection();
            var query = $"INSERT INTO users (username, password, recovery_code) VALUES ('{username}', '{password}', '{recovery_code}');";
            _command = new MySqlCommand(query);
            _command.Connection = _connection;
            _command.ExecuteNonQuery();
            _connection.Close();
        }
    }
}