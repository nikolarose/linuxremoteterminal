using LinuxRemoteTerminal.mysql;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace LinuxRemoteTerminal.Forms
{
    public partial class ServerEdit : Form
    {
        MyConnector _connector = new MyConnector();
        string user;
        private bool _isDragging;
        private Point _offset;
        public ServerEdit(string user, string relace, string IP, string username, int port)
        {
            InitializeComponent();
            label6.Hide();
            textBox1.Text = IP;
            textBox4.Text = relace;
            textBox2.Text = username;
            textBox3.Text = port.ToString();
            textBox4.Enabled = false;
            this.user = user;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            errorProvider1.Clear();
            label6.Hide();
            if (int.TryParse(textBox3.Text, out int port1))
            {
                _connector.EditData(user, textBox1.Text, textBox2.Text, port1, textBox4.Text);
                Close();
            }
            else
            {
                label6.Show();
                errorProvider1.SetError(button1, "Chybně zadaný port!");
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

        private void button3_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var response = MessageBox.Show("Jste si jisti?", "Smazání serveru", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (response == DialogResult.Yes)
            {
                _connector.RemoveData(user, textBox4.Text);
                Close();
            }
        }
    }
}
