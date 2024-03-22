using System;
using System.Drawing;
using System.Windows.Forms;

namespace LinuxRemoteTerminal
{
    public partial class ConsoleForm : Form
    {
        private string username;
        private bool _isDragging;
        private Point _offset;
        public ConsoleForm(string username)
        {
            InitializeComponent();
            this.username = username;
        }

        private void menuStrip1_MouseDown(object sender, MouseEventArgs e)
        {
            _isDragging = true;
            _offset = e.Location;
        }

        private void menuStrip1_MouseUp(object sender, MouseEventArgs e)
        {
            _isDragging = false;
        }

        private void menuStrip1_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging)
            {
                Point newLocation = PointToScreen(new Point(e.X, e.Y));
                newLocation.Offset(-_offset.X, -_offset.Y);
                Location = newLocation;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }
    }
}