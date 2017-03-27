using System;
using System.Collections.Generic;
using System.Text;

namespace GameClient
{
    public class MahjongRoom:Room
    {
        public MahjongRoom(string serverIP, Player player, string roomId) : base(serverIP,player,roomId)
        {

        }
    }
}
