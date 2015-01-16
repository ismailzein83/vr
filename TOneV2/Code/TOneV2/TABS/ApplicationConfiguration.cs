using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TABS
{
    public class ApplicationConfiguration
    {
        public static string hibernateconnection_string
        {
            get
            {
                string OriginalConnectionString = System.Configuration.ConfigurationManager.AppSettings["hibernate.connection.connection_string"].ToString();
                if (DataConfiguration.ConnectionStringEncrypted)
                    return WebHelperLibrary.Utility.SimpleDecode(OriginalConnectionString);
                return OriginalConnectionString;
            }
        }


        public static string CRMconnection_string
        {
            get
            {
                string OriginalCRMConnectionString = System.Configuration.ConfigurationManager.AppSettings["CRMconnection_string"].ToString();
                if (DataConfiguration.ConnectionStringEncrypted)
                    return WebHelperLibrary.Utility.SimpleDecode(OriginalCRMConnectionString);
                return OriginalCRMConnectionString;
            }
        }
    }
}
