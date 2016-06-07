using System.Web.Configuration;

namespace Vanrise.CommonLibrary
{
    public static class Config
    {
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
