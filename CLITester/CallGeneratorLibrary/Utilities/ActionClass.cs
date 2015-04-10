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
            try
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
            catch
            {
                return null;
            }
        }

        public static string GetRemoteAddress()
        {
            try
            {
                return HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
                catch
            {
                return null;
            }
        }

        public static string GetComputerName()
        {
            try
            {
                return System.Environment.MachineName;
            }
            catch
            {
                return null;
            }
        }
    }
}
