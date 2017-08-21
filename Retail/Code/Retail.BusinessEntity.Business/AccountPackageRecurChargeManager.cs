using System;
using System.Collections.Generic;
using Vanrise.Common;
using Retail.BusinessEntity.Data;
using Retail.BusinessEntity.Entities;

namespace Retail.BusinessEntity.Business
{
    public class AccountPackageRecurChargeManager
    {
        public void ApplyAccountPackageReccuringCharges(List<AccountPackageRecurCharge> accountPackageRecurChargeList, DateTime chargeDay)
        {
            IAccountPackageRecurChargeDataManager dataManager = BEDataManagerFactory.GetDataManager<IAccountPackageRecurChargeDataManager>();
            dataManager.ApplyAccountPackageReccuringCharges(accountPackageRecurChargeList, chargeDay);
        }

        public List<AccountPackageRecurCharge> GetAccountRecurringCharges(Guid acountBEDefinitionId, long accountId, DateTime includedFromDate, DateTime includedToDate)
        {
            IAccountPackageRecurChargeDataManager dataManager = BEDataManagerFactory.GetDataManager<IAccountPackageRecurChargeDataManager>();
            return dataManager.GetAccountRecurringCharges(acountBEDefinitionId, accountId, includedFromDate, includedToDate);
        }

        public DateTime? GetMaximumChargeDay()
        {
            IAccountPackageRecurChargeDataManager dataManager = BEDataManagerFactory.GetDataManager<IAccountPackageRecurChargeDataManager>();
            return dataManager.GetMaximumChargeDay();
        }

        public HashSet<AccountPackageRecurChargeKey> GetAccountRecurringChargeKeys(DateTime chargeDay)
        {
            IAccountPackageRecurChargeDataManager dataManager = BEDataManagerFactory.GetDataManager<IAccountPackageRecurChargeDataManager>();
            List<AccountPackageRecurChargeKey> accountPackageRecurChargeKeyList= dataManager.GetAccountRecurringChargeKeys(chargeDay);

            if (accountPackageRecurChargeKeyList == null)
                return null;

            return accountPackageRecurChargeKeyList.ToHashSet();
        }
    }
}