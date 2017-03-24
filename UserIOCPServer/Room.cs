using AsyncSocketServer;
using Mahjong;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameServer
{

    /// <summary>
    /// 基础游戏房间模型 
    /// </summary>
    public class Room
    {
        /// <summary>
        /// 接牌人索引
        /// </summary>
        int PickCardsPlayer = 0;
        int Count = 0;//总人数
        int SendNo = 0;//发送消息序列
        IGameProp Prop = null;//当前房间中的游戏道具
        Player[] PlayerList = null;//房间中的玩家
        public string RoomId { get; set; }
        Dictionary<int, int []> PlayBack = null;
        /// <summary>
        /// 房间状态 
        /// </summary>
        public bool IsStart = false;
        /// <summary>
        /// 房间
        /// </summary>
        /// <param name="prop">游戏道具</param>
        /// <param name="max">最大人数</param>
        public Room(IGameProp prop, int max)
        {
            //房间人数
            PlayerList = new Player[max];
            Prop = prop;
            Count = max;
        }
        /// <summary>
        /// 进入房间
        /// </summary>
        /// <param name="player">玩家</param>
        /// <returns></returns>
        public bool Login(Player player)
        {
            for (int i = 0; i < PlayerList.Length; i++)
            {
                if (PlayerList[i] == null)
                {
                    PlayerList[i] = player;
                    player.Index = i;
                    NoticePlayerInfo();//广播信息
                    return true;
                }
            }
            Start();//开始游戏
            return false;
        }
        /// <summary>
        /// 下线一个玩家
        /// </summary>
        /// <returns></returns>
        public void RemovePlayer(Player player)
        {
            if (IsStart)
            {
                //游戏已经开始
            }else
            {
                //游戏没有开始
            }

        }
        /// <summary>
        /// 开始游戏
        /// </summary>
        void Start()
        {
            Program.Logger.Info("房间"+RoomId+"游戏开始");
            IsStart = true;
            SendCard();
        } 
        /// <summary>
        /// 向指定玩家发牌并通知其它玩家
        /// </summary>
        void SendCard()
        {
            GameCard[] cards = Prop.GetCards();
            for(int i = 0; i < Count; i++)
            {
                if(i== PickCardsPlayer) {
                    PlayerList[PickCardsPlayer].SendCard(cards);
                }else
                {
                    PlayerList[PickCardsPlayer].RadioSendCard(PickCardsPlayer,cards.Length);
                }
            }
            SendPlayToken(PlayerList[PickCardsPlayer]);
            PickCardsPlayer++;
        }
        /// <summary>
        /// 向指定玩家发送出牌令
        /// </summary>
        /// <param name="player"></param>
        void SendPlayToken(Player player)
        {
            player.SendPlayToken();
        }
        /// <summary>
        /// 通知所有玩家用户信息
        /// </summary>
        void NoticePlayerInfo()
        {
            for (int i = 0; i < Count; i++)
            {
                if(PlayerList[i]!=null) PlayerList[i].RadioPalyerInfo(PlayerList);
            }
        }
        /// <summary>
        /// 查找房间中指定token的玩家
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Player GetPlayer(string token)
        {
            for(int i = 0; i < PlayerList.Length; i++)
            {
                if (PlayerList[i].Token == token)
                {
                    return PlayerList[i];
                }
            }
            return null;
        }
        /// <summary>
        /// 接收用户回发数据
        /// </summary>
        public void Accept(string token, IncomingDataParser incomingData)
        {
            string command = incomingData.Command;
            string data = incomingData.GetString("data");
            int sendNo = incomingData.GetInt("sendNo");
            Player player = GetPlayer(token);
            if (player==null) return;
            if (command == "PlayerPlay") DoPlayerPlay(player,data);
            else if(command== "DoPlayerFeedback") DoPlayerFeedback(player, sendNo,data);
        }
        /// <summary>
        /// 处理用户出牌信息
        /// </summary>
        void DoPlayerPlay(Player player,string data)
        {
            //将数据转为 Card 对象
            GameCard[] cards = null;
            //PlayBack = new Dictionary<int, object>();
            SendNo++;
            Prop.WaitPlayer();
            #region 通知其它玩家
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
            for (int i = 0; i < Count; i++)
            {
                if (i != player.Index) PlayerList[i].RadioPlayerPlay(SendNo, cards);
            }
            #endregion
        }
        /// <summary>
        /// 处理用户对其它玩家出牌后的反馈
        /// </summary>
        /// <param name="player"></param>
        /// <param name="data"></param>
        void DoPlayerFeedback(Player player, int sendNo, string data)
        {
            if (sendNo == SendNo)//验证用户反馈是否为针对本次出牌信息
            {
                int [] operation = null;
                //Prop.AcceptPlayerBack(player.Index, operation);
                
                PlayBack[player.Index] = operation;
                if (PlayBack.Count == Count - 1)//收到其他玩家的反馈
                {
                    int tokenIndex=Prop.GetPlayToken(PlayBack);
                    SendPlayToken(PlayerList[tokenIndex]);
                }
            }
        }
    }
}
