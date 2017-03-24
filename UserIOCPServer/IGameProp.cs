using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameServer
{
    //游戏中的牌面
    public class GameCard
    {
        public int Type { get; set; }
        public int Value { get; set; }
    }
    /// <summary>
    /// 游戏道具
    /// </summary>
    public interface IGameProp
    {
        /// <summary>
        /// 获取牌组
        /// </summary>
        GameCard [] GetCards();
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
