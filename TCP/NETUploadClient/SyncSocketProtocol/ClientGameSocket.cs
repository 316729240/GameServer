using System;
using System.Collections.Generic;
using System.Text;
using NETUploadClient.SyncSocketProtocolCore;

namespace NETUploadClient.SyncSocketProtocol
{
    public class ClientGameSocket : ClientBaseSocket
    {
        public ClientGameSocket(): base()
        {
            m_protocolFlag = AsyncSocketServer.ProtocolFlag.Upload;
        }

    }
}
