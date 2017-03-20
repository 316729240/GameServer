using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Threading;
//using System.Threading.Tasks;

namespace NETUploadClient.Util.Common
{
    public class SyncEvents
    {
        public SyncEvents()

        {


            _waitThreadEvent = new ManualResetEvent(false);
            _newItemEvent = new AutoResetEvent(false);
            _exitThreadEvent = new ManualResetEvent(false);

            _eventArray = new WaitHandle[3];
            _eventArray[0] = _newItemEvent;
            _eventArray[1] = _exitThreadEvent;
            _eventArray[2] = _waitThreadEvent;

        }


        // 公共属性允许对事件进行安全访问。  

        public EventWaitHandle ExitThreadEvent

        {

            get { return _exitThreadEvent; }

        }
        public EventWaitHandle WaitThreadEvent

        {

            get { return _waitThreadEvent; }

        }
        public EventWaitHandle NewItemEvent

        {

            get { return _newItemEvent; }

        }

        public WaitHandle[] EventArray

        {

            get { return _eventArray; }

        }


        private EventWaitHandle _newItemEvent;

        private EventWaitHandle _exitThreadEvent;
        private EventWaitHandle _waitThreadEvent;
        private WaitHandle[] _eventArray;
    }
}
