using System;
using System.Drawing;
using System.Security.Cryptography;
using System.Windows.Forms;
using LinuxRemoteTerminal.mysql;

namespace LinuxRemoteTerminal
{
    public partial class LoginForm : Form
    {
        private MyConnector _connector = new MyConnector();
        private bool _isDragging;
        private Point _offset;
        public LoginForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Equals("") || textBox2.Text.Equals(""))
            {
                MessageBox.Show("Please fill in all fields!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            var username = textBox1.Text;
            var password = "";
            SHA256 sha256 = SHA256.Create();
            password = BitConverter.ToString(sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(textBox2.Text))).Replace("-", "");
            if (_connector.ValidateLogIn(username, password))
            {
                MessageBox.Show("Logged in!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Invalid credentials!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var exit = MessageBox.Show("Are you sure you want to exit?", "Exit", MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);
            if (exit == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void label1_MouseDown(object sender, MouseEventArgs e)
        {
            _isDragging = true;
            _offset = e.Location;
        }

        private void label1_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging)
            {
                Point newLocation = PointToScreen(new Point(e.X, e.Y));
                newLocation.Offset(-_offset.X, -_offset.Y);
                Location = newLocation;
            }
        }

        private void label1_MouseUp(object sender, MouseEventArgs e)
        {
            _isDragging = false;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            RegisterForm registerForm = new RegisterForm();
            registerForm.Show();
            Hide();
        }
    }
}