using System;
using System.Collections.Generic;
using System.Text;

namespace GameClient
{
    class Program
    {
        static void Main(string[] args)
        {
            string token = Guid.NewGuid().ToString();
            Console.Write("输入一个名字：");
            string name=Console.ReadLine();
            GameRoom room = new GameRoom(token, name, "", "100023");
            Console.ReadKey();
        }
    }
}
