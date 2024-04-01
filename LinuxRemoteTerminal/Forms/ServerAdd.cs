using LinuxRemoteTerminal.mysql;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace LinuxRemoteTerminal.Forms
{
    public partial class ServerAdd : Form
    {
        private bool _isDragging;
        private Point _offset;
        MyConnector _connector = new MyConnector();
        string user;
        public ServerAdd(string user)
        {
            InitializeComponent();
            label6.Hide();
            this.user = user;
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            _isDragging = true;
            _offset = e.Location;
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging)
            {
                Point newLocation = PointToScreen(new Point(e.X, e.Y));
                newLocation.Offset(-_offset.X, -_offset.Y);
                Location = newLocation;
            }
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            _isDragging = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            label6.Hide();
            errorProvider1.Clear();

            if (String.IsNullOrEmpty(textBox1.Text) || String.IsNullOrEmpty(textBox2.Text) || String.IsNullOrEmpty(textBox3.Text) || String.IsNullOrEmpty(textBox4.Text))
            {
                label6.Show();
                errorProvider1.SetError(button1, "Některé údaje byly chybně vyplněny!");
                return;
            }

            _connector.WriteServer(user, textBox1.Text, textBox2.Text, textBox3.Text, textBox4.Text);
            Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
