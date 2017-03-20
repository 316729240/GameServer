using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using NETUploadClient.SyncSocketCore;
using NETUploadClient.Protocol;
using NETUploadClient.Util;
using NETUploadClient.Util.Common;
using NETUploadClient.Protocol;

namespace NETUploadClient.SyncSocketProtocolCore
{
    public class ClientBaseSocket : SyncSocketInvokeElement
    {
        protected string m_errorString;
        public string ErrorString { get { return m_errorString; } }
        protected string m_userName;
        protected string m_password;

        protected int _userid;
        protected long _token;
        public ClientBaseSocket(): base()
        { }
        public ClientBaseSocket(OutputReceiver outputReceiver)
            : base(outputReceiver)
        {
        }

        public bool CheckErrorCode()
        {
            int errorCode = 0;
            m_incomingDataParser.GetValue(ProtocolKey.Code, ref errorCode);
            if (errorCode == ProtocolCode.Success)
                return true;
            else
            {
                m_errorString = ProtocolCode.GetErrorCodeString(errorCode);
                return false;
            }
        }

        public bool DoActive()
        {
            try
            {
                m_outgoingDataAssembler.Clear();
                m_outgoingDataAssembler.AddRequest();
                m_outgoingDataAssembler.AddCommand(ProtocolKey.Active);
                SendCommand();
                return true;
             
            }
            catch (Exception E)
            {
                Console.WriteLine(E .Message );
                //记录日志
                m_errorString = E.Message; 
                return false;
            }
        }

     
        public bool DoLogin(int userid, long token)
        {
            try
            {
               
                m_outgoingDataAssembler.Clear();
                m_outgoingDataAssembler.AddRequest();
                m_outgoingDataAssembler.AddCommand((short)XYSocketCommand.Login);
                m_outgoingDataAssembler.AddValue(ProtocolKey.UserID, userid);
                m_outgoingDataAssembler.AddValue(ProtocolKey.Token, token);
                SendCommand();
                bool bSuccess = RecvCommand();
                if (bSuccess)
                {
                    bSuccess = CheckErrorCode();
                    if (bSuccess)
                    {
                        _userid = userid;
                        _token  = token;
                    }
                    return bSuccess;
                }
                else
                    return false;
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
                //记录日志
                m_errorString = E.Message;
                return false;
            }
        }

        public bool DoLogout(int userid, long token)
        {
            try
            {
              
                m_outgoingDataAssembler.Clear();
                m_outgoingDataAssembler.AddRequest();
                m_outgoingDataAssembler.AddCommand((short)XYSocketCommand.Logout );
                m_outgoingDataAssembler.AddValue(ProtocolKey.UserID, userid);
                m_outgoingDataAssembler.AddValue(ProtocolKey.Token, token);
                SendCommand();
                this.Disconnect();
              
                return true;
            }
            catch (Exception E)
            {
                //记录日志
                m_errorString = E.Message;
                Console.WriteLine(E.Message );
                return false;
            }
        }
        public bool ReConnect()
        {
            if (m_tcpClient.Connected)// && (!DoActive()))
                return true;
            else
            {
                if (!m_tcpClient.Connected)
                {
                    try
                    {
                        Connect(m_host, m_port);
                        return true;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        return false;
                    }
                }
                else
                    return true;
            }
        }

        public bool ReConnectAndLogin()
        {
            if (m_tcpClient.Connected && (!DoActive()))
                return true;
            else
            {
                if (!m_tcpClient.Connected)
                {
                    try
                    {
                        Disconnect();
                        Connect(m_host, m_port);
                     
                        return DoLogin(_userid ,_token );
                    }
                    catch (Exception E)
                    {
                        return false;
                    }
                }
                else
                    return true;
            }
        }
    }
}
