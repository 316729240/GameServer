using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using NETUploadClient.Util;

namespace NETUploadClient.Protocol
{
    /// <summary>
    /// 更新
    /// </summary>
   
    public sealed  class Replace: OPBase
    {
     
        public Replace()
        {
            protocolId = (short)XYSocketCommand.Replace ;
        }
     
        public byte propscount = 0;//属性个数
        //替换属性列表
        public List<XYProtocolReplaceProperty> lpi = new List<XYProtocolReplaceProperty>();
    

        public override Boolean Property2Buffer()
        {
            length = 0;
            List<byte[]> lbyte = new List<byte[]>();
            foreach (var item in lpi)
            {
                lbyte.Add(item.ToBytes());
                length += (short)lbyte[lbyte.Count - 1].Length;
            }
            length++;//属性个数+1个字节
            base.Property2Buffer();//构造buffer
            Buffer[12] = (byte)lbyte.Count;
            int propertybyteindex = 13;//拷贝属性字节开始地址
            foreach (var item in lbyte)
            {
                item.CopyTo(Buffer, propertybyteindex);
                propertybyteindex += item.Length;
            }
            //计算校验字节,逐个字节异或
            CalVerifyByte();
            return true;
        }

        public override Boolean Buffer2Property()
        {
            this.objId = BitConverter.ToInt32(Buffer, 4);
            this.objType = BitConverter.ToInt32(Buffer, 8);
            this.propscount = Buffer[12];
            int offset = 13;
            int vallen = 0;
            for (var i = 0; i < this.propscount; i++)
            {
                if (Buffer[offset + 4] == (byte)ProtocolDataType.int_type)
                {
                  
                    XYProtocolReplaceProperty pi = new XYProtocolReplaceProperty();
                    pi.propid  = BitConverter.ToInt32(Buffer, offset);
                    offset += 5;
                    pi.propdatatype = (byte)ProtocolDataType.int_type;
                    pi.s_intval = BitConverter.ToInt32(Buffer, offset);
                  
                    offset += 4;
                    pi.intval = BitConverter.ToInt32(Buffer, offset);
                    lpi.Add(pi);
                    offset += 4;
                }
                else if (Buffer[offset + 4] == (byte)ProtocolDataType.short_type)
                {
                 //   offset += 5;
                    XYProtocolReplaceProperty pi = new XYProtocolReplaceProperty();
                    pi.propid = BitConverter.ToInt32(Buffer, offset);
                    offset += 5;
                    pi.propdatatype = (byte)ProtocolDataType.short_type;
                    pi.s_shortval  = BitConverter.ToInt16(Buffer, offset);
                    offset += 2;
                    pi.shortval  = BitConverter.ToInt16(Buffer, offset);
                    lpi.Add(pi);
                    offset += 2;
                }
                else if (Buffer[offset + 4] == (byte)ProtocolDataType.string_type)
                {
                   // offset += 5;
                   
                    XYProtocolReplaceProperty pi = new XYProtocolReplaceProperty();
                    pi.propid = BitConverter.ToInt32(Buffer, offset);
                    offset += 5;
                    vallen = BitConverter.ToInt16(Buffer, offset);
                    offset += 2;
                    byte[] s_strbyte = new byte[vallen];
                    for (var j = 0; j < vallen; j++)
                    {
                        s_strbyte[j] = Buffer[offset + j];
                    }
                    pi.propdatatype = (byte)ProtocolDataType.string_type;
                    pi.s_stringval = System.Text.Encoding.GetEncoding("gb2312").GetString(s_strbyte);
                    lpi.Add(pi);
                    offset +=vallen;
                    vallen = BitConverter.ToInt16(Buffer, offset);
                    offset += 2;
                    byte[] strbyte = new byte[vallen];
                    for (var j = 0; j < vallen; j++)
                    {
                        strbyte[j] = Buffer[offset + j];
                    }
                    pi.stringval = System.Text.Encoding.GetEncoding("gb2312").GetString(strbyte);
                    offset += vallen;
                }

            }
            this.reason = Buffer[offset];
            offset += 1;
            this.opuid = BitConverter.ToInt32(Buffer, offset);
            offset += 4;
            this.curchnb = BitConverter.ToInt64(Buffer, offset);
            offset += 8;
            //校验
            this.verify = Buffer[Buffer.Length - 1];
            return true;
        }
        public override string GetProtocolString()
        {
            string ret = "[";
            string retpart = "";
            foreach (var item in lpi)
            {
                if (retpart != "") retpart += ",";
                retpart += "{\"propdatatype\":\"" + item.propdatatype.ToString() + ",\"propid\":\"" + item.propid.ToString() + "\"" + ",\"intval\":\"" + item.intval.ToString() + "\"" + "}";
            }
            ret += retpart + "]";
            return "{\"objId\":\"" + objId + "\",\"props\":" + ret + ",\"objType\":" + objType + ",\"curchnb\"" + curchnb + "}";
        }
    }

   
}
