using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;
using AsyncSocketServer;

namespace GameServer
{
    public class GameSocketProtocol : BaseSocketProtocol
    {
        /// <summary>
        /// 头像 
        /// </summary>
        string Portait = "";
        /// <summary>
        /// 用户登录标识
        /// </summary>
        string Token = "";
        /// <summary>
        /// 当前玩家所在房间
        /// </summary>
        ServerRoom GameRoom = null;
        //HashSet<BaseSocketProtocol> loginUser = new HashSet<BaseSocketProtocol>();//建立链接用户
        public GameSocketProtocol(Server asyncSocketServer, AsyncSocketUserToken asyncSocketUserToken)
            : base(asyncSocketServer, asyncSocketUserToken)
        {
            m_socketFlag = "Game";
        }
        public override bool ProcessCommand(byte[] buffer, int offset, int count) //处理分完包的数据，子类从这个方法继承
        {
            string command = m_incomingDataParser.Command;
            if (GameRoom != null)
            {
                try
                {
                    //当用户在房间时，将消息转入房间处理
                    GameRoom.Accept(Token, m_incomingDataParser);
                }catch
                {
                    Program.Logger.ErrorFormat("房间处理协议出错,command:"+ m_incomingDataParser.Command);
                }
            }
            if (command == "Active") DoActive();
            else if (command == "Login") DoLogin();
            else if (command == "LoginRoom") DoLoginRoom();
            return true;
        }
        /// <summary>
        /// 登记已链接用户
        /// </summary>
        /// <returns></returns>
        public override bool DoLogin()
        {
            m_incomingDataParser.GetValue("token", ref Token);
            m_incomingDataParser.GetValue("username", ref m_userName);
            m_incomingDataParser.GetValue("portait", ref Portait);
            Program.Logger.Info("用户连接信息:"+ m_userName + ":"+Token);
            Server server = (Server)m_asyncSocketServer;
            if (server.Users.ContainsKey(Token))
            {
                GameSocketProtocol SocketProtocol = (GameSocketProtocol)server.Users[Token];
                SocketProtocol.Close();
            }
            server.Users[Token] = this;
            return true;
        }
        /// <summary>
        /// 关闭链接
        /// </summary>
        public override void Close()
        {
            base.Close();
            ((Server)m_asyncSocketServer).Users.Remove(Token);
            if (GameRoom != null) GameRoom.RemovePlayer(Token);
        }
        void DoLoginRoom()
        {
            //int count= m_incomingDataParser.GetInt("Count");
            string roomId=m_incomingDataParser.GetString("roomId").Trim();
            Program.Logger.Info(UserName + "进入房间:" + roomId);
            if (roomId == "") return;
            Server server = (Server)m_asyncSocketServer;
            GameRoom = ((Server)m_asyncSocketServer).GetRoom(roomId);
            GameRoom.Login(new Player (this.Token, this.UserName,this.Portait,this));
        }
    }
}
