using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using UserIOCPServer;
namespace tcpserver
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
           // AsyncSocketServer.AsyncSocketServer p = new AsyncSocketServer.AsyncSocketServer(8000);
           Server Server1 = new Server(8000);
           Server1.Start(9999);
           // UserIOCPServer.UserTcpSvr server = new UserIOCPServer.UserTcpSvr(8000);
        }
    }
}
