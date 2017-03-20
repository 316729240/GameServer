using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using NETUploadClient.Util;

namespace NETUploadClient.Protocol
{
  /// <summary>
  /// 增删改协议基类
  /// </summary>
    public   class OPBase : ProtocolBase
    {
     
        public int objId = 0;//实体id
        public int objType = 0;//实体类型
       
        public byte reason =0;//修改原因
   //     public int reason = 0;//修改原因
        public int opuid =0;//操作用户id
        public Int64 curchnb = 0;//更新号
      /// <summary>
      /// buffer转换为属性
      /// </summary>
      /// <returns></returns>
        public override    Boolean Buffer2Property()
        {
            //
            int curidex = 0;
            if (Buffer == null) return false;
            length = (short)Buffer.Length;
            byte[] byteArrayprotoclid = new byte[2];//协议号
            byteArrayprotoclid[0] = Buffer[++curidex];
            byteArrayprotoclid[1] = Buffer[++curidex];
            protocolId =  BitConverter.ToInt16 (byteArrayprotoclid, 0);
            byte[] byteArrayobjid = new byte[4];//实体id
            byteArrayobjid[0] = Buffer[++curidex];
            byteArrayobjid[1] = Buffer[++curidex];
            byteArrayobjid[3] = Buffer[++curidex];
            byteArrayobjid[4] = Buffer[++curidex];
            objId = BitConverter.ToInt32(byteArrayobjid, 0);
            byte[] byteArrayobjtype = new byte[4];//实体类型
            byteArrayobjtype[0] = Buffer[++curidex];
            byteArrayobjtype[1] = Buffer[++curidex];
            byteArrayobjtype[2] = Buffer[++curidex];
            byteArrayobjtype[3] = Buffer[++curidex];
            objType = BitConverter.ToInt32(byteArrayobjtype, 0);
       
           // reason = Buffer[++curidex]; 
           // opuid = Buffer[++curidex];
            byte[] byteArraycurchnb = new byte[8];//更新号
            byteArraycurchnb[0] = Buffer[++curidex];
            byteArraycurchnb[1] = Buffer[++curidex];
            byteArraycurchnb[2] = Buffer[++curidex];
            byteArraycurchnb[3] = Buffer[++curidex];
            byteArraycurchnb[4] = Buffer[++curidex];
            byteArraycurchnb[5] = Buffer[++curidex];
            byteArraycurchnb[6] = Buffer[++curidex];
            byteArraycurchnb[7] = Buffer[++curidex];
            curchnb = BitConverter.ToInt64(byteArraycurchnb, 0);
            verify= Buffer[Buffer.Length - 1] ;//校验位
            return true;
        }
        /// <summary>
        /// 属性转换为buffer
        /// </summary>
        /// <returns></returns>
        public override Boolean Property2Buffer()
        {

            length+=2;
            byte[] byteArrayprotoclid = new byte[2];//协议号
            DataTool.DataToolInstance.ConvertInt16ToByteArray((short)protocolId, ref byteArrayprotoclid);
         
            length += 4;
            byte[] byteArrayobjid = new byte[4];//实体id
            DataTool.DataToolInstance.ConvertIntToByteArray(objId, ref byteArrayobjid);
          
            length += 4;
            byte[] byteArrayobjtype = new byte[4];//实体类型
            DataTool.DataToolInstance.ConvertIntToByteArray(objType, ref byteArrayobjtype);  
           
        
            length += 1;// 原因
          //  byte[] byteArrayreason = new byte[4];//原因
           // DataTool.DataToolInstance.ConvertIntToByteArray(reason, ref byteArrayreason);

            length += 4;
            byte[] byteArrayopuid = new byte[4];//更新人id
            DataTool.DataToolInstance.ConvertIntToByteArray(opuid, ref byteArrayopuid);

          
            byte[] byteArraycurchnb = new byte[8];//更新号
            DataTool.DataToolInstance.ConvertInt64ToByteArray(curchnb, ref byteArraycurchnb);
            
            length += 8;
            length += 2;//协议长度
            length += 1;//校验位
            if(Buffer==null )
            Buffer = new byte[length];
            byte[] byteArrayprotocllen = new byte[2];//协议号
            DataTool.DataToolInstance.ConvertInt16ToByteArray((short)length, ref byteArrayprotocllen);
            Buffer[0] = byteArrayprotocllen[0];//设置协议长度
            Buffer[1] = byteArrayprotocllen[1];
            byteArrayprotoclid.CopyTo(Buffer,2);//拷贝协议号
            byteArrayobjid.CopyTo(Buffer, 4);//拷贝id
            byteArrayobjtype.CopyTo(Buffer, 8);//拷贝type

           // byteArrayreason.CopyTo(Buffer, Buffer.Length - 1 - 8 - 4 - 4);//reason
            Buffer[Buffer.Length - 1 - 8 - 4-1] = reason;

            byteArrayopuid.CopyTo(Buffer, Buffer.Length - 1 - 8- 4);//操作人
            byteArraycurchnb.CopyTo(Buffer, Buffer.Length - 1 - 8);//更新号
          //  Buffer[Buffer.Length - 1] = verify;//校验位

            return true;
        }

        public override Boolean CalVerifyByte()
        {
            if (Buffer.Length > 0)
            {
                for (int i = 0; i < Buffer.Length - 1; i++)
                {
                    if (i == 0)
                        Buffer[Buffer.Length - 1] = Buffer[i];
                    else
                    {
                        Buffer[Buffer.Length - 1] ^= Buffer[i];
                    }

                }
                return true;
            }
            else {
                return false;
            }

        }
    }

   
}
