using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AsynchLib;
namespace AsyncServerForm
{
    public partial class Form1 : Form
    {
        SocketServer mServer;

        public Form1()
        {
            InitializeComponent();
            mServer = new SocketServer();
            mServer.RaiseClientConnectedEvent += HandleClientConnected;
            mServer.RaiseTextReceivedEvent += HandleTextReceived;
        }

        private void btnAcceptIncoming_Click(object sender, EventArgs e)
        {
            mServer.StartListeningForIncomingConnection();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btnStopServer_Click(object sender, EventArgs e)
        {
            mServer.StopServer();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            mServer.StopServer();
        }

        void HandleClientConnected(object sender, ClientConnectedEventArgs ccea)
        {
            txtConsole.AppendText($"[{DateTime.Now}] New Client Connected: {ccea.NewClient}{Environment.NewLine}");
        }

        void HandleTextReceived(object sender, TextReceivedEventArgs trea)
        {
            txtConsole.AppendText(string.Format(
                "[{0}]Received from {1}/{2}: {3}{4}",
                DateTime.Now,
                trea.NameSenderClient,
                trea.SenderClient,
                trea.TextReceived,
                Environment.NewLine
            ));

        }
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            txtConsole.AppendText($"***Bongo cat petted***{Environment.NewLine}");
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
