using System;
using System.Collections.Generic;
using Retail.BusinessEntity.Entities;

namespace Retail.BusinessEntity.Data
{
    public interface IRecurChargeBalanceUpdateSummaryDataManager : IDataManager
    {
        List<RecurChargeBalanceUpdateSummary> GetRecurChargeBalanceUpdateSummaryList(DateTime chargeDay);

        void Insert(RecurChargeBalanceUpdateSummary recurChargeBalanceUpdateSummary);

        void Delete(DateTime chargeDay);

        DateTime? GetMaximumChargeDay();
    }
}