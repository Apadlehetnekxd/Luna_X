using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;

namespace WebView2Example
{
    public partial class Form1 : Form
    {
        private Process cmdProcess;
        private object textBoxOutput;

        public Form1()
        {
            InitializeComponent();
            InitializeTerminal();
            InitializeTerminal2();
        }

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        private const int WM_VSCROLL = 0x0115;
        private const int SB_LINEUP = 0;
        private const int SB_LINEDOWN = 1;

        public string LatestRecievedText { get; private set; }
        public object Timer1 { get; private set; }

        private void InitializeTerminal()
        {
            if (Panel1 == null)
            {
                MessageBox.Show("A panel1 nem található! Győződj meg róla, hogy létezik a Form-on.");
                return;
            }

            TextBox textBoxOutput = new TextBox
            {
                Multiline = true,
                Width = Panel1.Width - 20,
                Height = Panel1.Height - 70,
                Location = new Point(10, 10),
                ReadOnly = true,
                BackColor = Color.Black,
                ForeColor = Color.White,
                Font = new Font("Consolas", 10),
                BorderStyle = BorderStyle.None,
                ScrollBars = ScrollBars.None,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };

            textBoxOutput.MouseWheel += (sender, e) =>
            {
                if (e.Delta > 0)
                {
                    SendMessage(textBoxOutput.Handle, WM_VSCROLL, SB_LINEUP, 0);
                }
                else if (e.Delta < 0)
                {
                    SendMessage(textBoxOutput.Handle, WM_VSCROLL, SB_LINEDOWN, 0);
                }
            };

            Panel1.Controls.Add(textBoxOutput);

            TextBox textBoxInput = new TextBox
            {
                Width = Panel1.Width - 20,
                Height = 30,
                Location = new Point(10, Panel1.Height - 40),
                BackColor = Color.Black,
                ForeColor = Color.Gray,
                Font = new Font("Consolas", 10),
                Text = "Command...",
                BorderStyle = BorderStyle.None,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };

            textBoxInput.GotFocus += (sender, e) =>
            {
                if (textBoxInput.Text == "Command...")
                {
                    textBoxInput.Text = string.Empty;
                    textBoxInput.ForeColor = Color.White;
                }
            };

            textBoxInput.LostFocus += (sender, e) =>
            {
                if (string.IsNullOrWhiteSpace(textBoxInput.Text))
                {
                    textBoxInput.Text = "Command...";
                    textBoxInput.ForeColor = Color.Gray;
                }
            };

            Panel1.Controls.Add(textBoxInput);

            cmdProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            cmdProcess.OutputDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    if (!e.Data.StartsWith("(c) Microsoft Corporation") &&
                        !e.Data.StartsWith("C:\\") &&
                        !e.Data.Contains("Minden jog fenntartva"))
                    {
                        Invoke((MethodInvoker)(() =>
                        {
                            textBoxOutput.AppendText(e.Data + Environment.NewLine);
                            textBoxOutput.SelectionStart = textBoxOutput.Text.Length;
                            textBoxOutput.ScrollToCaret();
                        }));
                    }
                }
            };

            cmdProcess.Start();
            cmdProcess.BeginOutputReadLine();
            cmdProcess.StandardInput.WriteLine("aa.bat");

