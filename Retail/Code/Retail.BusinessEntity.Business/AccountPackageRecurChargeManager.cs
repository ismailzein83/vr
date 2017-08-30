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

        public Dictionary<long, List<AccountPackageRecurCharge>> GetAccountRecurringChargesByAccountPackage(List<AccountPackageRecurChargePeriod> accountPackageRecurChargePeriods)
        {
            IAccountPackageRecurChargeDataManager dataManager = BEDataManagerFactory.GetDataManager<IAccountPackageRecurChargeDataManager>();
            List<AccountPackageRecurCharge> accountPackageRecurCharges = dataManager.GetAccountRecurringCharges(accountPackageRecurChargePeriods);
            if (accountPackageRecurCharges == null)
                return null;

            Dictionary<long, List<AccountPackageRecurCharge>> accountRecurringChargesByAccountPackage = new Dictionary<long, List<AccountPackageRecurCharge>>();
            foreach (AccountPackageRecurCharge accountPackageRecurCharge in accountPackageRecurCharges)
            {
                List<AccountPackageRecurCharge> accountPackageRecurChargeList = accountRecurringChargesByAccountPackage.GetOrCreateItem(accountPackageRecurCharge.AccountPackageID);
                accountPackageRecurChargeList.Add(accountPackageRecurCharge);
            }

            return accountRecurringChargesByAccountPackage;
        }

        public DateTime? GetMaximumChargeDay()
        {
            IAccountPackageRecurChargeDataManager dataManager = BEDataManagerFactory.GetDataManager<IAccountPackageRecurChargeDataManager>();
            return dataManager.GetMaximumChargeDay();
        }

        public HashSet<AccountPackageRecurChargeKey> GetAccountRecurringChargeKeys(DateTime chargeDay)
        {
            IAccountPackageRecurChargeDataManager dataManager = BEDataManagerFactory.GetDataManager<IAccountPackageRecurChargeDataManager>();
            List<AccountPackageRecurChargeKey> accountPackageRecurChargeKeyList = dataManager.GetAccountRecurringChargeKeys(chargeDay);

            if (accountPackageRecurChargeKeyList == null)
                return null;

            return accountPackageRecurChargeKeyList.ToHashSet();
        }

        public int GetChargeAmountPrecision()
        {
            IAccountPackageRecurChargeDataManager dataManager = BEDataManagerFactory.GetDataManager<IAccountPackageRecurChargeDataManager>();
            return dataManager.GetChargeAmountPrecision();
        }
    }
}