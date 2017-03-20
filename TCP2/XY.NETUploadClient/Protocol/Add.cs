using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using NETUploadClient.Util;

namespace NETUploadClient.Protocol
{
    /// <summary>
    /// 增加
    /// </summary>
    
    public sealed class Add : OPBase
    {

        public Add()
        {
            protocolId = (short)XYSocketCommand.Add;
        }

        public byte propscount = 0;//属性个数
        public List<XYProtocolProperty> lpi = new List<XYProtocolProperty>();
        /// <summary>
        /// 属性转换为缓冲区
        /// </summary>
        /// <returns></returns>
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
        /// <summary>
        /// 取出缓冲区的内容设置属性
        /// </summary>
        /// <returns></returns>
        public override Boolean Buffer2Property()
        {
            this.objId  = BitConverter.ToInt32(Buffer, 4);
            this.objType  = BitConverter.ToInt32(Buffer, 8);
            this.propscount = Buffer[12];
            int offset = 13;
            int valuelength = 0;
            for (var i = 0; i < this.propscount; i++)
            {
                if (Buffer[offset + 4] == (byte)ProtocolDataType.int_type)
                {
                    XYProtocolProperty protocolItem = new XYProtocolProperty();
                    protocolItem.propdatatype = (byte)ProtocolDataType.int_type;
                    protocolItem.propid  = BitConverter.ToInt32(Buffer, offset);
                    offset += 5;
                    protocolItem.intval = BitConverter.ToInt32(Buffer, offset);
                    lpi.Add(protocolItem);
                    offset += 4;
                }
                else if (Buffer[offset + 4] == (byte)ProtocolDataType.short_type)
                {
                    XYProtocolProperty protocolItem = new XYProtocolProperty();
                    protocolItem.propdatatype = (byte)ProtocolDataType.short_type;
                    protocolItem.propid = BitConverter.ToInt32(Buffer, offset);
                    offset += 5;
                    protocolItem.shortval = BitConverter.ToInt16(Buffer, offset);
                    lpi.Add(protocolItem);
                    offset += 2;
                }
                else if (Buffer[offset + 4] == (byte)ProtocolDataType.string_type)
                {


                  //  offset += 5;
                  
                   
                    XYProtocolProperty protocolItem = new XYProtocolProperty();
                    protocolItem.propdatatype = (byte)ProtocolDataType.string_type;
                    protocolItem.propid = BitConverter.ToInt32(Buffer, offset);
                    offset += 5;
                    valuelength = BitConverter.ToInt16(Buffer, offset);
                    offset += 2;
                    byte[] strbyte = new byte[valuelength];
                    for (var j = 0; j < valuelength; j++)
                    {
                        strbyte[j] = Buffer[offset + j];
                    }
                    protocolItem.stringval = System.Text.Encoding.GetEncoding("gb2312").GetString(strbyte);
                    lpi.Add(protocolItem);
                    offset += valuelength;
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
                retpart += "{\"propdatatype\":\"" + item.propdatatype.ToString() + ",\"propid\":\"" + item.propid.ToString() + "\"" + ",\"intval\":\"" + item.intval.ToString() + "\"" +"}";
            }
            ret += retpart + "]";
            return "{\"objId\":\"" + objId + "\",\"props\":" + ret + ",\"objType\":"+ objType + ",\"curchnb\""+ curchnb + "}";
        }
    }


}
