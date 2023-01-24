using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using AsynchLib;

namespace AsyncClientSocket
{
    class Program
    {
        public static AsyncLibClient client = new AsyncLibClient();
        static string strInputUser = null;

        static void Main(string[] args)
        {
            Console.BackgroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Client started.");
            Console.WriteLine("Please Enter a Valid IP Address: ");
            string strIPAddress = Console.ReadLine();
            Console.WriteLine("Please Enter a Valid Port Number: ");
            string strPortNumber = Console.ReadLine();
            Console.WriteLine("Please enter name (this will shown to other clients.): ");
            string clientName = Console.ReadLine();
            if (!client.SetServerIPAddress(strIPAddress) || !client.SetPortNumber(strPortNumber))
            {
                Console.WriteLine("Invalid IP Address or Port Number.");
                Console.ReadKey();
                return;
            }

            client.ConnectToServer();
            client.SendToServer(clientName);
            Console.WriteLine("Connected to Server. For sending message type and enter.\n For exit type <EXIT>.");
            do
            {
                strInputUser = Console.ReadLine();
                if (strInputUser.Trim() != "<EXIT>")
                {
                    client.SendToServer(strInputUser);
                }
                else if (strInputUser.Equals("<EXIT>"))
                    client.CloseAndDisconnect();

            } while (strInputUser != "<EXIT>");
        }

        
    }
}
