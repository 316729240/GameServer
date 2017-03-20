using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
//using System.Threading.Tasks;

namespace NETUploadClient.Util.Common
{
    public delegate void MyEventHandler(MyEventArgs margs);

    public class MyEventArgs 
    {
        // Fields
        private object _obj;
        private string _objname;

        // Methods
        public MyEventArgs(object obj, string objname)
        {
            this._obj = obj;
            this._objname = objname;
        }

        // Properties
        public object MyArg
        {
            get
            {
                return this._obj;
            }
        }

        public string OName
        {
            get
            {
                return this._objname;
            }
        }
    }


    public delegate void NewLineEvent(string message);

    public class OutputReceiver
    {
        public event NewLineEvent NewLineEvent;

        public void AddNewLine(string msg)
        {
            if (NewLineEvent != null)
            {
                NewLineEvent(msg);
            }
        }
    }
}
