using Renci.SshNet;
using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LinuxRemoteTerminal
{
    public partial class ConsoleForm : Form
    {
        private bool _isDragging;
        private Point _offset;
        SshClient client;
        public ConsoleForm()
        {
            InitializeComponent();
            label6.Hide();

            richTextBox1.Text = "";
            richTextBox1.Rtf = "";
            richTextBox1.ReadOnly = true;
            richTextBox1.WordWrap = false;
            richTextBox1.ScrollBars = RichTextBoxScrollBars.Vertical;
            richTextBox1.AllowDrop = false;
            richTextBox1.DetectUrls = false;
            richTextBox1.Multiline = true;
            richTextBox1.Clear();
            Console.SetOut(new MultiTextWriter(new ControlWriter(richTextBox1), Console.Out));
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

        public void PrintToConsole(string message)
        {
            message = message.Replace("&#91;", "["); // [ -> [
            message = message.Replace("&#93;", "]"); // ] -> ]

            BeginInvoke((MethodInvoker)delegate
            {
                Console.WriteLine(message);
                richTextBox1.ScrollToCaret();
            });
        }

        public async Task ExecuteSshCommandAsync(string command)
        {
            try
            {
                var sshCommand = client.CreateCommand(command);
                var asyncResult = sshCommand.BeginExecute();

                using (var reader = new StreamReader(sshCommand.OutputStream))
                {
                    while (!asyncResult.IsCompleted)
                    {
                        string line = await reader.ReadLineAsync();
                        if (line != null)
                        {
                            PrintToConsole(line);
                        }
                    }
                }

                var result = sshCommand.EndExecute(asyncResult);
                PrintToConsole(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Chyba při provádění SSH příkazu: " + ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                ExecuteSshCommandAsync(textBox1.Text);
            });
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

        private async void button3_Click(object sender, EventArgs e)
        {
            errorProvider1.Clear();
            label6.Hide();

            if (string.IsNullOrEmpty(textBox2.Text) || string.IsNullOrEmpty(textBox3.Text) || string.IsNullOrEmpty(textBox4.Text) || string.IsNullOrEmpty(textBox5.Text))
            {
                errorProvider1.SetError(button3, "Spojení nemohlo být uskutečněno. Některé údaje nejsou plně vyplněny.");
                label6.Show();
                return;
            }
            else if (!int.TryParse(textBox3.Text, out int port) || port < 1 || port > 65535)
            {
                errorProvider1.SetError(button3, "Spojení nemohlo být uskutečněno. Chyba portu.");
                label6.Show();
                return;
            }
            else if (button3.Text == "Spojit")
            {
                try
                {
                    int fport = int.Parse(textBox3.Text);
                    client = new SshClient(textBox2.Text, fport, textBox4.Text, textBox5.Text);
                    await Task.Run(() =>
                    {
                        try
                        {
                            client.Connect();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Chyba při připojování k serveru: " + ex.Message);
                            return;
                        }
                    });

                    if (client.IsConnected)
                    {
                        Console.WriteLine("Připojeno k serveru: " + textBox2.Text);
                        textBox2.Enabled = false;
                        textBox3.Enabled = false;
                        textBox4.Enabled = false;
                        textBox5.Enabled = false;
                        button3.Text = "Odpojit se";
                    }
                    else
                    {
                        Console.WriteLine("Připojení nebylo úspěšné. Zkontrolujte, zda je heslo správné.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Chyba při inicializaci SSH klienta: " + ex.Message);
                }
            }
            else
            {
                if (client != null && client.IsConnected)
                {
                    client.Disconnect();
                    client.Dispose();
                }

                textBox2.Enabled = true;
                textBox3.Enabled = true;
                textBox4.Enabled = true;
                textBox5.Enabled = true;

                textBox2.Clear();
                textBox3.Clear();
                textBox4.Clear();
                textBox5.Clear();
                button3.Text = "Spojit";
            }
        }
    }
}