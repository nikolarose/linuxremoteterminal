using System;
using System.Windows.Forms;

namespace LinuxRemoteTerminal
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Equals("") || textBox2.Text.Equals(""))
            {
                MessageBox.Show("Please fill in all fields!");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
        }
    }
}