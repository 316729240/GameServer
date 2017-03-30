using AsyncSocketServer;
using Common;
using GameCommon;
using LitJson;
using NETUploadClient.SyncSocketProtocolCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
namespace GameClient
{
    public abstract class Room : ClientBaseSocket
    {
        internal Player Player = null;//当前玩家
        Player[] PlayerList = null;//房间中的玩家 
        internal DataProvider DataProvider = new DataProvider();
        /// <summary>
        /// 房间状态 
        /// </summary>
        public bool IsStart = false;
        /// <summary>
        /// 房间id
        /// </summary>
        public string RoomId { get; set; }
        public Room(string serverIP,Player player,string roomId, DataProvider dataProvider)
        {
            DataProvider = dataProvider;
            PlayerList = new Player[DataProvider.Get("playerCount").ToInt()];
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
            LitJson.JsonData data = LitJson.JsonMapper.ToObject(comm.GetString("data"));
            Common.WriteLog("收到命令：" + comm.Command + "  " + comm.GetString("data"));
            if (comm.Command == "RoomPlayerList")//加载当前房间中的用户列表
            {
                #region 进入房间时加载当前玩家列表
                for(int i=0;i< data.Count; i++)
                {
                    int index = data[i]["index"].ValueAsInt();
                    string token = data[i]["token"].ToStr();
                    if (Player.Token == token)
                    {
                        PlayerList[index] = Player;
                        PlayerList[index].Hand = CreateHand();
                        DataProvider.Set("Player",Player);
                    }
                    else
                    {
                        PlayerList[index] = new Player
                        {
                            Name = data[i]["name"].ValueAsString(),
                            Token = data[i]["token"].ValueAsString(),
                            Portrait = data[i]["portrait"].ValueAsString(),
                            Index = data[i]["index"].ValueAsInt(),
                            Status = data[i]["status"].ValueAsInt() == 1,
                            Hand = CreateHand()
                        };
                    }
                }
                #endregion
            }
            else if (comm.Command== "RadioPalyerInfo")//广播玩家信息
            {
                #region 收到其他玩家状态变更
                int index = data["index"].ValueAsInt();
                    PlayerList[index] = new Player
                    {
                        Name = data["name"].ValueAsString(),
                        Token = data["token"].ValueAsString(),
                        Portrait = data["portrait"].ValueAsString(),
                        Index = data["index"].ValueAsInt(),
                        Status= data["status"].ValueAsInt()==1,
                        Hand = CreateHand()
                    };
                if (PlayerList[index].Status)
                {
                    Common.WriteLog(PlayerList[index].Name + "玩家进入");
                }
                else
                {
                    if (IsStart)
                    {
                        Common.WriteLog(PlayerList[index].Name + "玩家掉线");
                    }
                    else
                    {
                        Common.WriteLog(PlayerList[index].Name + "玩家离开");
                    }
                }
                #endregion
            }
            else if(comm.Command== "GameStart")//游戏开始
            {
                IsStart = true;
            }
            else if(comm.Command== "SendCard")//收到新牌
            {
                Player.Hand.AddCards(data.ToIntArr());//加入至手牌中
                ReceivedCard(data.ToIntArr());
            }
            else if(comm.Command== "RadioSendCard")//其他玩家获取手牌信息
            {
                int index = data["index"].ValueAsInt();
                int count = data["count"].ValueAsInt();
                PlayerList[index].Hand.AddCards(count);//加入到手牌
            }
            else if (comm.Command == "RadioPlayerPlay")//其他玩家出牌信息
            {
                int index = data["index"].ValueAsInt();
                int [] cards = data["cards"].ToIntArr();
                DataProvider.Set("PlayerPlayCards", cards);
                DataProvider.Set("NowPlayPlayer", PlayerList[index]);
                int[] operation = CheckPlay(PlayerList[index], cards);
                ResponsePlayerPlay(comm.GetInt("sendNo"), operation);
            }
            else if (comm.Command == "SetPlayerPlay")//设置当前用户可以出牌
            { 
                PlayerPlay();//玩家出牌
            }
        }
        /// <summary>
        /// 检测玩家是否有可能对当前出牌做出响应
        /// </summary>
        /// <param name="player"></param>
        /// <param name="cards"></param>
        public abstract int[] CheckPlay(Player player, int [] cards);
        /// <summary>
        /// 玩家出牌
        /// </summary>
        /// <returns></returns>
        public abstract void PlayerPlay();
        /// <summary>
        /// 出牌
        /// </summary>
        public void Play(int [] cards)
        {
            Player.Hand.Removed(cards);
            this.SendJson("PlayerPlay", cards);
        }
        /// <summary>
        /// 获得新牌
        /// </summary>
        /// <param name="data"></param>
        public abstract void ReceivedCard(int [] data);
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
        void ResponsePlayerPlay(int sendNo,int [] operation)
        {
            if (operation == null)
            {
                this.SendCommand("ResponsePlayerPlay", new Parameter[] {
                        new Parameter("sendNo",sendNo)
                    });
            }
            else { 
            this.SendCommand("ResponsePlayerPlay", new Parameter[] {
                        new Parameter("sendNo",sendNo),
                        new Parameter("data",operation.ToJson())
                    });
            }
        }
    }
}
