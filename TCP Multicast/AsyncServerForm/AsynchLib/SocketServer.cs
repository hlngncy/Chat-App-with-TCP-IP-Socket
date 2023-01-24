using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;

namespace AsynchLib
{
    public class SocketServer
    {
        
        IPAddress mIP;
        int mPort;
        string mHost;
        TcpListener mTcpListener;
        public EventHandler<ClientConnectedEventArgs> RaiseClientConnectedEvent;
        public EventHandler<TextReceivedEventArgs> RaiseTextReceivedEvent;
        List<TcpClient> mClients;
        
        protected virtual void OnRaiseClientConnectedEvent(ClientConnectedEventArgs e)
        {
            EventHandler<ClientConnectedEventArgs> handler = RaiseClientConnectedEvent;
            if(handler != null)
                handler(this, e);
        }

        protected virtual void OnRaiseTextReceivedEvent(TextReceivedEventArgs trea)
        {
            EventHandler<TextReceivedEventArgs> handler = RaiseTextReceivedEvent;
            if (handler != null)
                handler(this, trea);
        }
        public bool KeepRunning { get; set; }

        public SocketServer()
        {
            mClients = new List<TcpClient>();
        }
        public async void StartListeningForIncomingConnection(IPAddress ipaddr = null, int port = 6622)
        {
            if(ipaddr == null)
                ipaddr = IPAddress.Any;
            if (port <= 0)
                port = 6622;

            mIP = ipaddr;
            mPort = port;

            Debug.WriteLine(string.Format("IP Address : {0} \n Port: {1}",mIP.ToString(),mPort));

            mTcpListener = new TcpListener(mIP, mPort);
            if (!KeepRunning)
            {
                try
                {

                    mTcpListener.Start();
                    KeepRunning = true;
                    while (KeepRunning)
                    {
                        var paramClient = await mTcpListener.AcceptTcpClientAsync();
                        mClients.Add(paramClient);
                        Debug.WriteLine($"Client connected successfully: {paramClient.ToString()} Info: {paramClient.ToString()}\n Count: {mClients.Count}");
                        TakeCareofTcpClient(paramClient);
                        ClientConnectedEventArgs eaClientConnected = new ClientConnectedEventArgs(
                            paramClient.Client.RemoteEndPoint.ToString());
                        OnRaiseClientConnectedEvent(eaClientConnected);
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.ToString());
                }
            }
            
            
        }

        public void StopServer()
        {
            try
            {
                
                if (mTcpListener != null)
                    mTcpListener.Stop();
                foreach (TcpClient c in mClients)
                {
                    c.Close();
                }
                mClients.Clear();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private async void TakeCareofTcpClient(TcpClient paramClient)
        {
            NetworkStream stream = null;
            StreamReader reader = null;
            try
            {
                stream = paramClient.GetStream();
                reader = new StreamReader(stream);
                char[] buffer = new char[64];
                int nRet;
                bool nameControl = true;
                string cName = paramClient.ToString();
                while (KeepRunning)
                {
                    Debug.WriteLine("\n***Ready to read.");
                    nRet = await reader.ReadAsync(buffer, 0, buffer.Length);
                    Debug.WriteLine($"Returned: {nRet}");
                    
                    if (nRet == 0)
                    {
                        Debug.WriteLine($"Socket Disconnected");
                        RemoveClient(paramClient);
                        break;
                    }
                    string receivedText = new string(buffer,0,nRet);
                    
                    if (nameControl)
                    {
                        cName = receivedText;
                        Array.Clear(buffer, 0, buffer.Length);
                        KeepRunning = true;
                        nameControl = false;
                        continue;
                    }
                    sendToAll(receivedText, cName);
                    Debug.Write($"***RECEİVED: {receivedText}\n");
                    string cIP = paramClient.Client.RemoteEndPoint.ToString();
                    OnRaiseTextReceivedEvent(new TextReceivedEventArgs(
                        cIP,receivedText,cName
                        ));
                    Array.Clear(buffer, 0, buffer.Length);
                    KeepRunning = true;
                }
            }
            catch (Exception e)
            {   
                RemoveClient(paramClient);
                Debug.WriteLine(e.ToString());
            }
        }

        private void RemoveClient(TcpClient paramClient)
        {
            if (mClients.Contains(paramClient))
            {
                mClients.Remove(paramClient);
                Debug.WriteLine($"Client removed, count: {mClients.Count}");
            }
        }

        public async void sendToAll(string message, string paramClient)
        {
            
            if (string.IsNullOrEmpty(message))
            {
                return;
            }
            try
            {
                byte[] buffMessage = Encoding.ASCII.GetBytes($"From: {paramClient}\n{message}");
                foreach (TcpClient c in mClients)
                {
                    await c.GetStream().WriteAsync(buffMessage, 0 , buffMessage.Length);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }
    }
}
