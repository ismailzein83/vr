using Retail.BusinessEntity.Business;
using Retail.Teles.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
namespace Retail.Teles.Business
{
    public class SitesNotMappedToAccountFilter : ITelesEnterpriseSiteFilter
    {
        public string EditedEnterpriseSiteId { get; set; }
        public long AccountId { get; set; }
        public bool IsExcluded(ITelesEnterpriseSiteFilterContext context)
        {
            AccountBEManager accountBEManager = new BusinessEntity.Business.AccountBEManager();
            var account = accountBEManager.GetAccount(context.AccountBEDefinitionId,this.AccountId);
            account.ThrowIfNull("account",this.AccountId);
            if(!account.ParentAccountId.HasValue)
             throw  new NullReferenceException("account.ParentAccountId");
            EnterpriseAccountMappingInfo enterpriseAccountMappingInfo = new AccountBEManager().GetExtendedSettings<EnterpriseAccountMappingInfo>(context.AccountBEDefinitionId, account.ParentAccountId.Value);
            enterpriseAccountMappingInfo.ThrowIfNull("enterpriseAccountMappingInfo");
            TelesSiteManager telesSiteManager = new TelesSiteManager();
            var cachedAccountsBySites = telesSiteManager.GetCachedAccountsBySites(context.AccountBEDefinitionId, enterpriseAccountMappingInfo.TelesEnterpriseId);
            if (this.EditedEnterpriseSiteId != null && EditedEnterpriseSiteId == context.EnterpriseSiteId)
                return false;
            if (cachedAccountsBySites != null && cachedAccountsBySites.ContainsKey(context.EnterpriseSiteId))
                return true;
            return false;
        }
    }
}
