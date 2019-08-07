using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using BPMExtended.Main.Common;
using BPMExtended.Main.Entities;
using Newtonsoft.Json;
using Terrasoft.Core;
using Terrasoft.Core.DB;
using Terrasoft.Core.Entities;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ServiceModel.Activation;
using Terrasoft.Common;
using Terrasoft.Configuration;

namespace BPMExtended.Main.Business
{
    public class SecurityManager
    {
        public UserConnection BPM_UserConnection
        {
            get
            {
                return (UserConnection)HttpContext.Current.Session["UserConnection"];
            }
        }
        public bool GetPermission(string operationPermission)
        {
            bool result = false;
            ConfigurationServiceResponse response = new ConfigurationServiceResponse();
            try
            {
                BPM_UserConnection.DBSecurityEngine.CheckCanExecuteOperation(operationPermission);
                result = true;
            }
            catch (Exception e)
            {
                response.Exception = e;
                result = false;
            }
            return result;
        }
    }
}
