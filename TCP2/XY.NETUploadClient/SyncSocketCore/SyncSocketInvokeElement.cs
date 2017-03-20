using System;
using System.Text;
using System.Net.Sockets;
using NETUploadClient.Protocol;
using System.Threading;
using NETUploadClient.Util.Common;

namespace NETUploadClient.SyncSocketCore
{
    public class SyncSocketInvokeElement
    {
        protected TcpClient m_tcpClient;
        protected string m_host;
        protected int m_port;
        protected ProtocolFlag m_protocolFlag;
        protected int SocketTimeOutMS { get { return m_tcpClient.SendTimeout; } set { m_tcpClient.SendTimeout = value; m_tcpClient.ReceiveTimeout = value; } }
        private bool m_netByteOrder;
        public bool NetByteOrder { get { return m_netByteOrder; } set { m_netByteOrder = value; } } //长度是否使用网络字节顺序
        protected OutgoingDataAssembler m_outgoingDataAssembler; //协议组装器，用来组装往外发送的命令
        protected DynamicBufferManager m_recvBuffer; //接收数据的缓存
        protected IncomingDataParser m_incomingDataParser; //收到数据的解析器，用于解析返回的内容
        protected DynamicBufferManager m_sendBuffer; //发送数据的缓存，统一写到内存中，调用一次发送

        public event MyEventHandler CommandEvent;
        private OutputReceiver outputReceiver;
        public SyncSocketInvokeElement(OutputReceiver outputReceiver) : this()
        {
            this.outputReceiver = outputReceiver;
        }
        public SyncSocketInvokeElement()
        {

            m_tcpClient = new TcpClient();
            m_tcpClient.Client.Blocking = true; //使用阻塞模式，即同步模式
            m_protocolFlag = ProtocolFlag.None;
            SocketTimeOutMS = ProtocolConst.SocketTimeOutMS;

            m_outgoingDataAssembler = new OutgoingDataAssembler();
            m_recvBuffer = new DynamicBufferManager(ProtocolConst.ReceiveBufferSize);
            m_incomingDataParser = new IncomingDataParser();
            m_sendBuffer = new DynamicBufferManager(ProtocolConst.ReceiveBufferSize);
        }

        public Boolean Connect(string host, int port)
        {
            try
            {
                m_host = host;
                m_port = port;
                if (m_tcpClient.Connected == false) Disconnect();
                m_tcpClient.Connect(host, port);
                byte[] socketFlag = new byte[1];
                socketFlag[0] = (byte)m_protocolFlag;
                m_tcpClient.Client.Send(socketFlag, SocketFlags.None); //发送标识
              
                return true;
            }
            catch (Exception e)
            {
                if (outputReceiver != null)
                    outputReceiver.AddNewLine(e.Message );
                return false;
            }
        }

        public void Disconnect()
        {
            m_tcpClient.Close();
            m_tcpClient = null;
            m_tcpClient = new TcpClient();
          
        }

        public void SendCommand()
        {
         
            byte[] bufferUTF8 = m_outgoingDataAssembler.GetBytes();// Encoding.UTF8.GetBytes(commandText);

            int totalLength = sizeof(int) + bufferUTF8.Length; //获取总大小
                                                               //   int totalLength = sizeof(short) + bufferUTF8.Length + sizeof(byte); //获取总大小
            m_sendBuffer.Clear();
            m_sendBuffer.WriteInt(totalLength, false); //写入总大小
            m_sendBuffer.WriteInt(bufferUTF8.Length, false); //写入命令大小
            m_sendBuffer.WriteBuffer(bufferUTF8); //写入命令内容
          
            m_tcpClient.Client.Send(m_sendBuffer.Buffer, 0, m_sendBuffer.DataCount, SocketFlags.None); //使用阻塞模式，Socket会一次发送完所有数据后才返回
                                                                                                       //  System.Console.WriteLine(System.Text.Encoding.Default.GetString(m_sendBuffer.Buffer));
        }

        public void SendCommand(byte[] buffer, int offset, int count)
        {
            string commandText = m_outgoingDataAssembler.GetProtocolText();
            byte[] bufferUTF8 = Encoding.UTF8.GetBytes(commandText);
            int totalLength = sizeof(int) + bufferUTF8.Length + count; //获取总大小
            m_sendBuffer.Clear();
            m_sendBuffer.WriteInt(totalLength, false); //写入总大小
            m_sendBuffer.WriteInt(bufferUTF8.Length, false); //写入命令大小
            m_sendBuffer.WriteBuffer(bufferUTF8); //写入命令内容
            m_sendBuffer.WriteBuffer(buffer, offset, count); //写入二进制数据
            m_tcpClient.Client.Send(m_sendBuffer.Buffer, 0, m_sendBuffer.DataCount, SocketFlags.None);
        }

        public bool RecvCommand()
        {
            m_recvBuffer.Clear();
            m_tcpClient.Client.Receive(m_recvBuffer.Buffer, sizeof(int), SocketFlags.None);
            int packetLength = BitConverter.ToInt32(m_recvBuffer.Buffer, 0); //获取包长度
            if (NetByteOrder)
                packetLength = System.Net.IPAddress.NetworkToHostOrder(packetLength); //把网络字节顺序转为本地字节顺序
            m_recvBuffer.SetBufferSize(sizeof(int) + packetLength); //保证接收有足够的空间
            m_tcpClient.Client.Receive(m_recvBuffer.Buffer, sizeof(int), packetLength, SocketFlags.None);

            int commandLen = BitConverter.ToInt32(m_recvBuffer.Buffer, sizeof(int)); //取出命令长度


            if (!m_incomingDataParser.DecodeProtocolBuffer(m_recvBuffer.Buffer, sizeof(int) + sizeof(int), commandLen)) //解析命令
                return false;
            else
                return true;
        }

        public void RecvCommandContinus()
        {
            try
            {
                m_recvBuffer.Clear();
             int receivenum=   m_tcpClient.Client.Receive(m_recvBuffer.Buffer, sizeof(int), SocketFlags.None);
                if (receivenum == 0)
                {
                    if (outputReceiver != null)
                        outputReceiver.AddNewLine("服务器已经关闭。");
                    return;
                }
                int packetLength = BitConverter.ToInt32(m_recvBuffer.Buffer, 0); //获取包长度
                if (NetByteOrder)
                    packetLength = System.Net.IPAddress.NetworkToHostOrder(packetLength); //把网络字节顺序转为本地字节顺序
                m_recvBuffer.SetBufferSize(sizeof(int) + packetLength); //保证接收有足够的空间
                receivenum= m_tcpClient.Client.Receive(m_recvBuffer.Buffer, sizeof(int), packetLength, SocketFlags.None);

                int commandLen = BitConverter.ToInt32(m_recvBuffer.Buffer, sizeof(int)); //取出命令长度
                ProtocolBase command;
                if (m_incomingDataParser.DecodeProtocolBuffer(m_recvBuffer.Buffer, sizeof(int) + sizeof(int), commandLen, out command))
                {
                    if (CommandEvent != null)
                    {
                        CommandEvent(new MyEventArgs(command, ""));
                    }

                    ThreadPool.QueueUserWorkItem(e =>
                    {

                        RecvCommandContinus();
                    });
                }
            }
            catch (Exception e)
            {
                if (outputReceiver != null)
                    outputReceiver.AddNewLine(e.Message);
            }
        }


    }
}
