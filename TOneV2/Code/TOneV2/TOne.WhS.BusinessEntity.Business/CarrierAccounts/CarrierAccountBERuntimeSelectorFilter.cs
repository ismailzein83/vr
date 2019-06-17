using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.GenericData.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class CarrierAccountBERuntimeSelectorFilter : BERuntimeSelectorFilter, ICarrierAccountFilter
    {
        public DataTypeEnum DataType { get; set; }
        public bool IsExcluded(ICarrierAccountFilterContext context)
        {
            throw new NotImplementedException();
        }

        public override bool IsMatched(IBERuntimeSelectorFilterSelectorFilterContext context)
        {
            ConfigManager configManager = new ConfigManager();

            bool getFilteredData = false;
            if (this.DataType == DataTypeEnum.Traffic)
                getFilteredData = configManager.GetTrafficCarrierAccountFiltering();
            else if (this.DataType == DataTypeEnum.Billing)
                getFilteredData = configManager.GetBillingCarrierAccountFiltering();

            if (getFilteredData)
            {
                AccountManagerAssignmentManager accountManagerAssignmentManager = new AccountManagerAssignmentManager();
                IEnumerable<AccountManagerAssignment> accountManagerAssignments;
                bool result = accountManagerAssignmentManager.TryGetCurrentUserAccountManagerAssignments(out accountManagerAssignments);
                if (accountManagerAssignments != null)
                {
                    foreach (var accountManagerAssignement in accountManagerAssignments)
                    {
                        if (accountManagerAssignement.CarrierAccountId == (int)context.BusinessEntityId)
                            return true;
                    }
                }
            }
            return false;
        }
    }
}
