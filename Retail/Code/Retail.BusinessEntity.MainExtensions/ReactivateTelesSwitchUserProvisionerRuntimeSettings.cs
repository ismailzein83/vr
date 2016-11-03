using Retail.BusinessEntity.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Integration.Entities;

namespace Retail.BusinessEntity.MainExtensions
{
    public class ReactivateTelesSwitchUserProvisionerRuntimeSettings : ActionProvisioner
    {
        public override void Execute(IActionProvisioningContext context)
        {
            RequestManager manager = new RequestManager();
            ProvisioningData data = null;
            if (context.ExecutedActionsData != null)
            {
                foreach (var action in context.ExecutedActionsData)
                {
                    data = action as ProvisioningData;
                    if (data != null)
                        break;
                }
                string url = String.Format("https://c5-iot2-prov.teles.de/SIPManagement/rest/v1/domain/{0}/user?searchCol=loginName&search={1}", data.Domain, data.LoginName);
                var response = manager.GetRequest(url, null);

                var users = Vanrise.Common.Serializer.Deserialize<List<dynamic>>(response);

                if (users != null)
                {
                    foreach (var user in users)
                    {
                        if (data.LoginName.Equals(user.loginName))
                        {
                            user.state = "ACTIVE";
                            string url1 = String.Format("https://c5-iot2-prov.teles.de/SIPManagement/rest/v1/domain/{0}/user/{1}", data.Domain, user.id);
                            string userNameData = Vanrise.Common.Serializer.Serialize(user, true);
                            var userData = Encoding.UTF8.GetBytes(userNameData);
                            manager.PutRequest(url1, userData);
                        }
                        context.ExecutionOutput = new ActionProvisioningOutput
                        {
                            Result = ActionProvisioningResult.Succeeded,
                        };
                    }

                }
                else
                {
                    context.ExecutionOutput = new ActionProvisioningOutput
                    {
                        Result = ActionProvisioningResult.Failed,
                    };
                }
            }
        }
    }
}
