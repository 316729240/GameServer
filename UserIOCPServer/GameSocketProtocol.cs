using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;
using AsyncSocketServer;

namespace UserIOCPServer
{
    public class GameSocketProtocol : BaseSocketProtocol
    {
        string token = "";
        HashSet<BaseSocketProtocol> loginUser = new HashSet<BaseSocketProtocol>();//建立链接用户
        public GameSocketProtocol(Server asyncSocketServer, AsyncSocketUserToken asyncSocketUserToken)
            : base(asyncSocketServer, asyncSocketUserToken)
        {
            m_socketFlag = "Game";
        }
        public override bool ProcessCommand(byte[] buffer, int offset, int count) //处理分完包的数据，子类从这个方法继承
        {
            string command = m_incomingDataParser.Command;
            if (command == "Active") return DoActive();
            else if (command == "Login")
            {
                return DoLogin();
            }
            return true;
        }
        /// <summary>
        /// 登记已链接用户
        /// </summary>
        /// <returns></returns>
        public override bool DoLogin()
        {
            m_incomingDataParser.GetValue("Token", ref token);
            Server server = (Server)m_asyncSocketServer;
            if (server.Users.ContainsKey(token))
            {
                GameSocketProtocol SocketProtocol = (GameSocketProtocol)server.Users[token];
                SocketProtocol.Close();
            }
            server.Users[token] = this;
            
            DoSendResult();
            return true;
        }
        /// <summary>
        /// 关闭链接
        /// </summary>
        public override void Close()
        {
            base.Close();
            ((Server)m_asyncSocketServer).Users.Remove(token);
        }
    }
}
