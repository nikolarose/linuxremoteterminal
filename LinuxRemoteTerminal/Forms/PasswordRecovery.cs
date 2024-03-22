using LinuxRemoteTerminal.mysql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LinuxRemoteTerminal.Forms
{
    public partial class PasswordRecovery : Form
    {
        private bool _isDragging;
        private Point _offset;
        SHA256 sha256 = SHA256.Create();
        MyConnector _connector = new MyConnector();
        public PasswordRecovery()
        {
            InitializeComponent();
            label5.Hide();
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

        private void button3_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            label5.Hide();
            if (string.IsNullOrEmpty(textBox1.Text) || string.IsNullOrEmpty(textBox2.Text) || string.IsNullOrEmpty(textBox3.Text))
            {
                label5.Show();
                errorProvider1.SetError(button1, "Prosím, vyplňte všechna pole!");
                return;
            }
            if (!_connector.CheckIfUserExists(textBox1.Text)){
                label5.Show();
                errorProvider1.SetError(button1, "Zadané uživatelské jméno neexistuje!");
                textBox1.Clear();
                textBox2.Clear();
                textBox3.Clear();
                return;
            }
            var password = BitConverter.ToString(sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(textBox2.Text))).Replace("-", "");
            if (!_connector.PasswordReset(textBox1.Text, password, textBox3.Text))
            {
                label5.Show();
                errorProvider1.SetError(button1, "Kombinace uživatelského jména a obnovovacího klíče není platná.");
                textBox1.Clear();
                textBox2.Clear();
                textBox3.Clear();
            }
            else
            {
                label5.Text = "Heslo změněno.";
                label5.BackColor = Color.Green;
                label5.Show();
                textBox1.Clear();
                textBox2.Clear();
                textBox3.Clear();
            }
        }
    }
}
