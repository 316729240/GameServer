using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AsyncSocketServer
{
    public class OutgoingDataAssembler
    {
        private List<string> m_protocolText;
        private List<Type > m_protocoltype;
        public OutgoingDataAssembler()
        {
            m_protocolText = new List<string>();
            m_protocoltype = new List<Type>();
        }

        public void Clear()
        {
            m_protocolText.Clear();
            m_protocoltype.Clear();
        }

        public string GetProtocolText()
        {
            string tmpStr = "";
            if (m_protocolText.Count > 0)
            {
                tmpStr = m_protocolText[0];
                for (int i = 1; i < m_protocolText.Count; i++)
                {
                    tmpStr = tmpStr + ProtocolKey.ReturnWrap + m_protocolText[i];
                }
            }
            return tmpStr;
        }

        public void AddRequest()
        {
            //    m_protocolText.Add(ProtocolKey.LeftBrackets + ProtocolKey.Request + ProtocolKey.RightBrackets);
        }

        public void AddResponse()
        {
            m_protocolText.Add(ProtocolKey.Response + ProtocolKey.EqualSign + (short)XYSocketCommand.Notify);
            m_protocoltype.Add(typeof(short));
            // m_protocolText.Add(ProtocolKey.LeftBrackets + ProtocolKey.Response + ProtocolKey.RightBrackets);
        }

        public void AddCommand(string commandKey)
        {
            // m_protocolText.Add(ProtocolKey.Command + ProtocolKey.EqualSign + commandKey);

        }
        public void AddCommand(short commandKey)
        {
            m_protocolText.Add(ProtocolKey.Command + ProtocolKey.EqualSign + commandKey);
            m_protocoltype.Add(typeof(short));
        }
        public void AddSuccess()
        {
            m_protocolText.Add(ProtocolKey.Result + ProtocolKey.EqualSign + (byte)ExcuteResult.Success );
            m_protocoltype.Add(typeof(byte));
            // m_protocolText.Add(ProtocolKey.Code + ProtocolKey.EqualSign + ProtocolCode.Success.ToString());
        }
        public void AddFailure()
        {
            m_protocolText.Add(ProtocolKey.Result + ProtocolKey.EqualSign + (byte)ExcuteResult.Fail );
            m_protocoltype.Add(typeof(byte ));

        }
        public void AddFailure(int errorCode, string message)
        {
            m_protocolText.Add(ProtocolKey.Code + ProtocolKey.EqualSign + errorCode.ToString());
            m_protocolText.Add(ProtocolKey.Message + ProtocolKey.EqualSign + message);
        }

        public void AddValue(string protocolKey, string value)
        {
            m_protocolText.Add(protocolKey + ProtocolKey.EqualSign + value);
        }

        public void AddValue(string protocolKey, byte value)
        {
            m_protocolText.Add(protocolKey + ProtocolKey.EqualSign + value.ToString());
            m_protocoltype.Add(typeof(byte));
        }
        public void AddValue(string protocolKey, short value)
        {
            m_protocolText.Add(protocolKey + ProtocolKey.EqualSign + value.ToString());
            m_protocoltype.Add(typeof(short));
        }

        public void AddValue(string protocolKey, int value)
        {
            m_protocolText.Add(protocolKey + ProtocolKey.EqualSign + value.ToString());
            m_protocoltype.Add(typeof(Int32));
        }
        public void AddValue(string protocolKey, long value)
        {
            m_protocolText.Add(protocolKey + ProtocolKey.EqualSign + value.ToString());
            m_protocoltype.Add(typeof(long));
        }

       
        public byte[] GetBytes()
        {
            int bufferlen = 0; byte[] buffer = null;
            for (var i = 0; i < m_protocoltype.Count; i++)
            {
                if (m_protocoltype[i] == typeof(byte))
                {
                    bufferlen++;
                }
                else if (m_protocoltype[i] == typeof(short))
                {
                    bufferlen += 2;
                }
                else if (m_protocoltype[i] == typeof(int))
                {
                    bufferlen += 4;
                }
                else if (m_protocoltype[i] == typeof(long))
                {
                    bufferlen += 8;
                }
            }
            int offset = 0;
            if (bufferlen > 0)
            {
                buffer = new byte[bufferlen + 3];//

                offset = 2;
                for (var i = 0; i < m_protocoltype.Count; i++)
                {
                    var parseval = m_protocolText[i].Split('=')[1];
                  //  Console.WriteLine(m_protocoltype[i].GetType());
                    if (typeof(byte) == m_protocoltype[i])
                    {
                        buffer[offset] = byte.Parse(parseval);
                        offset++;
                    }
                    else if (typeof(short) == m_protocoltype[i])
                    {
                        var val = short.Parse(parseval);
                        byte[] byteArrayprotoclid = new byte[2];//协议号
                        DataTool.DataToolInstance.ConvertInt16ToByteArray((short)val, ref byteArrayprotoclid);
                        buffer[offset] = byteArrayprotoclid[0];
                        buffer[offset + 1] = byteArrayprotoclid[1];
                        offset += 2;
                    }
                    //else if (typeof(Int16) == m_protocoltype[i])
                    //{
                    //    var val = short.Parse(parseval);
                    //    byte[] byteArrayprotoclid = new byte[2];//协议号
                    //    DataTool.DataToolInstance.ConvertInt16ToByteArray((short)val, ref byteArrayprotoclid);
                    //    buffer[offset] = byteArrayprotoclid[0];
                    //    buffer[offset + 1] = byteArrayprotoclid[1];
                    //    offset += 2;
                    //}
                    else if (typeof(Int32) == m_protocoltype[i])
                    {
                        var val = int.Parse(parseval);
                        byte[] byteArrayprotoclid = new byte[4];//协议号
                        DataTool.DataToolInstance.ConvertIntToByteArray(val, ref byteArrayprotoclid);
                        for (var k = 0; k < 4; k++)
                        {
                            buffer[offset + k] = byteArrayprotoclid[k];
                        }
                        offset += 4;
                    }
                    else if (typeof(long) == m_protocoltype[i])
                    {
                        var val = long.Parse(parseval);
                        byte[] byteArraytoken = new byte[8];//token
                        DataTool.DataToolInstance.ConvertInt64ToByteArray(val, ref byteArraytoken);
                        for (var k = 0; k < 8; k++)
                        {
                            buffer[offset + k] = byteArraytoken[k];
                        }
                        offset += 8;
                    }
                }
            }
            //协议长度
            byte[] byteArrayprotocllen = new byte[2];//
            DataTool.DataToolInstance.ConvertInt16ToByteArray((short)buffer.Length , ref byteArrayprotocllen);
            for (var k = 0; k < 2; k++)
            {
                buffer[ k] = byteArrayprotocllen[k];
            }
            //校验位
            buffer[buffer.Length - 1] = 1;
            return buffer;
        }

        public byte[] GetBytes(List<string> m_protocolText, List<Type> m_protocoltype)
        {
            int bufferlen = 0; byte[] buffer = null;
            for (var i = 0; i < m_protocoltype.Count; i++)
            {
                if (m_protocoltype[i] == typeof(byte))
                {
                    bufferlen++;
                }
                else if (m_protocoltype[i] == typeof(short))
                {
                    bufferlen += 2;
                }
                else if (m_protocoltype[i] == typeof(long))
                {
                    bufferlen += 8;
                }
            }
            if (bufferlen > 0)
            {
                buffer = new byte[bufferlen + 3];//
                int offset = 0;

                for (var i = 0; i < m_protocoltype.Count; i++)
                {
                    var parseval = m_protocolText[i].Split('=')[1];
                    Console.WriteLine(m_protocoltype[i].GetType());
                    if (typeof(byte) == m_protocoltype[i])
                    {
                        buffer[offset] = byte.Parse(parseval);
                        offset++;
                    }
                    else if (typeof(short) == m_protocoltype[i])
                    {
                        var val = short.Parse(parseval);
                        byte[] byteArrayprotoclid = new byte[2];//协议号
                        DataTool.DataToolInstance.ConvertInt16ToByteArray((short)val, ref byteArrayprotoclid);
                        buffer[offset] = byteArrayprotoclid[0];
                        buffer[offset + 1] = byteArrayprotoclid[1];
                        offset += 2;
                    }
                    else if (typeof(Int16) == m_protocoltype[i])
                    {
                        var val = short.Parse(parseval);
                        byte[] byteArrayprotoclid = new byte[2];//协议号
                        DataTool.DataToolInstance.ConvertInt16ToByteArray((short)val, ref byteArrayprotoclid);
                        buffer[offset] = byteArrayprotoclid[0];
                        buffer[offset + 1] = byteArrayprotoclid[1];
                        offset += 2;
                    }
                    else if (typeof(long) == m_protocoltype[i])
                    {
                        var val = long.Parse(parseval);
                        byte[] byteArraytoken = new byte[8];//token
                        DataTool.DataToolInstance.ConvertInt64ToByteArray(val, ref byteArraytoken);
                        for (var k = 0; k < 8; k++)
                        {
                            buffer[offset + k] = byteArraytoken[k];
                        }
                        offset += 8;
                    }
                }
            }
            return buffer;
        }

     

        public void AddValue(string protocolKey, Single value)
        {
            m_protocolText.Add(protocolKey + ProtocolKey.EqualSign + value.ToString());
        }

        public void AddValue(string protocolKey, double value)
        {
            m_protocolText.Add(protocolKey + ProtocolKey.EqualSign + value.ToString());
        }
    }
}
