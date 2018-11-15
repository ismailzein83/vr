using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Common
{
    public static class VRWebUtilities
    {
        public static string HtmlDecode(string value)
        {
            return System.Net.WebUtility.HtmlDecode(value);
        }

        public static string HtmlEncode(string value)
        {
            return System.Net.WebUtility.HtmlEncode(value);
        }

        public static string UrlDecode(string encodedValue)
        {
            return System.Net.WebUtility.UrlDecode(encodedValue);
        }

        public static string UrlEncode(string value)
        {
            return System.Net.WebUtility.UrlEncode(value);
        }
    }
}
