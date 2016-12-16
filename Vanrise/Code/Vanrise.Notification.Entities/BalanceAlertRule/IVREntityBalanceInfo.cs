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

        Decimal CurrentBalance { get; }

        int? AlertRuleId { get; }

        Decimal? NextAlertThreshold { get; }

        Decimal? LastExecutedAlertThreshold { get; }
        int? ThresholdActionIndex { get; }

        VRBalanceActiveAlertInfo ActiveAlertsInfo { get; }
    }

    public class VREntityBalanceInfoBatch
    {
        public List<IVREntityBalanceInfo> BalanceInfos { get; set; }
    }
}
