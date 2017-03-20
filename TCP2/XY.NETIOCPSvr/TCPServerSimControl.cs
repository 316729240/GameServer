
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;



namespace AsyncSocketServer
{
    public class TCPServerSimControl
    {
      
        public event MyEventHandler EventUserOffline;
        private Dictionary<long, short> _dicHttpLoginUser;//http端登陆用户
        private Dictionary<long, short> _dicTokenUserid;//tcp端登陆用户
        private Dictionary<short, long> _dicUseridToken;//tcp端登陆用户
        private Dictionary<short, BaseSocketProtocol> _dicTokenAsyncSocketUserToken;

        private OutputReceiver outputReceiver;

        private static TCPServerSimControl _hcc;
        public static TCPServerSimControl TCPServerControlInstance
        {
            get
            {
                if (_hcc == null)
                {
                    _hcc = new TCPServerSimControl();
                }
                return _hcc;
            }
        }

      

        private TCPServerSimControl()
        {
            _dicTokenAsyncSocketUserToken = new Dictionary<short, BaseSocketProtocol>();
            _dicTokenUserid = new Dictionary<long, short>();
            _dicUseridToken = new Dictionary<short, long>();
            _dicHttpLoginUser = new Dictionary<long, short>();
         
        }

        public Boolean HttpLogin(long token, short userid)
        {
            if (!_dicHttpLoginUser.ContainsKey(token))
            {
                _dicHttpLoginUser.Add(token ,userid );
            }
            else
            {
                _dicHttpLoginUser[token] = userid;

            }
            Info("httplogin "+token+","+ userid);
            return true;
        }
      
        /// <summary>
        /// 关联用户和协议对象
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="token"></param>
        /// <param name="sockettoken"></param>
        /// <returns></returns>
        public Boolean AddAsyncSocketUserToken(short userid,long token, BaseSocketProtocol sockettoken)
        {
          
            if (!_dicTokenAsyncSocketUserToken.ContainsKey(userid))
            {
                _dicTokenAsyncSocketUserToken.Add(userid, sockettoken);
               
            }
            else
            {
                _dicTokenAsyncSocketUserToken[userid] = sockettoken;

            }
            if (!_dicUseridToken.ContainsKey(userid))
            {
                if(!_dicTokenUserid.ContainsKey (token))
                _dicTokenUserid.Add(token, userid);
                else
                {

                    _dicTokenUserid[token] = userid;
                }
                _dicUseridToken.Add(userid, token);
            }
            else
            {
                long oldtoken = _dicUseridToken[userid];
                _dicUseridToken[userid] = token;
                if(_dicTokenUserid .ContainsKey (oldtoken))
                _dicTokenUserid.Remove(oldtoken);
                _dicTokenUserid.Add(token, userid);
            }
            return true;

        }
        /// <summary>
        /// 注册输出消息对象
        /// </summary>
        /// <param name="outputReceiver"></param>
        public void RegistOutputReceiver(OutputReceiver outputReceiver)
        {
            this.outputReceiver = outputReceiver;
          

        }

        /// <summary>
        /// 显示信息
        /// </summary>
        /// <param name="msg"></param>
        public void Info(string msg)
        {
            if (outputReceiver != null)
            {
                outputReceiver.AddNewLine(msg);

            }
        }
        /// <summary>
        /// 是否存在某个登陆用户
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Boolean ExistLoinUser(short userid,long token)
        {
            if (_dicHttpLoginUser.ContainsKey(token)&& _dicHttpLoginUser[token] == userid)
            {
                return true;
            }
            return false  ;
        }

        /// <summary>
        /// 下线
        /// </summary>
        /// <param name="UID"></param>
        /// <returns></returns>
        public Boolean LogOut(short userid)
        {
            if (_dicUseridToken.ContainsKey(userid))
            {
                long token = _dicUseridToken[userid];
                _dicTokenUserid.Remove(token);
                _dicUseridToken.Remove(userid);
                BaseSocketProtocol sockettoken = _dicTokenAsyncSocketUserToken[userid];
                sockettoken.Close();
                _dicTokenAsyncSocketUserToken.Remove(userid);
                Info("httpLogOut "  + userid);
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 启动
        /// </summary>
        /// <returns></returns>
        public Boolean Start()
        {
           
            return true;

        }

        public  Configuration OpenConfigFile(string configPath)
        {
            var configFile = new FileInfo(configPath);
            var vdm = new VirtualDirectoryMapping(configFile.DirectoryName, true, configFile.Name);
            var wcfm = new WebConfigurationFileMap();
            wcfm.VirtualDirectories.Add("/", vdm);
            return WebConfigurationManager.OpenMappedWebConfiguration(wcfm, "/");
        }
        public Boolean Start(string config)
        {
          return  Start(OpenConfigFile(config));
        }
        public Boolean Start(Configuration config)
        {
            DateTime currentTime = DateTime.Now;
            log4net.GlobalContext.Properties["LogDir"] = currentTime.ToString("yyyyMM");
            log4net.GlobalContext.Properties["LogFileName"] = "_SocketAsyncServer" + currentTime.ToString("yyyyMMdd");
            Program.Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

          //  Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            //    Configuration config = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
            Program.FileDirectory = config.AppSettings.Settings["FileDirectory"].Value;
            if (Program.FileDirectory == "")
                Program.FileDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Files");
            if (!Directory.Exists(Program.FileDirectory))
                Directory.CreateDirectory(Program.FileDirectory);
            int port = 0;
            if (!(int.TryParse(config.AppSettings.Settings["Port"].Value, out port)))
                port = 9999;
            int parallelNum = 0;
            if (!(int.TryParse(config.AppSettings.Settings["ParallelNum"].Value, out parallelNum)))
                parallelNum = 8000;
            int socketTimeOutMS = 0;
            if (!(int.TryParse(config.AppSettings.Settings["SocketTimeOutMS"].Value, out socketTimeOutMS)))
                socketTimeOutMS = 5 * 60 * 1000;

            Program.AsyncSocketSvr = new AsyncSocketServer(parallelNum);
            Program.AsyncSocketSvr.SocketTimeOutMS = socketTimeOutMS;
            Program.AsyncSocketSvr.Init();
            IPEndPoint listenPoint = new IPEndPoint(IPAddress.Parse("0.0.0.0"), port);
            Program.AsyncSocketSvr.Start(listenPoint);
            Info("server is ready");
            if (outputReceiver != null)
                Program.AsyncSocketSvr.RegistOutputReceiver(outputReceiver);

            return true;

        }
        public Boolean Stop()
        {
            return true;
        }
      

        /// <summary>
        /// 发送消息到客户端
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="content">发送内容</param>
        /// <param name="resultInfo">发送结果</param>
        /// <returns></returns>
        public Boolean Send(short UID, ProtocolBase content,ref ResultInfo resultInfo)
        {
            if (!_dicTokenAsyncSocketUserToken.ContainsKey(UID))
                return false;
            return _dicTokenAsyncSocketUserToken[UID].Send(content );
           
         
        }

       
       
       
    }
}
