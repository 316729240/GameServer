using AsyncSocketServer;
using NETUploadClient.SyncSocketProtocolCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace GameClient
{
    public class GameRoom : ClientBaseSocket
    {
        /// <summary>
        /// 用户连接标识
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// 用户头像
        /// </summary>
        public string Portait { get; set; }
        /// <summary>
        /// 房间id
        /// </summary>
        public string RoomId { get; set; }
        public GameRoom(string token,string username,string portait,string roomId)
        {
            this.Token = token;
            this.Username = username;
            this.Portait = portait;
            this.RoomId = roomId;
            m_protocolFlag = AsyncSocketServer.ProtocolFlag.Upload;
            this.Connect("127.0.0.1", 9999);
        }
        /// <summary>
        /// 连接成功
        /// </summary>
        public override void ConnectSuccess()
        {
            Common.WriteLog("与服务器连接成功");
            this.DoActive();
            this.SendCommand("Login", new Parameter[] {
                new Parameter("token",Token),
                new Parameter("username",Username),
                new Parameter("portait",Portait)
            });
            this.SendCommand("LoginRoom", new Parameter[] {
                new Parameter("roomId",RoomId)
            });
        }
        /// <summary>
        /// 接收到命令
        /// </summary>
        /// <param name="comm"></param>
        public override void RecvCommand(IncomingDataParser comm)
        {
            LitJson.JsonData data =LitJson.JsonMapper.ToObject(comm.GetString("data"));
            Common.WriteLog("收到命令："+comm.Command);
            if (comm.Command== "RadioPalyerInfo")
            {
            }
        }
        /// <summary>
        /// 是否重新链接
        /// </summary>
        /// <param name="count">第几次链接</param>
        /// <returns></returns>
        public override bool IsReconnect(int count)
        {
            Thread.Sleep(3000);
            if (count < 10)
            {
                Console.WriteLine("尝试重连" + DateTime.Now);
                return true;
            }
            else
            {
                Console.WriteLine("连接超时");
                return false;
            }
        }
    }
}
