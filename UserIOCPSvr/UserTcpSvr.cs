using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
namespace UserIOCPSvr
{
    public class UserTcpSvr : AsyncSocketServer.AsyncSocketServer
    {
        public UserTcpSvr(int numConnections): base(numConnections)
        {
            //this.SocketTimeOutMS = socketTimeOutMS;
            this.Init();
            int port = 9999;
            IPEndPoint listenPoint = new IPEndPoint(IPAddress.Parse("0.0.0.0"), port);
            this.Start(listenPoint);
        }
    }

}
