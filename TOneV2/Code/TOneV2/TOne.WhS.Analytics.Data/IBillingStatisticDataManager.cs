﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Analytics.Entities.BillingReport;

namespace TOne.WhS.Analytics.Data
{
    public interface IBillingStatisticDataManager : IDataManager
    {
        List<ZoneProfit> GetZoneProfit(DateTime fromDate, DateTime toDate, string customerIds, string supplierIds, int currencyId);
    }
}
