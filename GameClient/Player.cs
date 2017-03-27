using System;
using System.Collections.Generic;
using System.Text;

namespace GameClient
{
    public class Player
    {/// <summary>
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
        /// <summary>
        /// 玩家手牌
        /// </summary>
        public IHand Hand = null;
    }
}
