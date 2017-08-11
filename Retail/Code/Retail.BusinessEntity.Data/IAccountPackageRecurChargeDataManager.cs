using System;
using System.Collections.Generic;
using Retail.BusinessEntity.Entities;

namespace Retail.BusinessEntity.Data
{
    public interface IAccountPackageRecurChargeDataManager : IDataManager
    {
        List<AccountPackageRecurCharge> GetAccountPackageRecurChargesNotSent(DateTime effectiveDate);

        void ApplyAccountPackageReccuringCharges(List<AccountPackageRecurCharge> accountPackageRecurChargeList, DateTime effectiveDate, long processInstanceId);
    }
}