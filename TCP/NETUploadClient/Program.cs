using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NETUploadClient.SyncSocketProtocolCore;
using NETUploadClient.SyncSocketProtocol;
using System.IO;
using System.Threading;
using AsyncSocketServer;

namespace NETUploadClient
{
    class Program
    {
        public static int PacketSize = 32 * 1024;

        static void Main(string[] args)
        {
            ClientGameSocket gameSocket = new ClientGameSocket();
            gameSocket.Connect("127.0.0.1", 9999);
            gameSocket.OnRecvData = OnRecvData;
            Console.WriteLine("Connect Server Success");
            gameSocket.DoActive();
            gameSocket.SendCommand("Login", new Parameter[] {
                new Parameter("Token","1234122223")
            });
            /*
            uploadSocket.DoLogin("admin", "admin");
            Console.WriteLine("Login Server Success");
            Console.WriteLine("Please Input Upload FileName");
            //string fileName = Console.ReadLine();
            string fileName = Path.Combine(Directory.GetCurrentDirectory(), "borlndmm.dll");
            for (int i = 0; i < 3; i++) //发送失败后，尝试3次重发
            {
                if (SendFile(fileName, uploadSocket))
                {
                    Console.WriteLine("Upload File Success");
                    break;
                }
                Thread.Sleep(10 * 1000); //发送失败等待10S后重连
            }*/
            Console.ReadKey();
        }
        static bool OnRecvData(IncomingDataParser data)
        {

            return true;
        }
    }
}
