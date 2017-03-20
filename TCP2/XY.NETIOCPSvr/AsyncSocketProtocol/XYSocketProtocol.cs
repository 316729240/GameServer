using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;



namespace AsyncSocketServer
{
    public class XYSocketProtocol : BaseSocketProtocol
    {
        private string m_fileName;
        public string FileName { get { return m_fileName; } }
        private FileStream m_fileStream;

        public XYSocketProtocol(AsyncSocketServer asyncSocketServer, AsyncSocketUserToken asyncSocketUserToken)
            : base(asyncSocketServer, asyncSocketUserToken)
        {
            m_socketFlag = "XY";
            m_fileName = "";
            m_fileStream = null;
            lock (m_asyncSocketServer.XYSocketProtocolMgr)
            {
                m_asyncSocketServer.XYSocketProtocolMgr.Add(this);
            }
        }

        public override void Close()
        {
            base.Close();
            m_fileName = "";
            if (m_fileStream != null)
            {
                m_fileStream.Close();
                m_fileStream = null;
            }
            lock (m_asyncSocketServer.XYSocketProtocolMgr)
            {
                m_asyncSocketServer.XYSocketProtocolMgr.Remove (this);
            }
        }
        public override  bool DoLogin(int userid,long token)
        {
            Notify notify = new Notify();
            notify.opuid = userid;
            notify.matter = 0;
            notify.lparam.Add(1);
            if (TCPServerControl.TCPServerControlInstance .ExistLoinUser (userid,token ))
            {


                m_logined = true;
                Program.Logger.InfoFormat("{0} login success", userid);
               
                notify.lparam.Add(1);
                //m_outgoingDataAssembler.AddSuccess();
              
                //m_outgoingDataAssembler.AddValue("para1", (Int32)0);
                //m_outgoingDataAssembler.AddValue("para2", (Int32)0);
                //m_outgoingDataAssembler.AddValue("userid", userid);
                //m_outgoingDataAssembler.AddValue("curChNb", (Int64)0);
             
            }
            else
            {
                m_logined = false;
                notify.lparam.Add(0);
                //m_outgoingDataAssembler.AddFailure(ProtocolCode.UserOrPasswordError, "");
                Program.Logger.ErrorFormat("{0} login failure,password error", userid);
            }
            notify.Property2Buffer();
            //if (m_incomingDataParser.GetValue(ProtocolKey.UserName, ref userName) & m_incomingDataParser.GetValue(ProtocolKey.Password, ref password))
            //{
            //    if (password.Equals(BasicFunc.MD5String("admin"), StringComparison.CurrentCultureIgnoreCase))
            //    {

            //        m_userName = userName;
            //        m_logined = true;
            //        Program.Logger.InfoFormat("{0} login success", userName);
            //    }
            //    else
            //    {
            //       
            //        Program.Logger.ErrorFormat("{0} login failure,password error", userName);
            //    }
            //}
            //else
            //    m_outgoingDataAssembler.AddFailure(ProtocolCode.ParameterError, "");
             DoSendResult(notify.Buffer );
            return m_logined;
        }
        public override bool ProcessCommand(byte[] buffer, int offset, int count) //处理分完包的数据，子类从这个方法继承
        {
            XYSocketCommand command = (XYSocketCommand)BitConverter.ToInt16 (buffer,10); //StrToCommand(m_incomingDataParser.Command);
            m_outgoingDataAssembler.Clear();
            m_outgoingDataAssembler.AddResponse();
            //  m_outgoingDataAssembler.AddCommand(m_incomingDataParser.Command);
            if (!CheckLogined(command)) //检测登录
            {
              
                Program.Logger.InfoFormat("not login ");
                TCPServerControl.TCPServerControlInstance.Info("not login");
                    m_outgoingDataAssembler.AddFailure();
                    m_outgoingDataAssembler.AddValue("para1", (Int32)0);
                    m_outgoingDataAssembler.AddValue("para2", (Int32)0);
                    m_outgoingDataAssembler.AddValue("userid", (short)0);
                    m_outgoingDataAssembler.AddValue("userid", (short)0);
                

                return DoSendResult();
            }
          else  if (command == XYSocketCommand.Login)
            {
                Login lg = new Login();
                lg.CopyBuffer(buffer, 8, BitConverter.ToInt16(buffer, 8));
                lg.Buffer2Property();
                Boolean ret = DoLogin(lg.userid, lg.token);
                if (ret)
                {
                  
                 ret = TCPServerControl.TCPServerControlInstance.AddAsyncSocketUserToken(lg.userid, lg.token,this);
                    if(ret)
                 TCPServerControl.TCPServerControlInstance.Info(string.Format("{0} login success", lg.userid));
                    else { TCPServerControl.TCPServerControlInstance.Info(string.Format("{0} login fail", lg.userid)); }
                }
               
                   return ret;
            }
          else  if (command == XYSocketCommand.Logout)
            {
                //退出
                Loginout lg = new Loginout();
                lg.CopyBuffer(buffer, 8, BitConverter.ToInt16(buffer, 8));
                lg.Buffer2Property();

                Boolean ret = TCPServerControl.TCPServerControlInstance.LogOut(lg.userid);
                Program.Logger.InfoFormat("{0} loginout success", lg.userid);
                TCPServerControl.TCPServerControlInstance.Info(string.Format ("{0} loginout success", lg.userid));
                return ret;

            }
            else if (command == XYSocketCommand.ServerTest )
            {
                Add lg = new Add();

                lg.objId = 1;
                lg.curchnb = 1;
                #region
                m_outgoingDataAssembler.AddValue("protocolId",lg.protocolId);
                m_outgoingDataAssembler.AddValue("objId", lg.objId );
                m_outgoingDataAssembler.AddValue("objType", lg.objType );
                m_outgoingDataAssembler.AddValue("propsCount", (byte)lg.lpi.Count );
                //
                m_outgoingDataAssembler.AddValue("reason", lg.reason );
                m_outgoingDataAssembler.AddValue("opUID", lg.opuid );
                m_outgoingDataAssembler.AddValue("curChNb", lg.curchnb );
                DoSendResult();
                #endregion
                return true; 
            }
            else
            {
                Program.Logger.Error("Unknow command: " + m_incomingDataParser.Command);
                return false;
            }
        }

        public XYSocketCommand StrToCommand(string command)
        {
            if (command.Equals(ProtocolKey.Login, StringComparison.CurrentCultureIgnoreCase))
                return XYSocketCommand.Login;
          
            else
                return XYSocketCommand.None;
        }

        public bool CheckLogined(XYSocketCommand command)
        {
            if ((command == XYSocketCommand.Login) | (command == XYSocketCommand.Active))
                return true;
            else
                return m_logined;
        }

      
    }

    public class XYSocketProtocolMgr : Object
    {
        private List<XYSocketProtocol> m_list;

        public XYSocketProtocolMgr()
        {
            m_list = new List<XYSocketProtocol>();
        }

        public int Count()
        {
            return m_list.Count;
        }

        public XYSocketProtocol ElementAt(int index)
        {
            return m_list.ElementAt(index);
        }

        public void Add(XYSocketProtocol value)
        {
            m_list.Add(value);
        }

        public void Remove(XYSocketProtocol value)
        {
            m_list.Remove(value);
        }
    }
}
