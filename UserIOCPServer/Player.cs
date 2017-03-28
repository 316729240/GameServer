using AsyncSocketServer;
using Common;
using GameCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameServer
{
    /// <summary>
    /// 玩家
    /// </summary>
    public class Player:GameCommon.GamePlayer
    {
        public BaseSocketProtocol UserProtocol = null;
        public Player(string token,string name,string portrait,BaseSocketProtocol protocol)
        {
            this.Token = token;
            this.Name = name;
            this.Portrait = portrait;
            UserProtocol = protocol;
        }
        /// <summary>
        /// 向玩家发牌
        /// </summary>
        /// <param name="cards"></param>
        public void SendCard(int [] cards)
        {
            Hand.AddCards(cards);//将新牌加入手牌并通知客户端更新手牌信息
            UserProtocol.SendJson("SendCard", cards);
        }
        /// <summary>
        /// 恢复玩家数据
        /// </summary>
        public void RecoveryPlayerData()
        {
            throw new  Exception("恢复用户数据未做");
        }
        /// <summary>
        /// 通知玩家发牌信息
        /// </summary>
        /// <param name="playerIndex">玩家索引</param>
        /// <param name="count">牌数</param>
        public void RadioSendCard(int playerIndex,int count)
        {
            Dictionary<string, object> attr = new Dictionary<string, object>();
            attr["index"] = playerIndex;
            attr["count"] = count;
            UserProtocol.SendJson("RadioSendCard",attr);
        }
        /// <summary>
        /// 向玩家发送当前房间中的用户列表
        /// </summary>
        /// <param name="playerList"></param>
        internal void SendRoomPlayer(Player[] playerList)
        {
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
            for(int i = 0; i < playerList.Length; i++)
            {
                Player player = playerList[i];
                if (player != null) { 
                Dictionary<string, object> attr = new Dictionary<string, object>();
                attr["status"] = player.Status ? 1 : 0;
                attr["token"] = player.Token;
                attr["name"] = player.Name;
                attr["portrait"] = player.Portrait;
                attr["index"] = player.Index;
                list.Add(attr);
                }
            }
            UserProtocol.SendJson("RoomPlayerList", list);
        }

        /// <summary>
        /// 广播玩家信息
        /// </summary>
        /// <param name="player">玩家信息</param>
        /// <param name="status">玩家状态</param>
        public void RadioPalyerInfo(Player player)
        {
            Dictionary<string, object> attr = new Dictionary<string, object>();
            attr["status"] = player.Status?1:0;
            attr["token"] = player.Token;
            attr["name"] = player.Name;
            attr["portrait"] = player.Portrait;
            attr["index"] = player.Index;
            UserProtocol.SendJson("RadioPalyerInfo", attr);
        }
        /// <summary>
        /// 拒绝进入房间
        /// </summary>
        public void RoomRefuse()
        {
            UserProtocol.SendJson("RoomRefuse", null);
        }

        /// <summary>
        /// 通知游戏开始
        /// </summary>
        public void NoticePlayerGameStart()
        {
            UserProtocol.SendCommand("GameStart", null);
        }
        /// <summary>
        /// 向指定玩家发送出牌令
        /// </summary>
        /// <param name="cards">所出的牌</param>
        public void SendPlayToken()
        {
            UserProtocol.SendCommand("SetPlayerPlay",null);
        }
        
        /// <summary>
        /// 广播玩家出牌信息
        /// </summary>
        /// <param name="cards">所出的牌</param>
        public void RadioPlayerPlay(int sendNo,Player player, int[] cards)
        {
            Dictionary<string, object> attr = new Dictionary<string, object>();
            attr["index"] = player.Index;
            attr["cards"] = cards;
            SendJson("RadioPlayerPlay", sendNo,attr);
        }
        /// <summary>
        /// 向用户发送带序列的消息
        /// </summary>
        /// <param name="name"></param>
        /// <param name="sendNo"></param>
        /// <param name="data"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool SendJson(string name,int sendNo, object data)
        {
            return UserProtocol.SendCommand(name, new Parameter[] {
                new Parameter("sendNo",sendNo),
                new Parameter("data",data.ToJson())
            });
        }

    }
}
