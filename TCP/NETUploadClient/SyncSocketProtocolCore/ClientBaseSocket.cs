﻿using System;
using System.Collections.Generic;
using System.Text;
using NETUploadClient.SyncSocketCore;
using AsyncSocketServer;
using Common;
using GameCommon;

namespace NETUploadClient.SyncSocketProtocolCore
{

    public class ClientBaseSocket : SyncSocketInvokeElement
    {
        protected string m_errorString;
        public string ErrorString { get { return m_errorString; } }
        protected string m_userName;
        protected string m_password;

        public ClientBaseSocket()
            : base()
        {
        }

        public bool CheckErrorCode()
        {
            int errorCode = 0;
            m_incomingDataParser.GetValue(AsyncSocketServer.ProtocolKey.Code, ref errorCode);
            if (errorCode == AsyncSocketServer.ProtocolCode.Success)
                return true;
            else
            {
                m_errorString = AsyncSocketServer.ProtocolCode.GetErrorCodeString(errorCode);
                return false;
            }
        }

        public bool DoActive()
        {
            try
            {
                m_outgoingDataAssembler.Clear();
                m_outgoingDataAssembler.AddRequest();
                m_outgoingDataAssembler.AddCommand(AsyncSocketServer.ProtocolKey.Active);
                SendCommand();
                bool bSuccess = false;// RecvCommand();
                if (bSuccess)
                    return CheckErrorCode();
                else
                    return false;
            }
            catch (Exception E)
            {
                //记录日志
                m_errorString = E.Message; 
                return false;
            }
        }
        /// <summary>
        /// 命令请求
        /// </summary>
        /// <param name="name">命令名</param>
        /// <param name="para">参数组</param>
        /// <returns></returns>
        public bool SendCommand(string name, Parameter [] para)
        {
            try
            {
                m_outgoingDataAssembler.Clear();
                m_outgoingDataAssembler.AddRequest();
                m_outgoingDataAssembler.AddCommand(name);
                for(int i = 0; i < para.Length; i++)
                {
                    m_outgoingDataAssembler.AddValue(para[i].Name, para[i].Value);
                }
                SendCommand();
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
                m_errorString = E.Message;
                return false;
            }
        }
        /// <summary>
        /// 发送一个json对象
        /// </summary>
        /// <param name="name">命令</param>
        /// <param name="data">数据</param>
        /// <returns></returns>
        public bool SendJson(string name, object data)
        {
            return SendCommand(name, new Parameter[] {
                new Parameter("data",data.ToJson())
            });
        }
        /// <summary>
        /// 发送一个json对象
        /// </summary>
        /// <param name="name">命令</param>
        /// <param name="data">数据</param>
        /// <returns></returns>
        public bool SendJson(string name, Parameter[] data)
        {
            if (data == null)
            {
                return SendCommand(name, null);
            }
            else
            {
                return SendCommand(name, new Parameter[] {
                new Parameter("data",data.ToJson())
            });
            }
        }
        public bool DoLogin(string userName, string password)
        {
            try
            {
                m_outgoingDataAssembler.Clear();
                m_outgoingDataAssembler.AddRequest();
                m_outgoingDataAssembler.AddCommand(AsyncSocketServer.ProtocolKey.Login);
                m_outgoingDataAssembler.AddValue(AsyncSocketServer.ProtocolKey.UserName, userName);
                //m_outgoingDataAssembler.AddValue(AsyncSocketServer.ProtocolKey.Password, AsyncSocketServer.BasicFunc.MD5String(password));
                SendCommand();
                bool bSuccess = RecvCommand();
                if (bSuccess)
                {
                    bSuccess = CheckErrorCode();
                    if (bSuccess)
                    {
                        m_userName = userName;
                        m_password = password;
                    }
                    return bSuccess;
                }
                else
                    return false;
            }
            catch (Exception E)
            {
                //记录日志
                m_errorString = E.Message;
                return false;
            }
        }

        public bool ReConnect()
        {
            if (m_tcpClient.Connected && (!DoActive()))
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
                    catch (Exception)
                    {
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
                        return DoLogin(m_userName, m_password);
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
