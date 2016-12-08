using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AuthExample
{
    static class Utilities
    {
        public static string ToJson(this object input)
        {
            try
            {
                return JsonConvert.SerializeObject(input);
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static T FromJson<T>(this string input)
        {
            if (typeof(T) == typeof(string))
                return (T)(input as object);
            try
            {
                return JsonConvert.DeserializeObject<T>(input);
            }
            catch (Exception)
            {
                //log
            }
            return default(T);
        }
    }
}
