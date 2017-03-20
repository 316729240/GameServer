using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using NETUploadClient.Util;

namespace NETUploadClient.Protocol
{
    public enum ProtocolDataType { short_type,int_type,string_type}


    public abstract  class ProtocolBase
    {
        protected  short length = 0;//协议长度
        public short protocolId = 0;//协议号
        protected byte verify = 0;//校验位

        public byte []Buffer
        { get; set; }
        public abstract Boolean Property2Buffer();
        public abstract  Boolean Buffer2Property();
    
        public virtual  Boolean CalVerifyByte()
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
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 检查校验位是否正确
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public virtual Boolean CheckVerifyByte(byte[] buffer)
        {
           
            byte cmp=0;
            if (buffer.Length > 0)
            {
                for (int i = 0; i < buffer.Length - 1; i++)
                {
                    if (i == 0)
                        cmp = buffer[i];
                    else
                    {
                        cmp ^= buffer[i];
                    }

                }
                return cmp== buffer[buffer.Length -1];
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 设置缓冲区
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public virtual  Boolean CopyBuffer(byte[] buffer, int offset, int count)
        {
            if (count > 0)
            {
                if (Buffer == null)
                {
                    Buffer = new byte[count];
                }
                for (var i = 0; i < Buffer.Length; i++)
                {
                    Buffer[i] = buffer[offset + i];

                }
                return true;
            }

            return false;

        }

        public virtual string GetProtocolString()
        {
            return "";
        }

    }
    /// <summary>
    /// 替换用的属性元素
    /// </summary>
    public class XYProtocolReplaceProperty: XYProtocolProperty
    {
        
        public string s_stringval = "";
        public short s_shortval = 0;
        public int s_intval = 0;

       /// <summary>
       /// 转换为字节
       /// </summary>
       /// <returns></returns>
        public byte[] ToBytes()
        {
            byte[] byteret = null;
            byte[] byteArraypropid = new byte[4];
            DataTool.DataToolInstance.ConvertIntToByteArray(propid, ref byteArraypropid);
            if (propdatatype == (byte)ProtocolDataType.string_type)
            {
                int offset = 5;
                byte[] s_byteArray = System.Text.Encoding.GetEncoding("gb2312").GetBytes(s_stringval);
                byte[] byteArray = System.Text.Encoding.GetEncoding("gb2312").GetBytes(stringval);

                byteret = new byte[offset +2+ s_byteArray.Length+2+ byteArray.Length ];
                byte[] s_byteArraylength = new byte[2];
                DataTool.DataToolInstance.ConvertInt16ToByteArray((short)s_byteArray.Length, ref s_byteArraylength);
                s_byteArraylength.CopyTo(byteret, offset);//拷贝长度
                offset += 2;
                s_byteArray.CopyTo(byteret, offset);//拷贝内容
                offset += s_byteArray.Length ;

                byte[] byteArraylength = new byte[2];
                DataTool.DataToolInstance.ConvertInt16ToByteArray((short)byteArray.Length, ref byteArraylength);
                byteArraylength.CopyTo(byteret, offset);
                offset +=  2;
                byteArray.CopyTo(byteret, offset);
                offset += byteArray.Length;
            }
            else if (propdatatype == (byte)ProtocolDataType.int_type)
            {
                int offset = 5;
                byte[] s_byteArray = new byte[4];
                DataTool.DataToolInstance.ConvertIntToByteArray(s_intval, ref s_byteArray);
                byteret = new byte[offset + s_byteArray.Length*2];
                s_byteArray.CopyTo(byteret, offset);

                byte[] byteArray = new byte[4];
                DataTool.DataToolInstance.ConvertIntToByteArray(intval, ref byteArray);
                byteArray.CopyTo(byteret, offset+4);
            }
            else if (propdatatype == (byte)ProtocolDataType.short_type)
            {
                int offset = 5;
                byte[] s_byteArray = new byte[2];
                DataTool.DataToolInstance.ConvertInt16ToByteArray(s_shortval, ref s_byteArray);
                byteret = new byte[offset + s_byteArray.Length*2];
                s_byteArray.CopyTo(byteret, offset);

                byte[] byteArray = new byte[2];
                DataTool.DataToolInstance.ConvertInt16ToByteArray(shortval, ref byteArray);
                byteArray.CopyTo(byteret, offset+2);
            }

            byteArraypropid.CopyTo(byteret, 0);
            byteret[4] = propdatatype;

            return byteret;
        }
        public Boolean BuildProperty()
        {
            return true;
        }
    }
    /// <summary>
    /// 属性元素
    /// </summary>
    public class XYProtocolProperty
    {
        public int propid = 0;//属性id
        public byte propdatatype = 0;//数据属性类型
        public short length = 0;//长度
        public string stringval = "";
        public short shortval = 0;
        public int intval = 0;

        protected byte[] Buffer
        { get; set; }
        public byte[] ToBytes()
        {
            byte[] byteret = null;
            byte[] byteArraypropid = new byte[4];
            DataTool.DataToolInstance.ConvertIntToByteArray(propid, ref byteArraypropid);
            if (propdatatype ==(byte) ProtocolDataType.string_type)
            {
                byte[] byteArray = System.Text.Encoding.GetEncoding("gb2312").GetBytes(stringval);
                byteret = new byte[7+byteArray .Length ];
                byte[] byteArraylength = new byte[2];
                DataTool.DataToolInstance.ConvertInt16ToByteArray((short)byteArray.Length, ref byteArraylength);
                byteArraylength.CopyTo(byteret, 5);
                byteArray.CopyTo(byteret,7);
            }
           else if (propdatatype == (byte)ProtocolDataType.int_type )
            {
                byte[] byteArray = new byte[4];
                    DataTool.DataToolInstance.ConvertIntToByteArray(intval,ref byteArray);
                byteret = new byte[5 + byteArray.Length];
                byteArray.CopyTo(byteret, 5);
            }
            else if (propdatatype == (byte)ProtocolDataType.short_type )
            {
               byte[] byteArray = new byte[2];
                DataTool.DataToolInstance.ConvertInt16ToByteArray(shortval, ref byteArray);
                byteret = new byte[5 + byteArray.Length];
                byteArray.CopyTo(byteret, 5);
            }

            byteArraypropid.CopyTo(byteret,0);
            byteret[4] = propdatatype;
            return byteret;
        }
        public  virtual Boolean BuildProperty()
        {
            return true;
        }
    }
}
