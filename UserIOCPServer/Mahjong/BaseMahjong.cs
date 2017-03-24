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
        public GameCard[] GetCards()
        {
            throw new NotImplementedException();
        }

        public int GetPlayToken(Dictionary<int, int[]> operation)
        {
            throw new NotImplementedException();
        }

        public void WaitPlayer()
        {
            throw new NotImplementedException();
        }
    }
}
