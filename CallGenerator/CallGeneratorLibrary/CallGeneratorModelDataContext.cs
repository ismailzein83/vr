using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace CallGeneratorLibrary
{
    public partial class CallGeneratorModelDataContext
    {
        
        public static string connectionString
        {
            get {
                if (ConfigurationManager.AppSettings["CallGeneratorConnectionString"] == null)
                    return ConfigurationManager.ConnectionStrings["CallGeneratorConnectionString"].ConnectionString;
                else
                    return ConfigurationManager.AppSettings["CallGeneratorConnectionString"];
            }
        }

        public CallGeneratorModelDataContext() : base(connectionString, mappingSource)
        {
            OnCreated();
        }
    }
}
