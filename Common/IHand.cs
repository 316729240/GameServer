using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LitJson;

namespace GameCommon
{
    /// <summary>
    /// 玩家手牌
    /// </summary>
    public interface IHand
    {
        /// <summary>
        /// 向当前手牌列表中添加牌面
        /// </summary>
        /// <param name="data">牌面</param>
        void AddCards(int[] data);
        /// <summary>
        /// 向当彰手牌列表中添加指定数量未知牌面
        /// </summary>
        /// <param name="count">数量</param>
        void AddCards(int count);
        /// <summary>
        /// 从手牌中取指定牌组
        /// </summary>
        /// <param name="cards">牌组</param>
        void Removed(int [] cards);
    }
}
