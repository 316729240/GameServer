using GameServer;
using System;
using System.Collections.Generic;
using System.Text;
using log4net;
using System.Reflection;
//[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace ConsoleApplicationServer
{
    class Program
    {
        static void Main(string[] args)
        {
            //ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            Server Server1 = new Server(8000);
            Server1.Start(9999);
            //Logger.Info("服务已启动");
        }
    }
}
