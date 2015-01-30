using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CallGeneratorLibrary.Utilities
{
    public class ActionClass
    {
        public static string GetIPAddress()
        {
            string VisitorsIPAddr = string.Empty;
            if (HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null)
            {
                VisitorsIPAddr = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
            }
            else if (HttpContext.Current.Request.UserHostAddress.Length != 0)
            {
                VisitorsIPAddr = HttpContext.Current.Request.UserHostAddress;
            }
            return VisitorsIPAddr;
        }

        public static string GetRemoteAddress()
        {
            return HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
        }

        public static string GetComputerName()
        {
            return System.Environment.MachineName;
        }
    }
}
