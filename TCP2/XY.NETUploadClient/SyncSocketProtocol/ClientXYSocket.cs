using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using NETUploadClient.SyncSocketProtocolCore;
using NETUploadClient.Protocol;
using NETUploadClient.Util.Common;
//using NETUploadClient.Util.Common;

namespace NETUploadClient.SyncSocketProtocol
{
    class ClientXYSocket : ClientBaseSocket
    {
        public ClientXYSocket(OutputReceiver outputReceiver)
            : base(outputReceiver)
        {
            m_protocolFlag = ProtocolFlag.XY ;
        }

      
       

    }
}
