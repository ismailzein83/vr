using Retail.Teles.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Teles.Business
{
    public class TelesEnterpriseManager
    {
        public IEnumerable<TelesEnterpriseInfo> GetEnterprisesInfo(int switchId, int domainId, TelesEnterpriseFilter filter)
        {
            TelesWebAPIClient telesWebAPIClient = new TelesWebAPIClient();
            var actionPath = string.Format("/SIPManagement/rest/v1/domain/{0}/sub",domainId);
            List<dynamic> enterprises  = telesWebAPIClient.Get<List<dynamic>>(switchId, actionPath);
            List<TelesEnterpriseInfo> telesEnterpriseInfo = new List<TelesEnterpriseInfo>();
            if(enterprises != null)
            {
                foreach(var enterprise in enterprises)
                {
                    telesEnterpriseInfo.Add(new TelesEnterpriseInfo
                    {
                        Name = enterprise.name,
                        TelesEnterpriseId = enterprise.id
                    });
                }
            }
            return telesEnterpriseInfo;
        }
    }
}
