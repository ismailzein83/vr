using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.GenericData.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class FinancialAccountDefinitionFilter : IWHSFinancialAccountDefinitionFilter
    {
        public int? CarrierAccountId { get; set; }
        public int? CarrierProfileId { get; set; }
        public int? FinancialAccountId { get; set; }
        public bool IsMatched(IWHSFinancialAccountDefinitionFilterContext context)
        {
            WHSFinancialAccountManager financialAccountManager = new WHSFinancialAccountManager();
            List<WHSCarrierFinancialAccountData> matchExistingCustomerFinancialAccounts;
            List<WHSCarrierFinancialAccountData> matchExistingSupplierFinancialAccounts;
            financialAccountManager.GetMatchExistingFinancialAccounts(this.CarrierAccountId, this.CarrierProfileId, out matchExistingCustomerFinancialAccounts, out matchExistingSupplierFinancialAccounts);
            return financialAccountManager.CanSelectFinAccDefInAddOrUpdate(this.FinancialAccountId, context.FinancialAccountDefinitionId, this.CarrierAccountId, this.CarrierProfileId, matchExistingCustomerFinancialAccounts, matchExistingSupplierFinancialAccounts);
        }
    }
}
