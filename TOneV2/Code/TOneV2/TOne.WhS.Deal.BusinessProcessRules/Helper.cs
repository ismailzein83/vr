using System;
using TOne.WhS.Deal.Business;

namespace TOne.WhS.Deal.BusinessProcessRules
{
    public static class Helper
    {
        public static string GetDealZoneMessage(int carrierId, long zoneId, string zoneName, DateTime effectiveDate, bool isSale)
        {
            var dealDefinitionManager = new DealDefinitionManager();
            var dealId = dealDefinitionManager.IsZoneIncludedInDeal(carrierId, zoneId, effectiveDate, isSale);
            if (dealId.HasValue)
            {
                var deal = new DealDefinitionManager().GetDealDefinition(dealId.Value);
                return string.Format("zone '{0}' in deal '{1}'", zoneName, deal.Name);
            }
            return null;
        }
    }
}
