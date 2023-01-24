using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AsynchLib
{
    public class ClientConnectedEventArgs :EventArgs
    {
        public string NewClient { get; set; }
        public ClientConnectedEventArgs(string newClient)
        {
            NewClient = newClient;
        }
    }

    public class TextReceivedEventArgs : EventArgs
    {
        public string TextReceived { get; set; }
        public string SenderClient { get; set; }
        public string NameSenderClient { get; set; }
        public TextReceivedEventArgs(string senderClient, string textReceived, string nameSenderClient)
        {
            SenderClient = senderClient.Trim();
            TextReceived = textReceived.Trim();
            NameSenderClient = nameSenderClient.Trim();
        }
    }
}