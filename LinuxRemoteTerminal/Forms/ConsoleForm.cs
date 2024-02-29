using System;
using System.Windows.Forms;

namespace LinuxRemoteTerminal
{
    public partial class ConsoleForm : Form
    {
        private string username;
        public ConsoleForm(string username)
        {
            InitializeComponent();
            this.username = username;
        }
    }
}