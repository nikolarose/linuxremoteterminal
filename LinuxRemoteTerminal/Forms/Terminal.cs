using LinuxRemoteTerminal.Forms;
using LinuxRemoteTerminal.mysql;
using LinuxRemoteTerminal.Utils;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LinuxRemoteTerminal
{
    public partial class ConsoleForm : Form
    {
        private bool _isDragging;
        private Point _offset;
        SshClient client;
        MyConnector _connector = new MyConnector();
        string user = "";
        bool editMode = false;
        public List<string> output = new List<string>();

        public ConsoleForm(string user)
        {
            InitializeComponent();
            label6.Hide();
            this.user = user;
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            richTextBox1.Text = "";
            richTextBox1.Rtf = "";
            richTextBox1.ReadOnly = true;
            richTextBox1.WordWrap = false;
            richTextBox1.ScrollBars = RichTextBoxScrollBars.Vertical;
            richTextBox1.AllowDrop = false;
            richTextBox1.DetectUrls = false;
            richTextBox1.Multiline = true;
            richTextBox1.Clear();
            label13.Hide();
            textBox1.Enabled = false;
            button1.Enabled = false;
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
            Invoke((MethodInvoker)delegate
            {
                richTextBox1.AppendText(message + Environment.NewLine);
                richTextBox1.ScrollToCaret();
            });
        }

        public async Task ExecuteSshCommandAsync(string command)
        {
            try
            {
                var sshCommand = client.CreateCommand(command);
                var asyncResult = sshCommand.BeginExecute();

                while (!asyncResult.IsCompleted)
                {
                    await Task.Delay(100);
                }

                using (var outputReader = new StreamReader(sshCommand.OutputStream))
                {
                    string output = await outputReader.ReadToEndAsync();
                    sshCommand.EndExecute(asyncResult);
                    PrintToConsole(output);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Equals("clear"))
            {
                richTextBox1.Clear();
                textBox1.Clear();
                return;
            }

            try
            {
                ExecuteSshCommandAsync(textBox1.Text);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Chyba při provádění SSH příkazu: " + ex.Message);
            }

            textBox1.Clear();
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
                        textBox1.Enabled = true;
                        button1.Enabled = true;
                        timer1.Start();
                        label8.Text = await Task.Run(() => GeoUtil.GetCountry(textBox2.Text));
                        label9.Text = PingUtil.getPing(textBox2.Text).ToString();
                        textBox2.Enabled = false;
                        textBox3.Enabled = false;
                        comboBox1.Enabled = false;
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
                timer1.Stop();
                richTextBox1.Clear();
                textBox1.Enabled = false;
                button1.Enabled = false;
                label9.Text = "-";
                label8.Text = "-";
                comboBox1.SelectedIndex = 0;
                textBox2.Enabled = true;
                textBox3.Enabled = true;
                textBox4.Enabled = true;
                comboBox1.Enabled = true;
                textBox5.Enabled = true;

                textBox2.Clear();
                textBox3.Clear();
                textBox4.Clear();
                textBox5.Clear();
                button3.Text = "Spojit";
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button1.PerformClick();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label9.Text = PingUtil.getPing(textBox2.Text).ToString();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!editMode)
            {
                if (comboBox1.SelectedIndex >= 1)
                {
                    textBox2.Text = _connector.getIP(user, comboBox1.Text);
                    textBox3.Text = _connector.getPort(user, comboBox1.Text).ToString();
                    textBox4.Text = _connector.getUsername(user, comboBox1.Text);
                }
            }
            else
            {
                if (comboBox1.SelectedIndex >= 1)
                {
                    ServerEdit edit = new ServerEdit(user, comboBox1.Text, _connector.getIP(user, comboBox1.Text), _connector.getUsername(user, comboBox1.Text), _connector.getPort(user, comboBox1.Text));
                    edit.ShowDialog();
                    button6.PerformClick();
                }
            }

        }

        public void RefreshData()
        {
            var data = _connector.GetData(user);
            comboBox1.DataSource = data;
            comboBox1.Refresh();
            comboBox1.SelectedIndex = 0;
        }

        private void ConsoleForm_Load(object sender, EventArgs e)
        {
            var data = _connector.GetData(user);
            comboBox1.DataSource = data;
            comboBox1.Refresh();
            comboBox1.SelectedIndex = 0;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            ServerAdd server = new ServerAdd(user);
            server.ShowDialog();
            RefreshData();
            comboBox1.SelectedIndex = 0;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (!editMode)
            {
                editMode = true;
                textBox2.Enabled = false;
                textBox3.Enabled = false;
                textBox4.Enabled = false;
                textBox5.Enabled = false;
                button3.Enabled = false;
                label13.Show();
            }
            else
            {
                editMode = false;
                label13.Hide();
                RefreshData();
                textBox2.Enabled = true;
                textBox3.Enabled = true;
                textBox4.Enabled = true;
                textBox5.Enabled = true;
                button3.Enabled = true;
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            RefreshData();
        }
    }
}