            textBoxInput.KeyDown += (sender, e) =>
            {
                if (e.KeyCode == Keys.Enter && !string.IsNullOrWhiteSpace(textBoxInput.Text) && textBoxInput.Text != "Command...")
                {
                    cmdProcess.StandardInput.WriteLine(textBoxInput.Text);
                    textBoxInput.Clear();
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }
            };
        }

        private async void LoadWebViewContent(string htmlFile)
        {
            this.Controls.Add(webView2);
            await webView2.EnsureCoreWebView2Async(null);
            string htmlPath = Path.Combine(Application.StartupPath, htmlFile);
            webView2.CoreWebView2.Navigate($"file:///{htmlPath}");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            timer1.Interval = 30;
            timer1.Start();
            LoadWebViewContent("bin/monaco.html");
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            Panel1.Visible = true;
            Panel4.Visible = false;
            guna2Button3.Visible = false;
            webView2.Visible = false;
        }

        private void guna2Button7_Click(object sender, EventArgs e)
        {
            webView2.Visible = true;
            Panel1.Visible = false;
            Panel4.Visible = false;
            guna2Button3.Visible = true;
        }

        private void guna2Button8_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void guna2Button10_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Maximized;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (Opacity < 1)
            {
                Opacity += 0.1;
            }
            else
            {
                timer1.Stop();
            }
        }

        private void guna2Button6_Click(object sender, EventArgs e)
        {
            Process.Start("https://discord.com/oauth2/authorize?client_id=1320367952431808542&response_type=code&redirect_uri=https%3A%2F%2Fdiscord.gg%2FB348Cp8mhy&scope=identify");
        }

        private void guna2Button9_Click(object sender, EventArgs e)
        {
            Application.Restart();
            Environment.Exit(0);
        }
        private void guna2Button2_Click(object sender, EventArgs e)
        {
            Panel4.Visible = true;
            webView2.Visible = false;
            Panel1.Visible = false;
            guna2Button3.Visible = false;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control)
            {
                if (e.KeyCode == Keys.S)
                {
                    SaveText();
                }
                else if (e.KeyCode == Keys.L)
                {
                    LoadText();
                }
            }
        }

        private void SaveText()
        {
            if (webView2 != null)
            {
                webView2.CoreWebView2.ExecuteScriptAsync("SaveTextToFile();");
            }
        }

        private void LoadText()
        {
            if (webView2 != null)
            {
                webView2.CoreWebView2.ExecuteScriptAsync("LoadTextFromFile();");
            }
        }

        public void SetText(string text)
        {
            if (webView2 != null)
            {
                webView2.CoreWebView2.ExecuteScriptAsync($"SetText(\"{HttpUtility.JavaScriptStringEncode(text)}\")");
            }
        }

        public async Task<string> GetText()
        {
            if (webView2 != null)
            {
                var result = await webView2.CoreWebView2.ExecuteScriptAsync("GetText();");

                if (!string.IsNullOrEmpty(result))
                {
                    LatestRecievedText = result;
                }
            }
            return LatestRecievedText;
        }
        private void InitializeTerminal2()
        {
    

            TextBox textBoxOutput2 = new TextBox
            {
                Multiline = true,
                Width = Panel4.Width - 20,
                Height = Panel4.Height - 70,
                Location = new Point(10, 10),
                ReadOnly = true,
                BackColor = Color.Black,
                ForeColor = Color.White,
                Font = new Font("Consolas", 10),
                BorderStyle = BorderStyle.None,
                ScrollBars = ScrollBars.None,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };

  

            Panel4.Controls.Add(textBoxOutput2);

            TextBox textBoxInput = new TextBox
            {
                Width = Panel4.Width - 20,
                Height = 30,
                Location = new Point(10, Panel4.Height - 40),
                BackColor = Color.Black,
                ForeColor = Color.Gray,
                Font = new Font("Consolas", 10),
                Text = "Command...",
                BorderStyle = BorderStyle.None,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };

            textBoxInput.GotFocus += (sender, e) =>
            {
                if (textBoxInput.Text == "Command...")
                {
                    textBoxInput.Text = string.Empty;
                    textBoxInput.ForeColor = Color.White;
                }
            };

            textBoxInput.LostFocus += (sender, e) =>
            {
                if (string.IsNullOrWhiteSpace(textBoxInput.Text))
                {
                    textBoxInput.Text = "Command...";
                    textBoxInput.ForeColor = Color.Gray;
                }
            };

            Panel4.Controls.Add(textBoxInput);

            cmdProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            cmdProcess.OutputDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    if (!e.Data.StartsWith("(c) Microsoft Corporation") &&
                        !e.Data.StartsWith("C:\\") &&
                        !e.Data.Contains("Minden jog fenntartva"))
                    {
                        Invoke((MethodInvoker)(() =>
                        {
                            textBoxOutput2.AppendText(e.Data + Environment.NewLine);
                            textBoxOutput2.SelectionStart = textBoxOutput2.Text.Length;
                            textBoxOutput2.ScrollToCaret();
                        }));
                    }
                }
            };

            cmdProcess.Start();
            cmdProcess.BeginOutputReadLine();
            cmdProcess.StandardInput.WriteLine("lol.bat");

            textBoxInput.KeyDown += (sender, e) =>
            {
                if (e.KeyCode == Keys.Enter && !string.IsNullOrWhiteSpace(textBoxInput.Text) && textBoxInput.Text != "Command...")
                {
                    cmdProcess.StandardInput.WriteLine(textBoxInput.Text);
                    textBoxInput.Clear();
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }
            };
        }


        private async void guna2Button3_Click(object sender, EventArgs e)
        {
            if (webView2 != null)
            {
                var scriptResponse = await webView2.CoreWebView2.ExecuteScriptAsync("monaco.editor.getModels()[0].getValue();");

                if (!string.IsNullOrEmpty(scriptResponse))
                {
                    string monacoText = scriptResponse;
                    string fileFilter = "All Files (*.*)|*.*";

                    if (monacoText.Contains("<html>") || monacoText.Contains("<body>"))
                    {
                        fileFilter = "HTML Files (*.html)|*.html|All Files (*.*)|*.*";
                    }
                    else if (monacoText.Contains("class") || monacoText.Contains("function"))
                    {
                        fileFilter = "JavaScript Files (*.js)|*.js|All Files (*.*)|*.*";
                    }
                    else if (monacoText.Contains("<?php") || monacoText.Contains("?>"))
                    {
                        fileFilter = "PHP Files (*.php)|*.php|All Files (*.*)|*.*";
                    }
                    else if (monacoText.Contains("def ") || monacoText.Contains("import"))
                    {
                        fileFilter = "Python Files (*.py)|*.py|All Files (*.*)|*.*";
                    }
                    else if (monacoText.Contains("/*") || monacoText.Contains("*/"))
                    {
                        fileFilter = "CSS Files (*.css)|*.css|All Files (*.*)|*.*";
                    }

                    using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                    {
                        saveFileDialog.Filter = fileFilter;
                        saveFileDialog.DefaultExt = "txt";

                        if (saveFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            try
                            {
                                File.WriteAllText(saveFileDialog.FileName, monacoText);
                                MessageBox.Show("File successfully saved.");
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Error saving file: {ex.Message}");
                            }
                        }
                    }

                }
            }
        }

    
        
    }
}

