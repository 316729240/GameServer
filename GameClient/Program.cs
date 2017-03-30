using Mahjong;
using System;
using System.Collections.Generic;
using System.Text;
using GameCommon;
using System.Reflection;

namespace GameClient
{
    class Program
    {
        static void Main(string[] args)
        {
            string token = Guid.NewGuid().ToString();
            Console.Write("输入一个名字：");
            string name=Console.ReadLine();
            Player player = new Player()
            {
                Name = name,
                Token=token,
                Portrait=""
            };

            GameCommon.DataProvider dataProvider = new GameCommon.DataProvider();
            //为监听对象添加观察者
            AddObServer(dataProvider, "ReceivedCard", new string[] { "Mahjong.Observer.AnGang", "Mahjong.Observer.Peng" });//收到牌时观察者队列
            AddObServer(dataProvider, "RespondPlayerPlay", new string[] { "Mahjong.Observer.Peng", "Mahjong.Observer.Chi", "Mahjong.Observer.MingGang" });//回应其他玩家出牌时

            dataProvider.Set("playerCount",2);
            MahjongRoom room = new MahjongRoom("127.0.0.1:9999", player, "100023", dataProvider);
            Console.ReadKey();
        }
        static void AddObServer(GameCommon.DataProvider dataProvider, string listeningObject,string [] codeName)
        {
            List<IObserver> ObServerList = new List<IObserver>();
            for(int i=0;i< codeName.Length; i++) {

                    IObserver obj =(IObserver)Assembly.Load("Mahjong").CreateInstance(codeName[i], true, System.Reflection.BindingFlags.Default, null, new object[] { dataProvider }, null, null);
                    if (obj != null)
                    {
                        ObServerList.Add(obj);
                    }else
                    {
                        //加载观察者失败
                    }
            }
            dataProvider.Set("ReceivedCard", ObServerList);
        }
    }
}
