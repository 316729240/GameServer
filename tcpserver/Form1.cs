using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using GameServer;
using AsyncSocketServer;

namespace tcpserver
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Server Server1 = new Server(8000);
        private void Form1_Load(object sender, EventArgs e)
        {
           // AsyncSocketServer.AsyncSocketServer p = new AsyncSocketServer.AsyncSocketServer(8000);
           Server1.Start(9999);
            //Mahjong.MahjongEnShi mahjong = new Mahjong.MahjongEnShi();
            //mahjong.Shuffling();
           // UserIOCPServer.UserTcpSvr server = new UserIOCPServer.UserTcpSvr(8000);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            foreach(var user in Server1.Users)
            {
                user.Value.SendCommand("text", new Parameter[] {
                    new Parameter("id",user.Key)
                });
            }
        }
    }
}
