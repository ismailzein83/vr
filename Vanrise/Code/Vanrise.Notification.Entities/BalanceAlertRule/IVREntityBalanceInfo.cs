﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Notification.Entities
{
    public interface IVREntityBalanceInfo
    {
        string EntityId { get; }

        string EntityName { get; }

        Decimal CurrentBalance { get; }

        int CurrencyId { get; }

        int? AlertRuleId { get; }

        Decimal? NextAlertThreshold { get; }

        Decimal? LastExecutedAlertThreshold { get; }

        VRBalanceActiveAlertInfo ActiveAlertsInfo { get; }
    }

    public class VREntityBalanceInfoBatch
    {
        public List<IVREntityBalanceInfo> BalanceInfos { get; set; }
    }
}
