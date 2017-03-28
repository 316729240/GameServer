using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameServer
{
    public class Card
    {
        public Card(int number,bool isShow)
        {
            CardNumber = number;
            IsShow = isShow;
        }
        /// <summary>
        /// 牌面
        /// </summary>
        public int CardNumber { get; set; }
        /// <summary>
        /// 是否为明牌
        /// </summary>
        public bool IsShow = false;
    }
    /// <summary>
    /// 游戏道具
    /// </summary>
    public interface IGameProp
    {
        /// <summary>
        /// 获取牌组
        /// </summary>
        int [] GetCards(int count);
        /// <summary>
        /// 玩家出牌后等待期它玩家反馈
        /// </summary>
        void WaitPlayer();

        /// <summary>
        /// 获取出牌令的获得者序号
        /// </summary>
        /// <param name="operation">可能操作类型</param>
        int GetPlayToken(Dictionary<int, int[]> operation);

    }

}
