using AsyncSocketServer;
using Common;
using GameCommon;
using LitJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameServer
{

    /// <summary>
    /// 基础游戏房间模型 
    /// </summary>
    public abstract class ServerRoom
    {
        int NowCount = 0;//当前人数
        /// <summary>
        /// 游戏令牌
        /// </summary>
        internal int GameToken = -1;
        internal int Count = 0;//总人数
        int SendNo = 0;//发送消息序列
        internal IGameProp Prop = null;//当前房间中的游戏道具
        internal Player[] PlayerList = null;//房间中的玩家
        public string RoomId { get; set; }
        Dictionary<int, JsonData> PlayBack = null;
        /// <summary>
        /// 房间状态 
        /// </summary>
        public bool IsStart = false;
        /// <summary>
        /// 房间
        /// </summary>
        /// <param name="prop">游戏道具</param>
        /// <param name="max">最大人数</param>
        public ServerRoom(string roomId, int max)
        {
            RoomId = roomId;
            //房间人数
            PlayerList = new Player[max];
            Count = max;
        }
        /// <summary>
        /// 进入房间
        /// </summary>
        /// <param name="player">玩家</param>
        /// <returns></returns>
        public void Login(Player player)
        {
            
            for (int i = 0; i < PlayerList.Length; i++)
            {
                if (IsStart)//游戏已开始
                {
                    #region 断线重链
                    if (PlayerList[i].Token == player.Token)
                    {
                        PlayerList[i] = player;
                        player.Index = i;
                        player.Status = true;//上线
                        player.Hand = CreateHand();//创建手牌对象
                        NoticePlayerInfo(player);//广播信息
                        player.RecoveryPlayerData();
                        return;
                    }
                    #endregion
                }
                else//游戏未开始
                {
                    if (PlayerList[i] == null)
                    {
                        PlayerList[i] = player;
                        player.Index = i;
                        player.Status = true;//上线
                        NoticePlayerInfo(player);//广播信息
                        player.Hand = CreateHand();//创建手牌对象
                        NowCount++;
                        //当房间玩家到齐后开始游戏
                        if (NowCount == Count) Start();//开始游戏
                        return;
                    }
                }
            }
            player.RoomRefuse();
        }
        /// <summary>
        /// 下线一个玩家
        /// </summary>
        /// <returns></returns>
        public void RemovePlayer(string token)
        {
                for (int i = 0; i < PlayerList.Length; i++)
                {
                    if (PlayerList[i].Token == token)
                    {
                        PlayerList[i].Status = false;
                        NoticePlayerInfo(PlayerList[i]);//广播信息
                        if (!IsStart)//游戏没有开始直接移除玩家
                        {
                            PlayerList[i] = null;
                            NowCount--;
                        }
                        return;
                    }
                }

        }
        /// <summary>
        /// 开始游戏
        /// </summary>
         void Start()
        {
            NoticePlayerGameStart();
            Program.Logger.Info("房间"+RoomId+"游戏开始");
            IsStart = true;
            GameStart();
        }
        /// <summary>
        /// 游戏开始
        /// </summary>
        public abstract void GameStart();
        /// <summary>
        /// 向指定玩家发牌并通知其它玩家
        /// </summary>
        public void SendCard(int[] cards)
        {
            GameToken++;
            GameToken = GameToken % Count;
            //int[] cards = Prop.GetCards();
            for (int i = 0; i < Count; i++)
            {
                if(i== GameToken) {
                    PlayerList[i].SendCard(cards);
                }else
                {
                    PlayerList[i].RadioSendCard(GameToken, cards.Length);
                }
            }
        }
        /// <summary>
        /// 向指定玩家发送出牌令
        /// </summary>
        /// <param name="player"></param>
        public void SendPlayToken(Player player)
        {
            player.SendPlayToken();
        }
        /// <summary>
        /// 通知所有玩家用户信息
        /// </summary>
        void NoticePlayerInfo(Player player)
        {
            player.SendRoomPlayer(PlayerList);
            for (int i = 0; i < Count; i++)
            {
                //通知其他玩家
                if(PlayerList[i]!=null && i!=player.Index) PlayerList[i].RadioPalyerInfo(player);
            }
        }
        /// <summary>
        /// 通知所有玩家游戏开始
        /// </summary>
        void NoticePlayerGameStart()
        {
            for (int i = 0; i < Count; i++)
            {
                PlayerList[i].NoticePlayerGameStart();
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
        public void Accept(string token, IncomingDataParser comm)
        {
            string command = comm.Command;
            LitJson.JsonData data = LitJson.JsonMapper.ToObject(comm.GetString("data"));
            int sendNo = comm.GetInt("sendNo");
            Program.Logger.Debug("收到命令:"+command+" data:"+ comm.GetString("data"));
            Player player = GetPlayer(token);
            if (player==null) return;
            if (command == "PlayerPlay") DoPlayerPlay(player, data);//广播用户出牌信息
            else if (command == "ResponsePlayerPlay")//处理用户反馈
            {
                if (sendNo == SendNo)//验证用户反馈是否为针对本次出牌信息
                {
                    JsonData operation = data;
                    PlayBack[player.Index] = operation;
                    if (PlayBack.Count == Count - 1)//收到其他玩家的反馈
                    {
                        DoPlayerFeedback(player, PlayBack);
                    }
                }
            }
        }
        /// <summary>
        /// 处理用户出牌信息
        /// </summary>
        void DoPlayerPlay(Player player,LitJson.JsonData data)
        {
            PlayBack = new Dictionary<int, JsonData>();
            //将数据转为 Card 对象
            int[] cards = data.ToIntArr();

            player.Hand.Removed(cards);
            SendNo++;
            //Prop.WaitPlayer();
            #region 通知其它玩家
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
            for (int i = 0; i < Count; i++)
            {
                if (i != player.Index) PlayerList[i].RadioPlayerPlay(SendNo, player, cards);
            }
            #endregion
        }
        #region 设置令牌
        public void SetGameToken(int index)
        {
            GameToken = index;
            PlayerList[GameToken].SendPlayToken();
        }
        public void SetGameToken()
        {
            //GameToken++;
            //GameToken = GameToken % Count;
            PlayerList[GameToken].SendPlayToken();
        }
        #endregion
        /// <summary>
        /// 处理用户对其它玩家出牌后的反馈
        /// </summary>
        /// <param name="player"></param>
        /// <param name="data"></param>
        public abstract void DoPlayerFeedback(Player player,Dictionary<int, JsonData> operation);

        /// <summary>
        /// 创建手牌对象
        /// </summary>
        /// <returns></returns>
        public abstract IHand CreateHand();
    }
}
