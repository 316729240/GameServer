using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;



namespace AsyncSocketServer
{
    public class BaseSocketProtocol : AsyncSocketInvokeElement
    {
        protected string m_userName;
        public string UserName { get { return m_userName; } }
        protected bool m_logined;
        public bool Logined { get { return m_logined; } }
        protected string m_socketFlag;
        public string SocketFlag { get { return m_socketFlag; } }

        public BaseSocketProtocol(AsyncSocketServer asyncSocketServer, AsyncSocketUserToken asyncSocketUserToken)
            : base(asyncSocketServer, asyncSocketUserToken)
        {
            m_userName = "";
            m_logined = false;
            m_socketFlag = "";
        }
        public virtual bool DoLogin(int userid, long token)
        {
            return false;
        }
        public virtual  bool DoLogin()
        {
            string userName = "";
            string password = "";
            if (m_incomingDataParser.GetValue(ProtocolKey.UserID, ref userName) & m_incomingDataParser.GetValue(ProtocolKey.Password, ref password))
            {
                if (password.Equals(BasicFunc.MD5String("admin"), StringComparison.CurrentCultureIgnoreCase))
                {
                    m_outgoingDataAssembler.AddSuccess();
                    m_userName = userName;
                    m_logined = true;
                    Program.Logger.InfoFormat("{0} login success", userName);
                }
                else
                {
                    m_outgoingDataAssembler.AddFailure(ProtocolCode.UserOrPasswordError, "");
                    Program.Logger.ErrorFormat("{0} login failure,password error", userName);
                }
            }
            else
            {
                m_outgoingDataAssembler.AddFailure(ProtocolCode.ParameterError, "");
            }
            return DoSendResult();
        }

        public bool DoActive()
        {
            m_outgoingDataAssembler.AddSuccess();
            return DoSendResult();
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="pb"></param>
        /// <returns></returns>
        public bool Send(ProtocolBase pb)
        {
            pb.Property2Buffer();

            Program.Logger.InfoFormat("send {0}", DataTool.DataToolInstance.ToHexString(pb.Buffer));
            return DoSendResult(pb.Buffer );
        }
    }
}
