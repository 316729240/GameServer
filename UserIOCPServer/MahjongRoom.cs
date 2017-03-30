using AsyncSocketServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameCommon;
using Mahjong;

namespace GameServer
{

    /// <summary>
    /// 麻将游戏房间模型 
    /// </summary>
    public class MahjongRoom:ServerRoom
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
            SetGameToken(0);
        }
        /// <summary>
        /// 收集到玩家对出牌的反馈时
        /// </summary>
        /// <param name="player">上次出牌人</param>
        public override void DoPlayerFeedback(Player player,Dictionary<int, LitJson.JsonData> operation)
        {
            if (true)//其他玩家无操作时，继续给下一个玩家发牌
            {
                int[] cards = Prop.GetCards(1);
                SendCard(cards);
                SetGameToken();
            }
            else
            {

            }
        }


        /// <summary>
        /// 创建麻将手牌对象
        /// </summary>
        /// <returns></returns>
        public override IHand CreateHand()
        {
            return new MahjongHand();
        }
    }
}
