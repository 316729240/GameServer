using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using NETUploadClient.SyncSocketProtocolCore;
using NETUploadClient.SyncSocketProtocol;
using System.IO;
using System.Threading;
//using Newtonsoft.Json;
using NETUploadClient.Util;

namespace NETUploadClient
{
    class Program
    {
        public static int PacketSize = 32 * 1024;

        static void Main(string[] args)
        {
            //Dictionary<string, string> dic = new Dictionary<string, string>();
            //Dictionary<string, string> dicserver = new Dictionary<string, string>();
            //Dictionary<string, string> dicuser = new Dictionary<string, string>();
            //dicserver.Add("ip", "127.0.0.1");
            //dicserver.Add("port", "9999");

            //dic.Add("server", JsonConvert.SerializeObject(dicserver));


            //Boolean ret = false;

            //string token = "4749966201921598120";

            //dicuser.Add("userid", 1 + "");
            //dicuser.Add("token", token + "");

            //dic.Add("user", JsonConvert.SerializeObject(dicuser));

            //System.Console.WriteLine(token);
            //ret = TCPClientControl.TCPClientControlInstance.Login(JsonConvert.SerializeObject(dic));
        
            //Console.ReadKey();
        }

        protected static bool SendFile(string fileName, ClientUploadSocket uploadSocket)
        {
            FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.ReadWrite);
            try
            {
                try
                {
                    long fileSize = 0;
                    if (!uploadSocket.DoUpload("", Path.GetFileName(fileName), ref fileSize))
                        throw new Exception(uploadSocket.ErrorString);
                    fileStream.Position = fileSize;
                    byte[] readBuffer = new byte[PacketSize];
                    while (fileStream.Position < fileStream.Length)
                    {
                        int count = fileStream.Read(readBuffer, 0, PacketSize);
                        if (!uploadSocket.DoData(readBuffer, 0, count))
                            throw new Exception(uploadSocket.ErrorString);
                    }
                    if (!uploadSocket.DoEof(fileStream.Length))
                        throw new Exception(uploadSocket.ErrorString);
                    return true;
                }
                catch (Exception E)
                {
                    Console.WriteLine("Upload File Error: " + E.Message);
                    return false;
                }
            }
            finally
            {
                fileStream.Close();
            }
        }
    }
}
