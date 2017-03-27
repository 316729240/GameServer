using AsyncSocketServer;
using Mahjong;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameServer
{

    /// <summary>
    /// 麻将游戏房间模型 
    /// </summary>
    public class MahjongRoom:Room
    {
        /// <summary>
        /// 房间
        /// </summary>
        /// <param name="prop">游戏道具</param>
        /// <param name="max">最大人数</param>
        public MahjongRoom(string roomId, int max): base(roomId,max)
        {
            Prop = new Mahjong.BaseMahjong();
        }



        /// <summary>
        /// 游戏开始
        /// </summary>
        public override void GameStart()
        {
            #region 先给每个玩家发13张手牌
            for (int i = 0; i < Count; i++) {  
                int [] cards=Prop.GetCards(i==0?14:13);
                SendCard(cards);
            }
            #endregion
            PlayerList[0].SendPlayToken();
        }
        /// <summary>
        /// 收集到玩家对出牌的反馈时
        /// </summary>
        /// <param name="player"></param>
        public override void DoPlayerFeedback(Player player)
        {

        }
    }
}
