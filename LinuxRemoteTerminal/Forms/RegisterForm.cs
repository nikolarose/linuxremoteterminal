using LinuxRemoteTerminal.mysql;
using System;
using System.Drawing;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace LinuxRemoteTerminal
{

    public partial class RegisterForm : Form
    {
        Random random = new Random();
        SHA256 sha256 = SHA256.Create();
        MyConnector _connector = new MyConnector();
        private bool _isDragging;
        private Point _offset;
        public RegisterForm()
        {
            InitializeComponent();
            label4.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            label4.Hide();
            if (string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(textBox2.Text))
            {
                errorProvider1.SetError(button1, "Prosím, vyplňte všechna pole!");
                label4.Show();
                return;
            }
            var username = textBox1.Text;
            var password = BitConverter.ToString(sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(textBox2.Text))).Replace("-", "");
            if (_connector.CheckIfUserExists(username))
            {
                errorProvider1.SetError(button1, "Toto uživatelské jméno již existuje. Zvolte si prosím jiné");
                label4.Show();
                textBox1.Clear();
                textBox2.Clear();
                return;
            }
            int recovery_code = random.Next(100000, 1000000);
            MessageBox.Show("Uchovejte tento kód k případné potřebě obnovy hesla!: " + recovery_code, "Reset hesla", MessageBoxButtons.OK, MessageBoxIcon.Information);
            _connector.WriteRegister(username, password, recovery_code);
            LoginForm loginForm = new LoginForm();
            loginForm.Show();
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var exit = MessageBox.Show("Jste si jisti, že chcete aplikaci opustit?", "Vypnutí aplikace", MessageBoxButtons.YesNo,
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
            LoginForm loginForm = new LoginForm();
            loginForm.Show();
            Close();
        }
    }
}