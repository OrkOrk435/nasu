using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Net;
namespace Nasu
{
    public partial class Form1 : Form
    {

        public Point mouseLocation;
        string ip;
        string port;
        string message;
        string nick;
        OpenFileDialog fileOpen;
        private static Socket clientSocket;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
            {
                WindowState = FormWindowState.Maximized;
            }
            else
            {
                WindowState = FormWindowState.Normal;
            }
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            mouseLocation = new Point(-e.X, -e.Y);
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point mousePose = Control.MousePosition;
                mousePose.Offset(mouseLocation.X, mouseLocation.Y);
                Location = mousePose;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
            {
                WindowState = FormWindowState.Minimized;
            }
            else
            {
                WindowState = FormWindowState.Normal;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.MaximizedBounds = Screen.FromHandle(this.Handle).WorkingArea;

        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (panel2.Visible == false)
            {
                panel2.Visible = true;
            }
            else
            {
                panel2.Visible = false;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            ip = textBox1.Text;
            port = textBox2.Text;
            nick = textBox5.Text;
            //Here connection function
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            LoopConnect();
            backgroundWorker1.RunWorkerAsync();
            //End of connection function
            panel2.Visible = false;
            textBox4.Enabled = true;
            button6.Enabled = true;
            button7.Enabled = true;
        }

        private void LoopConnect()
        {
            int attempts = 0;
            while (!clientSocket.Connected)
            {
                try
                {
                    attempts++;
                    clientSocket.Connect(IPAddress.Parse(ip), Int32.Parse(port));
                }


                catch (SocketException)
                {
                    NetInfo.Text = "Próbowanie połączenia z serwerem. Prób: " + attempts;
                }
            }
            NetInfo.Text = "Nawiązano połączenie z serwerem";
        }

        private void button6_Click(object sender, EventArgs e)
        {
            message = textBox4.Text + Environment.NewLine;
            byte[] outstream = Encoding.UTF8.GetBytes(nick + ": " + message);
            //Here send of message
            clientSocket.Send(outstream);
            textBox4.Text = "";

        }

        private void button7_Click(object sender, EventArgs e)
        {
            fileOpen = new OpenFileDialog();
            fileOpen = new OpenFileDialog()
            {
                FileName = "Select a text file",
                Title = "Otwórz plik do wysłania"
            };
            if (fileOpen.ShowDialog() == DialogResult.OK)
            {
                byte[] outstream = File.ReadAllBytes(fileOpen.FileName + Environment.NewLine);

            }
        }


        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                var buffer = new byte[2048];
                int received = clientSocket.Receive(buffer, SocketFlags.None);
                if (received == 0) return;
                var data = new byte[received];
                Array.Copy(buffer, data, received);
                string text = Encoding.UTF8.GetString(data);
                textBox3.Text += text;
            }
        }
    }
}
