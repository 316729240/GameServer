using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using Common;
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

        public virtual bool DoLogin()
        {
            string userName = "";
            string password = "";
            if (m_incomingDataParser.GetValue(ProtocolKey.UserName, ref userName) & m_incomingDataParser.GetValue(ProtocolKey.Password, ref password))
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
                m_outgoingDataAssembler.AddFailure(ProtocolCode.ParameterError, "");
            return DoSendResult();
        }
        /// <summary>
        /// 发送一个json对象
        /// </summary>
        /// <param name="name">命令</param>
        /// <param name="data">数据</param>
        /// <returns></returns>
        public bool SendJson(string name,object data)
        {
            return SendCommand(name,new Parameter[] {
                new Parameter("data",data.ToJson())
            });
        }
        /// <summary>
        /// 发送一个json对象
        /// </summary>
        /// <param name="name">命令</param>
        /// <param name="data">数据</param>
        /// <returns></returns>
        public bool SendJson(string name,Dictionary<string,object> data)
        {
            if (data == null)
            {
                return SendCommand(name,null);
            }
            else { 
            return SendCommand(name, new Parameter[] {
                new Parameter("data",data.ToJson())
            });
            }
        }
        /// <summary>
        /// 命令请求
        /// </summary>
        /// <param name="name">命令名</param>
        /// <param name="para">参数组</param>
        /// <returns></returns>
        public bool SendCommand(string name, Parameter[] para)
        {
            try
            {
                m_outgoingDataAssembler.Clear();
                m_outgoingDataAssembler.AddRequest();
                m_outgoingDataAssembler.AddCommand(name);
                if (para != null) { 
                    for (int i = 0; i < para.Length; i++)
                    {
                        m_outgoingDataAssembler.AddValue(para[i].Name, para[i].Value);
                    }
                }
                DoSendResult();
                bool bSuccess = false;// RecvCommand();
                if (bSuccess)
                {
                    return bSuccess;
                }
                else
                    return false;
            }
            catch (Exception E)
            {
                //记录日志
                //m_errorString = E.Message;
                return false;
            }
        }
        public bool DoActive()
        {
            m_outgoingDataAssembler.AddSuccess();
            return DoSendResult();
        }
    }
}
