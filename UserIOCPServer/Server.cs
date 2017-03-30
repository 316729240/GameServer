using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using AsyncSocketServer;
using GameServer.Mahjong;
using log4net;

namespace GameServer
{

    public class Server : AsyncSocketServer.AsyncSocketServer
    {

        public Dictionary<string,BaseSocketProtocol> Users = new Dictionary<string,BaseSocketProtocol>();//建立链接用户
        public Dictionary<string, ServerRoom> Rooms = new Dictionary<string, ServerRoom>();
        public Server(int numConnections): base(numConnections)
        {
            //this.SocketTimeOutMS = socketTimeOutMS;
            /*this.Init();
            int port = 9999;
            IPEndPoint listenPoint = new IPEndPoint(IPAddress.Parse("0.0.0.0"), port);
            this.Start(listenPoint);*/
        }
        public void Start(int port)
        {
            Init();
            IPEndPoint listenPoint = new IPEndPoint(IPAddress.Parse("0.0.0.0"), port);
            Start(listenPoint);
        }

        public override void BuildingSocketInvokeElement(AsyncSocketUserToken userToken)
        {
            byte flag = userToken.ReceiveEventArgs.Buffer[userToken.ReceiveEventArgs.Offset];
            if (flag == 2)
                userToken.AsyncSocketInvokeElement = new GameSocketProtocol(this, userToken);
            if (userToken.AsyncSocketInvokeElement != null)
            {
                //Program.Logger.InfoFormat("Building socket invoke element {0}.Local Address: {1}, Remote Address: {2}",
                   // userToken.AsyncSocketInvokeElement, userToken.ConnectSocket.LocalEndPoint, userToken.ConnectSocket.RemoteEndPoint);
            }
        }
        /// <summary>
        /// 获取指定游戏房间对象
        /// </summary>
        /// <param name="roomId"></param>
        /// <returns></returns>
        public ServerRoom GetRoom(string roomId)
        {
            ServerRoom room = null;
            if (Rooms.ContainsKey(roomId))
            {
                room = Rooms[roomId];
            }else
            {
                room=new MahjongRoom (roomId,  2);
                Rooms[roomId] = room;
            }
            return room;
        }
    }
}
