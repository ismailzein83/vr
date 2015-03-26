﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BI.Entities;

namespace TOne.BI.Data
{
    public interface ISalesDataManager : IDataManager
    {
        IEnumerable<ProfitInfo> GetProfit(TimeDimensionType timeDimensionType, DateTime fromDate, DateTime toDate);
    }
}
