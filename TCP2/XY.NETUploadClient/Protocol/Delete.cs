using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using NETUploadClient.Util;

namespace NETUploadClient.Protocol
{
    /// <summary>
    /// 删除
    /// </summary>
  
    public sealed class Delete : OPBase
    {
        //需要关联删除的对象
        public List<int> _relateObjList = new List<int>();
        public Delete()
        {
            protocolId = (short)XYSocketCommand.Delete;

        }

        public override Boolean Property2Buffer()
        {

            length = 0;
            List<byte[]> lbyte = new List<byte[]>();
            foreach (var item in _relateObjList)
            {
                byte[] byteArrayobjid = new byte[4];//实体id
                DataTool.DataToolInstance.ConvertIntToByteArray(item, ref byteArrayobjid);
                lbyte.Add(byteArrayobjid);
                length += (short)lbyte[lbyte.Count - 1].Length;
            }
            length++;//对象个数+1个字节
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
            int offset = 12;
            int relateobjnum = Buffer[offset];
            _relateObjList.Clear();
            offset += 1;
            for (var i = 0; i < relateobjnum; i++)
            {
                _relateObjList.Add ( BitConverter.ToInt32(Buffer, offset));
                offset += 4;
            }
           // offset += 1;
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
           
           
            return "{\"objId\":" + objId + "}";
        }
    }
}
