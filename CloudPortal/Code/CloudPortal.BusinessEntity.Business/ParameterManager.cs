using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudPortal.BusinessEntity.Business
{
    public class ParameterManager
    {
        public string GetCloudPortalInternalURL()
        {
            return ConfigurationManager.AppSettings["CloudPortal_InternalURL"];
        }

        public string GetCloudPortalOnlineURL()
        {
            return ConfigurationManager.AppSettings["CloudPortal_OnlineURL"];
        }
    }
}
