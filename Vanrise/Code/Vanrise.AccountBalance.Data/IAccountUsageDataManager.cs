﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Entities;

namespace Vanrise.AccountBalance.Data
{
    public interface IAccountUsageDataManager:IDataManager
    {
        IEnumerable<AccountUsageInfo> GetAccountsUsageInfoByPeriod(Guid accountTypeId, DateTime periodStart);
        AccountUsageInfo TryAddAccountUsageAndGet(Guid accountTypeId, long accountId, DateTime periodStart, DateTime periodEnd, decimal usageBalance);

    }
}
