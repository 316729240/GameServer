using System;
using System.Collections.Generic;

using System.Text;

using NETUploadClient.Util;

namespace NETUploadClient.Protocol
{
  /// <summary>
  /// 通知
  /// </summary>
    public sealed  class Log : ProtocolBase
    {
     
        public Log()
        {
            protocolId =(short)XYSocketCommand.Log;
        }
        /// <summary>
        /// 告知事宜
        /// </summary>
        public byte matter = 0;
        /// <summary>
        /// 参数个数
        /// </summary>
        public byte paramlength = 0;
        public List<int> lparam = new List<int>();
        /// <summary>
        /// 操作用户id
        /// </summary>
        public int opuid = 0;
        /// <summary>
        /// 更新号
        /// </summary>
        public Int64 curchnb = 0;
      /// <summary>
      /// 从缓冲区获取属性
      /// </summary>
      /// <returns></returns>
        public override Boolean Buffer2Property()
        {
            if (Buffer == null) return false;
            matter = Buffer[4];
            lparam.Clear();
            lparam.Add(BitConverter.ToInt32(Buffer, 5));
            lparam.Add(BitConverter.ToInt32(Buffer, 9));
            opuid = BitConverter.ToInt32(Buffer, 13);
            curchnb = BitConverter.ToInt64(Buffer, 17);
            return true;
        }
        /// <summary>
        /// 将属性转化为缓冲区
        /// </summary>
        /// <returns></returns>
        public override Boolean Property2Buffer()
        {
            length += 2;
            byte[] byteArrayprotoclid = new byte[2];//协议号
            DataTool.DataToolInstance.ConvertInt16ToByteArray((short)protocolId, ref byteArrayprotoclid);
         
            length += 1;//告知事宜

            length += 4;
            byte[] byteArrayparam1 = new byte[4];//参数1
            DataTool.DataToolInstance.ConvertIntToByteArray(lparam[0], ref byteArrayparam1);

            length += 4;
            byte[] byteArrayparam2 = new byte[4];//参数2
            DataTool.DataToolInstance.ConvertIntToByteArray(lparam[1], ref byteArrayparam2);

            length += 4;
            byte[] byteArrayopuid = new byte[4];//用户id
            DataTool.DataToolInstance.ConvertIntToByteArray(opuid, ref byteArrayopuid);

            length += 8;
            byte[] byteArraycurchnb = new byte[8];//更新号
            DataTool.DataToolInstance.ConvertInt64ToByteArray(curchnb, ref byteArraycurchnb);

           
            length += 2;//长度
            length += 1;//校验位
            if (Buffer == null)
                Buffer = new byte[length];
            byte[] byteArrayprotocllen = new byte[2];//协议号
            DataTool.DataToolInstance.ConvertInt16ToByteArray((short)length, ref byteArrayprotocllen);
            Buffer[0] = byteArrayprotocllen[0];//设置协议长度
            Buffer[1] = byteArrayprotocllen[1];
            byteArrayprotoclid.CopyTo(Buffer, 2);//拷贝协议号
            Buffer[4] = matter;
            byteArrayparam1.CopyTo(Buffer, 4);//拷贝id
            byteArrayparam2.CopyTo(Buffer, 8);//拷贝type

            byteArrayopuid.CopyTo(Buffer, Buffer.Length - 1 - 8 - 4);
            byteArraycurchnb.CopyTo(Buffer, Buffer.Length - 1 - 8);
            CalVerifyByte();

            return true;
        }
    }

   
}
