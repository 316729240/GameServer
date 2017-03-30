using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameCommon
{    
    /// <summary>
     /// 数据支持
     /// </summary>
    public class DataProvider
    {
        Dictionary<string, object> data = new Dictionary<string, object>();
        public void Set(string name,object value)
        {
            data[name] = value;
        }
        public object Get(string name)
        {
            if (data.ContainsKey(name))
            {
                return data[name];
            }
            else
            {
                return null;
            }
        }
        public T Get<T>(string name)
        {
            if (data.ContainsKey(name))
            {
                try { 
                return (T)data[name];
                }catch
                {
                    return default(T);
                }
            }
            else
            {
                return default(T);
            }
        }
    }
}
