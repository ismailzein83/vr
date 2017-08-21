using System;
using System.Collections.Generic;
using Retail.BusinessEntity.Data;
using Retail.BusinessEntity.Entities;

namespace Retail.BusinessEntity.Business
{
    public class RecurChargeBalanceUpdateSummaryManager
    {
        public List<RecurChargeBalanceUpdateSummary> GetRecurChargeBalanceUpdateSummaryList(DateTime chargeDay)
        {
            IRecurChargeBalanceUpdateSummaryDataManager dataManager = BEDataManagerFactory.GetDataManager<IRecurChargeBalanceUpdateSummaryDataManager>();
            return dataManager.GetRecurChargeBalanceUpdateSummaryList(chargeDay);
        }


        public void Insert(RecurChargeBalanceUpdateSummary recurChargeBalanceUpdateSummary)
        {
            IRecurChargeBalanceUpdateSummaryDataManager dataManager = BEDataManagerFactory.GetDataManager<IRecurChargeBalanceUpdateSummaryDataManager>();
            dataManager.Insert(recurChargeBalanceUpdateSummary);
        }

        public void Delete(DateTime chargeDay)
        {
            IRecurChargeBalanceUpdateSummaryDataManager dataManager = BEDataManagerFactory.GetDataManager<IRecurChargeBalanceUpdateSummaryDataManager>();
            dataManager.Delete(chargeDay);
        }

        public DateTime? GetMaximumChargeDay()
        {
            IRecurChargeBalanceUpdateSummaryDataManager dataManager = BEDataManagerFactory.GetDataManager<IRecurChargeBalanceUpdateSummaryDataManager>();
            return dataManager.GetMaximumChargeDay();
        }
    }
}