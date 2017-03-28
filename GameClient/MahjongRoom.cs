using System;
using System.Collections.Generic;
using System.Text;
using LitJson;
using GameCommon;
using Mahjong;

namespace GameClient
{
    public class MahjongRoom:Room
    {
        public MahjongRoom(string serverIP, Player player, string roomId, Dictionary<string, object> config) : base(serverIP,player,roomId, config)
        {

        }
        /// <summary>
        /// 查找是否有可能对其他玩家出牌做出响应的操作
        /// </summary>
        /// <param name="player">出牌玩家</param>
        /// <param name="cards">所出牌面</param>
        /// <returns>响应操作</returns>
        public override int[] CheckPlay(Player player, JsonData cards)
        {
            //当前玩家响应跳过
            return null;
        }
        /// <summary>
        /// 获得新牌
        /// </summary>
        /// <param name="data"></param>
        public override void ReceivedCard(JsonData data)
        {
            string text = "";
            MahjongHand hand = (MahjongHand)Player.Hand;
            for (int i = 0; i < hand.Cards.Count; i++)
            {
                text += hand.Cards[i].CardNumber.ToString() + ",";
            }
            Console.Write("当前牌面："+ text);
        }
        /// <summary>
        /// 创建麻将手牌对象
        /// </summary>
        /// <returns></returns>
        public override IHand CreateHand()
        {
            return new MahjongHand();
        }
        /// <summary>
        /// 玩家出牌
        /// </summary>
        public override void PlayerPlay()
        {
            Console.Write("输入你要出的牌：");
            int card = int.Parse(Console.ReadLine());
            Play(new int[] {card });
        }
    }
}
