using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameServer.Mahjong
{
    /// <summary>
    /// 基础麻将
    /// </summary>
    public class BaseMahjong : IGameProp
    {
        List<int> Cards =new List<int>();
        /// <summary>
        /// 牌类型
        /// </summary>
        int[,] CardType = new int[5, 2] {
            { 9,0},/// 筒
            { 9,1},/// 条
            { 9,2},/// 万
            { 4,3},/// 风
            { 3,4}/// 中发白
        };
        public BaseMahjong()
        {
            Init(new int []{ 0, 1, 2});
        }
        /// <summary>
        /// 初始化牌堆
        /// </summary>
        /// <param name="type">牌型 0筒 1条 2万 4风 5中发白</param>
        void Init(int [] type)
        {
            Cards.Clear();
            for (int i = 0; i < type.Length; i++)
            {
                for(int n = 0; n < 4; n++) { 
                    for(int i1 = 1; i1 <= CardType[type[i], 0] ; i1++)
                    {
                        Cards.Add(i1*10+ CardType[type[i], 1]);
                    }
                }
            }
            Shuffling();
        }

        /// <summary>
        /// 洗牌
        /// </summary>
        void Shuffling()
        {
            for (int i = Cards.Count - 1; i > 0; i--)
            {
                int card = Cards[i];
                Random rnd = new Random();
                int index = rnd.Next(i - 1);
                Cards[i] = Cards[index];
                Cards[index] = card;
            }
        }

        /// <summary>
        /// 从头部开始获取指定数量的牌
        /// </summary>
        /// <param name="count">获取数量</param>
        /// <returns></returns>
        public int[] GetCards(int count)
        {
            int[] cards = new int[count];
            Cards.CopyTo(0,cards,0,count);
            Cards.RemoveRange(0,count);
            return cards;
        }

        public int GetPlayToken(Dictionary<int, int[]> operation)
        {
            throw new NotImplementedException();
        }

        public void WaitPlayer()
        {
            //throw new NotImplementedException();
        }
    }
}
