using LitJson;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameCommon
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
        public static int [] ToIntArr(this JsonData data)
        {
            int[] arr = new int[data.Count];
            for(int i = 0; i < arr.Length; i++)
            {
                arr[i] = data[i].ValueAsInt();
            }
            return arr;
        }
    }
}
