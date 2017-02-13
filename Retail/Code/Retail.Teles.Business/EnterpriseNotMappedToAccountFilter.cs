using Retail.Teles.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Teles.Business
{
    public class EnterpriseNotMappedToAccountFilter : ITelesEnterpriseFilter
    {
        public dynamic EditedEnterpriseId { get; set; }
        public bool IsExcluded(ITelesEnterpriseFilterContext context)
        {
            TelesEnterpriseManager telesEnterpriseManager = new TelesEnterpriseManager();
            var cachedAccountsByEnterprises = telesEnterpriseManager.GetCachedAccountsByEnterprises(context.AccountBEDefinitionId);
            if (this.EditedEnterpriseId != null && EditedEnterpriseId == context.EnterpriseId)
                return false;
            if (cachedAccountsByEnterprises.ContainsKey(context.EnterpriseId))
               return true;
           return false;
        }
    }
}
