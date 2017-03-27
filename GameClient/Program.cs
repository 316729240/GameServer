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
            Player player = new Player()
            {
                Name = name,
                Token=token,
                Portrait=""
            };
            Room room = new Room("127.0.0.1:9999", player, "100023");
            Console.ReadKey();
        }
    }
}
