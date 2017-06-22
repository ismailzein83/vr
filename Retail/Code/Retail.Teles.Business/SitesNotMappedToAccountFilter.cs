using Retail.Teles.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Teles.Business
{
    public class SitesNotMappedToAccountFilter : ITelesEnterpriseSiteFilter
    {
        public dynamic EditedEnterpriseSiteId { get; set; }
        public int EnterpriseId { get; set; }
        public bool IsExcluded(ITelesEnterpriseSiteFilterContext context)
        {
            TelesSiteManager telesSiteManager = new TelesSiteManager();
            var cachedAccountsBySites = telesSiteManager.GetCachedAccountsBySites(context.AccountBEDefinitionId, this.EnterpriseId);
            if (this.EditedEnterpriseSiteId != null && EditedEnterpriseSiteId == context.EnterpriseSiteId)
                return false;
            if (cachedAccountsBySites != null && cachedAccountsBySites.ContainsKey(context.EnterpriseSiteId))
                return true;
            return false;
        }
    }
}
