using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NETUploadClient.SyncSocketProtocolCore;

namespace NETUploadClient.SyncSocketProtocol
{
    class ClientGameSocket : ClientBaseSocket
    {
        public ClientGameSocket()
            : base()
        {
            m_protocolFlag = AsyncSocketServer.ProtocolFlag.Upload;
        }

    }
}
