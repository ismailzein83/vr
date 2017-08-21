using System;
using System.Collections.Generic;
using Retail.BusinessEntity.Entities;

namespace Retail.BusinessEntity.Data
{
    public interface IAccountPackageRecurChargeDataManager : IDataManager
    {
        void ApplyAccountPackageReccuringCharges(List<AccountPackageRecurCharge> accountPackageRecurChargeList, DateTime chargeDay);

        List<AccountPackageRecurCharge> GetAccountRecurringCharges(Guid acountBEDefinitionId, long accountId, DateTime includedFromDate, DateTime includedToDate);

        DateTime? GetMaximumChargeDay();

        List<AccountPackageRecurChargeKey> GetAccountRecurringChargeKeys(DateTime chargeDay);
    }
}