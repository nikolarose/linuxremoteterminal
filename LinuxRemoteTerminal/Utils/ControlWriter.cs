using System.IO;
using System.Text;
using System.Windows.Forms;

namespace LinuxRemoteTerminal
{
    public class ControlWriter : TextWriter
    {
        private Control richTextBox;
        public ControlWriter(Control richTextBox)
        {
            this.richTextBox = richTextBox;
        }

        public override void Write(char value)
        {
            richTextBox.Invoke((MethodInvoker)delegate
            {
                richTextBox.Text += value.ToString();
            });
        }

        public override void Write(string value)
        {
            richTextBox.Invoke((MethodInvoker)delegate
            {
                richTextBox.Text += (value);
            });
        }

        public override Encoding Encoding
        {
            get { return Encoding.UTF8; }
        }
    }
}
