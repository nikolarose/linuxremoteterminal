using System;
using System.Drawing;
using System.Windows.Forms;

namespace LinuxRemoteTerminal
{
    public partial class RegisterForm : Form
    {
        private bool isDragging;
        private Point offset;
        public RegisterForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Equals("") || textBox2.Text.Equals(""))
            {
                MessageBox.Show("Please fill in all fields!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            isDragging = true;
            offset = e.Location;
        }

        private void label1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                Point newLocation = PointToScreen(new Point(e.X, e.Y));
                newLocation.Offset(-offset.X, -offset.Y);
                Location = newLocation;
            }
        }

        private void label1_MouseUp(object sender, MouseEventArgs e)
        {
            isDragging = false;
        }
    }
}