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
        public DataTypeEnum? DataType { get; set; }
        public bool IsExcluded(ICarrierAccountFilterContext context)
        {
            if(DataType.HasValue)
            {
                ConfigManager configManager = new ConfigManager();

                var carrierAccount = context.CarrierAccount;

                bool getFilteredData = false;
                if (this.DataType.Value == DataTypeEnum.Traffic)
                    getFilteredData = configManager.GetTrafficCarrierAccountFiltering();
                else if (this.DataType.Value == DataTypeEnum.Billing)
                    getFilteredData = configManager.GetBillingCarrierAccountFiltering();

                if (!getFilteredData)
                    return false;
                else
                {
                    AccountManagerAssignmentManager accountManagerAssignmentManager = new AccountManagerAssignmentManager();
                    IEnumerable<AccountManagerAssignment> accountManagerAssignments;
                    bool result = accountManagerAssignmentManager.TryGetCurrentUserAccountManagerAssignments(out accountManagerAssignments);
                    if (accountManagerAssignments != null)
                    {
                        foreach (var accountManagerAssignement in accountManagerAssignments)
                        {
                            if (accountManagerAssignement.CarrierAccountId == carrierAccount.CarrierAccountId)
                                return false;
                        }
                    }
                    return true;
                }
            }
            return false;
        }

        public override bool IsMatched(IBERuntimeSelectorFilterSelectorFilterContext context)
        {
            if (DataType.HasValue)
            {
                ConfigManager configManager = new ConfigManager();

                bool getFilteredData = false;
                if (this.DataType.Value == DataTypeEnum.Traffic)
                    getFilteredData = configManager.GetTrafficCarrierAccountFiltering();
                else if (this.DataType.Value == DataTypeEnum.Billing)
                    getFilteredData = configManager.GetBillingCarrierAccountFiltering();

                if (!getFilteredData)
                    return true;
                else
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
                    return false;
                }
            }

            return true;
        }
    }
}
