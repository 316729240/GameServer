using LitJson;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{

    public static class Extend
    {
        public static string ToJson(this object obj)
        {
            return JsonMapper.ToJson(obj);
        }
        public static string ToStr(this object obj)
        {
            return obj + "";
        }
        public static int ToInt(this object obj)
        {
            try { 
                return Convert.ToInt32(obj);
            }catch{ return 0; }
        }
        public static double ToDouble(this object obj)
        {
            try
            {
                return Convert.ToDouble(obj);
            }
            catch { return 0; }
        }
    }
}
