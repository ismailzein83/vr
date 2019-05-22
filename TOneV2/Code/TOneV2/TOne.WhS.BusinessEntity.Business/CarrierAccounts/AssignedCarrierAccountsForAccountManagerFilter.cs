using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class AssignedCarrierAccountsForAccountManager : ICarrierAccountFilter
    {
        public bool IsExcluded(ICarrierAccountFilterContext context)
        {
            ConfigManager configManager = new ConfigManager();
            if (!configManager.GetRatePlanCustomerFiltering() && context != null && context.CarrierAccount != null)
            {
                IEnumerable<AccountManagerAssignment> accountManagerAssignments = new List<AccountManagerAssignment>();
                AccountManagerAssignmentManager accountManagerAssignmentManager = new AccountManagerAssignmentManager();

                if (accountManagerAssignmentManager.TryGetEffectiveNowAccountManagerAssignments(out accountManagerAssignments))
                {
                    if (accountManagerAssignments != null && accountManagerAssignments.Count() > 0)
                    {
                        if (!accountManagerAssignments.Any(x => x.CarrierAccountId == context.CarrierAccount.CarrierAccountId))
                            return true;
                    }
                }
            }

            return false;
        }
    }
}
