using MySqlConnector;
using System.Collections.Generic;
using System.Windows.Forms;

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

        public void WriteServer(string username, string IP, string user, string port, string name)
        {
            InitConnection();
            string cmd = "INSERT INTO `" + username + "` (`name`, `IPs`, `usrname`, `port`) VALUES ('" + name + "', '" + IP + "', '" + user + "', '" + port + "')";
            MySqlCommand command = new MySqlCommand(cmd, _connection);
            command.ExecuteNonQuery();
            _connection.Dispose();
        }

        public List<string> GetData(string username)
        {
            List<string> output = new List<string>();
            output.Add("-------------------");
            InitConnection();
            string cmd = "SELECT * from " + username + ";";
            MySqlCommand command = new MySqlCommand(cmd, _connection);
            MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                string value = reader.GetString("name");
                output.Add(value);
            }
            _connection.Dispose();
            return output;
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


        public bool PasswordReset(string username, string password, string recovery_code)
        {
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
            string command = "CREATE TABLE `maturita`.`" + username + "` ( `name` TEXT NOT NULL , `IPs` TEXT NOT NULL , `usrname` TEXT NOT NULL , `port` INT NOT NULL ) ENGINE = InnoDB; ";
            _command = new MySqlCommand(query);
            _command.Connection = _connection;
            _command.ExecuteNonQuery();
            _connection.Close();

            InitConnection();
            _command = new MySqlCommand(command);
            _command.Connection = _connection;
            _command.ExecuteNonQuery();
            _connection.Close();
        }

        public string getUsername(string usrname, string relace)
        {
            InitConnection();
            string cmd = "SELECT `usrname` FROM " + usrname + " WHERE name = '" + relace + "';";
            MySqlCommand command = new MySqlCommand(cmd, _connection);
            MySqlDataReader reader = command.ExecuteReader();
            reader.Read();
            string usrnm = (reader[0].ToString());
            _connection.Dispose();

            return usrnm;
        }

        public int getPort(string usrname, string relace)
        {
            InitConnection();
            string cmd = "SELECT `port` FROM " + usrname + " WHERE name = '" + relace + "';";
            MySqlCommand command = new MySqlCommand(cmd, _connection);
            MySqlDataReader reader = command.ExecuteReader();
            reader.Read();
            string port = (reader[0].ToString());
            int port1 = int.Parse(port);
            _connection.Dispose();

            return port1;
        }

        public string getIP(string usrname, string relace)
        {
            InitConnection();
            string cmd = "SELECT `IPs` FROM " + usrname + " WHERE name = '" + relace + "';";
            MySqlCommand command = new MySqlCommand(cmd, _connection);
            MySqlDataReader reader = command.ExecuteReader();
            reader.Read();
            string ipaddr = (reader[0].ToString());
            _connection.Dispose();
            return ipaddr;
        }

        public void EditData(string username, string IP, string user, int port, string name)
        {
            InitConnection();
            string cmd = "UPDATE " + username + " SET `IPs` = '" + IP + "', `usrname` = '" + user + "', `port` = '" + port + "' WHERE `name` = '" + name + "';";
            MySqlCommand command = new MySqlCommand(cmd, _connection);
            command.ExecuteNonQuery();
            _connection.Dispose();
        }

        public void RemoveData(string username, string name)
        {
            InitConnection();
            string cmd = "DELETE FROM " + username + " WHERE `name` = '" + name + "';";
            MySqlCommand command = new MySqlCommand(cmd, _connection);
            command.ExecuteNonQuery();
            _connection.Dispose();
        }

    }
}