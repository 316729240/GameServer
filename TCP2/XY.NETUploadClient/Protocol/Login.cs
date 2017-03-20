using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using NETUploadClient.Util;

namespace NETUploadClient.Protocol
{
    /// <summary>
    /// 登录
    /// </summary>
    public sealed class Login : ProtocolBase
    {

        public Login()
        {
            protocolId = (short)XYSocketCommand.Login;
        }

        public int userid = 0;//用户id
        public Int64 token = 0;//token

        public override Boolean Buffer2Property()
        {
            //
            if (Buffer == null) return false;
            byte[] byteArrayprotoclid = new byte[4];//用户id
            int curidex = 4;//直接跳到userid
            byteArrayprotoclid[0] = Buffer[curidex++];
            byteArrayprotoclid[1] = Buffer[curidex++];
            byteArrayprotoclid[2] = Buffer[curidex++];
            byteArrayprotoclid[3] = Buffer[curidex++];
            userid = BitConverter.ToInt32(byteArrayprotoclid, 0);

            byte[] byteArrayprotocltoken = new byte[8];//协议号
            int copylen = byteArrayprotocltoken.Length;
            for (var i = 0; i < copylen; i++)
                byteArrayprotocltoken[i] = Buffer[curidex++];
        
            token = BitConverter.ToInt64(byteArrayprotocltoken, 0);
            return true;
        }
        /// <summary>
        /// 属性写入buffer中
        /// </summary>
        /// <returns></returns>
        public override Boolean Property2Buffer()
        {
            //
            if (Buffer == null)
            {
                Buffer = new byte[17];
            }

            int offset = 0;
            byte[] byteArrayprotocllegth = new byte[2];//长度
            DataTool.DataToolInstance.ConvertInt16ToByteArray((short)Buffer.Length , ref byteArrayprotocllegth);
            int copylen = byteArrayprotocllegth.Length;
            for (var i = 0; i < copylen; i++)
            {
                Buffer[offset] = (byte)byteArrayprotocllegth[i];
                offset++;
            }
            byte[] byteArrayprotoclid = new byte[2];//协议号
            DataTool.DataToolInstance.ConvertInt16ToByteArray((short)protocolId, ref byteArrayprotoclid);
           
            copylen = byteArrayprotoclid.Length;
            for (var i = 0; i < copylen; i++)
            {
                Buffer[offset] = (byte)byteArrayprotoclid[i];
                offset++;
            }

            byte[] byteArrayprotocluserid = new byte[4];//用户id
            DataTool.DataToolInstance.ConvertIntToByteArray((int)userid, ref byteArrayprotocluserid);

            copylen = byteArrayprotocluserid.Length;
            for (var i = 0; i < copylen; i++)
            {
                Buffer[offset] = (byte)byteArrayprotocluserid[i];
                offset++;
            }

            byte[] byteArraytoken = new byte[8];
            DataTool.DataToolInstance.ConvertInt64ToByteArray(token, ref byteArraytoken);
            copylen = byteArraytoken.Length;
            for (var i = 0; i < copylen; i++)
            {
                Buffer[offset] = (byte)byteArraytoken[i];
                offset++;
            }
            //计算verify

            Buffer[offset] = verify;//校验位
            return true;
        }
       
    }


}
