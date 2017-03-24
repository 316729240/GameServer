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
    }
}
