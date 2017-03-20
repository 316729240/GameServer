using NETUploadClient.Protocol;
using NETUploadClient.SyncSocketProtocol;
using NETUploadClient.Util;
using NETUploadClient.Util.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using TaskCore;

namespace NETUploadClient
{
    public class TCPClientControl
    {

        public event MyEventHandler CommandEvent;
        public event MyEventHandler EventUserOffline;
        public Dictionary<string, string> Config { get; set; }
        private static TCPClientControl _hcc;

        public static TCPClientControl TCPClientControlInstance
        {
            get
            {
                if (_hcc == null)
                {
                    _hcc = new TCPClientControl();
                }
                return _hcc;
            }
        }

        private OutputReceiver outputReceiver;
        private Dictionary<int, ClientXYSocket> _dicuploadSocket;

        private TCPClientControl()
        {
            Config = new Dictionary<string, string>();
            Config.Add("ip", "101.200.73.92");
            Config.Add("port", "9999");
        }

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


     
        public Boolean Login(string userid, long token, string ip, string port)
        {

            return Login(int.Parse (userid),token,ip,port);
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="token"></param>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public Boolean Login(int userid,long token,string ip,string port)
        {
            if (_dicuploadSocket == null)
            {
                _dicuploadSocket = new Dictionary<int, ClientXYSocket>();

            }
            try
            {
                if (!_dicuploadSocket.ContainsKey(userid))
                {
                    _dicuploadSocket.Add(userid, new ClientXYSocket(outputReceiver));

                    _dicuploadSocket[userid].Connect(ip, int.Parse(port));
                    _dicuploadSocket[userid].CommandEvent += new MyEventHandler(CommandEventHandler);
                }
                else
                {

                    _dicuploadSocket[userid].ReConnect();
                }


                Boolean ret = _dicuploadSocket[userid].DoLogin(userid, token);
                if (ret)
                {
                    ThreadPool.QueueUserWorkItem(e => {

                        _dicuploadSocket[userid].RecvCommandContinus();
                    });
                }
                return ret;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                if (outputReceiver != null)
                    outputReceiver.AddNewLine(e.Message);
                return false;

            }

        }

        private void CommandEventHandler(MyEventArgs margs)
        {
            HandleProtocol(margs);
            if (CommandEvent != null)
                CommandEvent(margs);

           
        }

        private void HandleProtocol(MyEventArgs margs)
        {
            if (margs.MyArg.GetType() == typeof(Add))
            {

                MysqliteControl.MysqliteControlInstance.Do((Add)margs.MyArg);
                if (outputReceiver != null)
                    outputReceiver.AddNewLine("增加 " + ((Add)margs.MyArg).GetProtocolString());
            }
           else if (margs.MyArg.GetType() == typeof(Delete))
            {
                MysqliteControl.MysqliteControlInstance.Do((Delete)margs.MyArg);
                if (outputReceiver != null)
                    outputReceiver.AddNewLine("删除 " + ((Delete)margs.MyArg).GetProtocolString());
            }
            else if (margs.MyArg.GetType() == typeof(Notify))
            {
                Notify add = (Notify)margs.MyArg;
                if (outputReceiver != null)
                    outputReceiver.AddNewLine("通知 " + add.GetProtocolString() );
            }
            else if (margs.MyArg.GetType() == typeof(Log))
            {
                Log add = (Log)margs.MyArg;
                if (outputReceiver != null)
                    outputReceiver.AddNewLine("日志 " + add.GetProtocolString());
            }
            else if (margs.MyArg.GetType() == typeof(Replace))
            {
                MysqliteControl.MysqliteControlInstance.Do((Replace)margs.MyArg);
                if (outputReceiver != null)
                    outputReceiver.AddNewLine("替换 " + ((Replace)margs.MyArg).GetProtocolString());
            }
        }

        public Boolean LogOut(string userid, long token)
        {

            return LogOut(int.Parse (userid),token);
        }

        public Boolean LogOut(int userid,long token)
        {
            Boolean ret = false;
            if (_dicuploadSocket != null && _dicuploadSocket.ContainsKey(userid))
            {
                ret = _dicuploadSocket[userid].DoLogout(userid, token);
            }
            if (outputReceiver != null)
            {
                if (ret)
                {
                    outputReceiver.AddNewLine("退出 " + userid);

                }


            }
            return ret;
        }
        public Boolean LogOut(int userid)
        {
            Boolean ret = false;
            if (_dicuploadSocket != null && _dicuploadSocket.ContainsKey(userid))
            {
                ret = _dicuploadSocket[userid].DoLogout(userid, 0);
            }
            if (outputReceiver != null)
            {
                if (ret)
                {
                    outputReceiver.AddNewLine("退出 " + userid);

                }


            }
            return ret;
        }
        public Boolean LogOut(string dicconfig)
        {
            //Dictionary<string, string> dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(dicconfig);
            //Dictionary<string, string> dicserver = JsonConvert.DeserializeObject<Dictionary<string, string>>(dic["server"]);
            //Dictionary<string, string> dicuser = JsonConvert.DeserializeObject<Dictionary<string, string>>(dic["user"]);
            Boolean ret = false;
            //if (_dicuploadSocket != null && _dicuploadSocket.ContainsKey(dicuser["userid"]))
            //{
            //    ret= _dicuploadSocket[dicuser["userid"]].DoLogout(short.Parse(dicuser["userid"]), long.Parse(dicuser["token"]));
            //}
            //if (outputReceiver != null)
            //{
            //    if(ret)
            //    {
            //        outputReceiver.AddNewLine("退出 "+dicuser["userid"]);

            //    }


            //}
                return ret;

        }

        public void DoActive()
        {
            foreach (int userid in _dicuploadSocket.Keys)
            {
                _dicuploadSocket[userid].DoActive();
            }
        }

        #region 动作执行

        /// <summary>
        /// 检查原料
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="actionid"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public Boolean CheckMaterial(int userid, int actionid, ref int code)
        {
            string strsqlcount = string.Format("select count(*) from actionconfig where prjid={0} and topic='2'", actionid);
            int idrefconfigobjnum = int.Parse(DbHelperSQLite.GetSingle(strsqlcount).ToString());
            if (idrefconfigobjnum == 0) { code = (int)CheckActionError.NoConfig; return true; }

            string strsql = BaseActionControl.BaseActionControlInstance .GetMaterialSql(userid, actionid);
            DataSet ds = DbHelperSQLite.Query(strsql);

            if (ds != null && ds.Tables.Count > 0)
            {

                foreach (DataRow datarow in ds.Tables[0].Rows)
                {
                    try
                    {
                        if (int.Parse(datarow["nowval"].ToString()) < int.Parse(datarow["needval"].ToString()))
                        {
                            code = (int)CheckActionError.NotMatchAllConfig;
                            return false;
                        }
                    }
                    catch
                    {
                        code = (int)CheckActionError.Unknow;
                        return false;
                    }
                }
                return true;


            }
            else
            {
                code = (int)CheckActionError.NoMatchConfig;
                return false ;
            }
        }

        /// <summary>
        /// 检查前提条件
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="actionid"></param>
        /// <returns></returns>
        public Boolean CheckPerequisite(int userid, int actionid, ref int code)
        {
            code = 0;
            string strsql = BaseActionControl.BaseActionControlInstance .GetPerequisiteSql(userid, actionid);
            DataSet ds = DbHelperSQLite.Query(strsql);
            string strsqlcount = string.Format("select count(*) from actionconfig where prjid={0} and topic='1'", actionid);
            int idrefconfigobjnum =int.Parse ( DbHelperSQLite.GetSingle(strsqlcount).ToString ());
            if (idrefconfigobjnum == 0) { code = (int)CheckActionError.NoConfig; return true; }

            if (ds != null && ds.Tables.Count > 0)
            {
                int CountConfig = 0;
                foreach (DataRow datarow in ds.Tables[0].Rows)
                {
                    if (datarow["val"].ToString() != "")
                    {
                        CountConfig++;
                    }
                }
                if (idrefconfigobjnum != CountConfig)
                    code = (int)CheckActionError.NotMatchAllConfig;
                return code == 0;

            }
            else
            {
               code = (int)CheckActionError.NoMatchConfig;
                return false ;
            }

        }

        /// <summary>
        /// 检查动作是否可执行
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="actionid"></param>
        /// <returns></returns>
        public Boolean CheckAction(int userid, int actionid, ref int code)
        {
            return CheckPerequisite(userid, actionid, ref code) && CheckMaterial(userid, actionid, ref code);
        }
        #endregion
    }
}
