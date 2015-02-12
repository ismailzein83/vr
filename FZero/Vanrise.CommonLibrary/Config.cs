using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace Vanrise.CommonLibrary
{
    public static class Config
    {
        //public static bool CheckPermission = true;
        //public static int UserId = 6;
        
        private static string sqlConnectionString;
        public static string SQLConnectionString
        {
            get
            {
                sqlConnectionString = WebConfigurationManager.ConnectionStrings["FMSConnectionString"].ConnectionString;
                return sqlConnectionString;
            }
            set { sqlConnectionString = value; }
        }
    }
}
