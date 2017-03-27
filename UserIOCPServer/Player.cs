using AsyncSocketServer;
using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameServer
{
    /// <summary>
    /// 玩家
    /// </summary>
    public class Player
    {
        /// <summary>
        /// 玩家时入房间后的序号
        /// </summary>
        public int Index = 0;
        /// <summary>
        /// 当前玩家状态
        /// </summary>
        public int Status = 0;
        /// <summary>
        /// Token
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        /// 玩家名字
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 玩家头像
        /// </summary>
        public string Portrait { get; set; }
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
            UserProtocol.SendJson("SendCard", cards);
        }
        /// <summary>
        /// 通知玩家发牌信息
        /// </summary>
        /// <param name="playerIndex">玩家索引</param>
        /// <param name="count">牌数</param>
        public void RadioSendCard(int playerIndex,int count)
        {
            UserProtocol.SendJson("RadioSendCard", new Parameter[] {
                new Parameter("index",playerIndex),
                new Parameter("count",count)
            });
        }
        /// <summary>
        /// 广播玩家信息
        /// </summary>
        /// <param name="player">玩家信息</param>
        /// <param name="status">玩家状态</param>
        public void RadioPalyerInfo(Player player,int status)
        {
            Dictionary<string, object> attr = new Dictionary<string, object>();
            attr["status"] = status;
            attr["token"] = player.Token;
            attr["name"] = player.Name;
            attr["portrait"] = player.Portrait;
            attr["index"] = player.Index;
            UserProtocol.SendJson("RadioPalyerInfo", attr);
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
        public void RadioPlayerPlay(int sendNo,GameCard[] cards)
        {
            SendJson("RadioPlayerPlay", sendNo,cards);
        }
        /// <summary>
        /// 向用户发送带序列的消息
        /// </summary>
        /// <param name="name"></param>
        /// <param name="sendNo"></param>
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
