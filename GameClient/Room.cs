using AsyncSocketServer;
using Common;
using NETUploadClient.SyncSocketProtocolCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace GameClient
{
    public abstract class Room : ClientBaseSocket
    {
        Player Player = null;//当前玩家
        Player[] PlayerList = null;//房间中的玩家 
        /// <summary>
        /// 房间id
        /// </summary>
        public string RoomId { get; set; }
        public Room(string serverIP,Player player,string roomId)
        {
            Player = player;
            this.RoomId = roomId;
            m_protocolFlag = AsyncSocketServer.ProtocolFlag.Upload;
            string[] ip = serverIP.Split(':');
            this.Connect(ip[0],int.Parse(ip[1]));
        }
        /// <summary>
        /// 连接成功
        /// </summary>
        public override void ConnectSuccess()
        {
            Common.WriteLog("与服务器连接成功");
            this.DoActive();
            this.SendCommand("Login", new Parameter[] {
                new Parameter("token",Player.Token),
                new Parameter("username",Player.Name),
                new Parameter("portrait",Player.Portrait)
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
            if (comm.Command== "RadioPalyerInfo")//广播玩家信息
            {
                int index=data["index"].ToInt();
                if (Player.Token == data["token"].ToStr())
                {
                    PlayerList[index] = Player;
                    PlayerList[index].Hand = CreateHand();
                }
                else
                {
                    PlayerList[index] = new Player
                    {
                        Name = data["name"].ToStr(),
                        Token = data["token"].ToStr(),
                        Portrait = data["portrait"].ToStr(),
                        Index = data["index"].ToInt(),
                        Hand = CreateHand()
                    };
                }
            }
            else if(comm.Command== "SendCard")//获取手牌信息
            {

            }
            else if (comm.Command == "RadioPlayerPlay")//广播出牌信息
            {

            }
            else if (comm.Command == "SetPlayerPlay")//设置当前用户可以出牌
            {

            }
        }
        /// <summary>
        /// 创建手牌对象
        /// </summary>
        /// <returns></returns>
        public abstract IHand CreateHand();
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
