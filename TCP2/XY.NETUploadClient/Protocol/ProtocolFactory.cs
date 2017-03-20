using System;
using System.Collections.Generic;
using System.IO;
//using System.Linq;
using System.Net;
using System.Text;
//using System.Threading.Tasks;

namespace NETUploadClient.HttpClient
{
    public class ProtocolFactory
    {
        private static ProtocolFactory _hcc;
        public static ProtocolFactory ProtocolFactoryInstance
        {
            get
            {
                if (_hcc == null)
                {
                    _hcc = new ProtocolFactory();
                }
                return _hcc;
            }
        }


        private ProtocolFactory()
        { }
      
    }
}
