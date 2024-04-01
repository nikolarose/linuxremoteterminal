using LinuxRemoteTerminal.Forms;
using LinuxRemoteTerminal.mysql;
using System;
using System.Drawing;
using System.Security.Cryptography;
using System.Windows.Forms;

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
            label4.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            label4.Hide();
            errorProvider1.Clear();
            if (textBox1.Text.Equals("") || textBox2.Text.Equals(""))
            {
                errorProvider1.SetError(button1, "Prosím, vyplňte všechna pole!");
                label4.Show();
                return;
            }
            var username = textBox1.Text;
            var password = "";
            SHA256 sha256 = SHA256.Create();
            password = BitConverter.ToString(sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(textBox2.Text))).Replace("-", "");
            if (_connector.ValidateLogIn(username, password))
            {
                ConsoleForm form = new ConsoleForm(textBox1.Text);
                form.Show();
                Hide();
            }
            else
            {
                errorProvider1.SetError(button1, "Přihlášení selhalo");
                label4.Show();
                textBox1.Clear();
                textBox2.Clear();
            }

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
            RegisterForm registerForm = new RegisterForm();
            registerForm.Show();
            Hide();
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            PasswordRecovery recovery = new PasswordRecovery();
            recovery.ShowDialog();
        }
    }
}