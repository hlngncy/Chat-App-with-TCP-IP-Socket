using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AsynchLib
{
    public class AsyncLibClient
    {
        IPAddress mServerIpAddress;
        int mServerPort;
        TcpClient mClient;
        public AsyncLibClient()
        {
            mClient = null;
            mServerPort = -1;
            mServerIpAddress = null;

        }

        public IPAddress ServerIPAddress
        {
            get
            {
                return mServerIpAddress;
            }
        }

        public int ServerPort
        {
            get
            {
                return mServerPort;
            }
        }

        public bool SetServerIPAddress(string _IPAddressServer)
        {
            IPAddress ipaddr = null;
            if (!IPAddress.TryParse(_IPAddressServer, out ipaddr))
            {
                Console.WriteLine("Invalid server IP supplied.");
                return false;
            }
            mServerIpAddress = ipaddr;
            return true;
        }

        public bool SetPortNumber(string _ServerPort)
        {
            int portNumber = 0;
            if (!int.TryParse(_ServerPort.Trim(), out portNumber) || portNumber<= 0 || portNumber> 65535)
            {
                Console.WriteLine("Invalid port number supplied.");
                return false;
            }
            mServerPort = portNumber;
            return true;
        }

        public async Task SendToServer(string strInputUser)
        {
            if (string.IsNullOrEmpty(strInputUser))
                return;
            if(mClient != null)
                if (mClient.Connected)
                {
                    StreamWriter clientStreamWriter = new StreamWriter(mClient.GetStream());
                    clientStreamWriter.AutoFlush = true;
                    await clientStreamWriter.WriteAsync(strInputUser);
                }
        }

        public void CloseAndDisconnect()
        {
            if(mClient != null)
                if (mClient.Connected)
                {
                    mClient.Close();
                }
        }

        public async Task ConnectToServer()
        {
            if (mClient == null)
                mClient = new TcpClient();
            try
            {
                await mClient.ConnectAsync(mServerIpAddress, mServerPort);
                Console.WriteLine($"Connected to server IP/Port: {mServerIpAddress}/{mServerPort}");
                ReadDataAsync(mClient);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private async Task ReadDataAsync(TcpClient mClient)
        {
            try
            {
                StreamReader clientReader = new StreamReader(mClient.GetStream());
                char [] buffer = new char[1024];
                int count = 0;
                while (true)
                {
                    count = await clientReader.ReadAsync(buffer, 0, buffer.Length);
                    if (count <= 0)
                    {
                        Console.WriteLine("Disconnected.");
                        mClient.Close();
                        break;
                    }
                    string receivedData = new string(buffer, 0, count);
                    Console.WriteLine(receivedData); 
                    Array.Clear(buffer, 0, buffer.Length);

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }   
}
