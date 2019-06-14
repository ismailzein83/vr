using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;

namespace TOne.WhS.BusinessEntity.Business
{
    public class AssignedCarrierAccountsForAccountManager : ICarrierAccountFilter
    {
        public ModuleName ModuleName { get; set; }

        public bool IsExcluded(ICarrierAccountFilterContext context)
        {
            context.ThrowIfNull("context");
            context.CarrierAccount.ThrowIfNull("context.CarrierAccount");

            if (TOne.WhS.BusinessEntity.Business.Helper.ShouldFilterCarrierAccount(this.ModuleName))
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
    }
}