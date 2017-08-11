﻿using System;
using System.Collections.Generic;
using Retail.BusinessEntity.Data;
using Retail.BusinessEntity.Entities;

namespace Retail.BusinessEntity.Business
{
    public class AccountPackageRecurChargeManager
    {
        public void ApplyAccountPackageReccuringCharges(List<AccountPackageRecurCharge> accountPackageRecurChargeList, DateTime effectiveDate, long processInstanceId)
        {
            IAccountPackageRecurChargeDataManager dataManager = BEDataManagerFactory.GetDataManager<IAccountPackageRecurChargeDataManager>();
            dataManager.ApplyAccountPackageReccuringCharges(accountPackageRecurChargeList, effectiveDate, processInstanceId);
        }

        public List<AccountPackageRecurCharge> GetAccountPackageRecurChargesNotSent(DateTime effectiveDate)
        {
            IAccountPackageRecurChargeDataManager dataManager = BEDataManagerFactory.GetDataManager<IAccountPackageRecurChargeDataManager>();
            return dataManager.GetAccountPackageRecurChargesNotSent(effectiveDate);
        }
    }
}