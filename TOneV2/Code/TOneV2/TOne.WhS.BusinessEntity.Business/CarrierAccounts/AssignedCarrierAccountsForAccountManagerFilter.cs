using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;

namespace TOne.WhS.BusinessEntity.Business
{
    public enum ModuleName { RatePlan = 0, CustomerRoute = 1, ProductRoute = 2 }

    public class AssignedCarrierAccountsForAccountManager : ICarrierAccountFilter
    {
        public ModuleName ModuleName { get; set; }

        public bool IsExcluded(ICarrierAccountFilterContext context)
        {
            context.ThrowIfNull("context");
            context.CarrierAccount.ThrowIfNull("context.CarrierAccount");

            if (ShouldFilterCarrierAccount())
            {
                List<int> carrierAccountIds;
                AccountManagerAssignmentManager accountManagerAssignmentManager = new AccountManagerAssignmentManager();

                if (accountManagerAssignmentManager.TryGetCurrentUserEffectiveNowCarrierAccountIds(out carrierAccountIds))
                {
                    if (carrierAccountIds == null || carrierAccountIds.Count() == 0)
                        return true;

                    if (!carrierAccountIds.Contains(context.CarrierAccount.CarrierAccountId))
                        return true;
                }
            }

            return false;
        }

        private bool ShouldFilterCarrierAccount()
        {
            ConfigManager configManager = new ConfigManager();
            switch (this.ModuleName)
            {
                case ModuleName.RatePlan:
                    return configManager.GetRatePlanCarrierAccountFiltering();
                case ModuleName.CustomerRoute:
                    return configManager.GetCustomerRouteCarrierAccountFiltering();
                //case ModuleName.ProductRoute:
                //    return configManager.GetProductRouteCarrierAccountFiltering();
                default:
                    return false;
            }
        }
    }
}