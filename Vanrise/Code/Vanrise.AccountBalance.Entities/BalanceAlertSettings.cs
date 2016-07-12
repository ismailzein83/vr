using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vanrise.Entities;

namespace Vanrise.AccountBalance.Entities
{
    public class BalanceAlertSettings
    {
        public List<BalanceAlertThresholdAction> ThresholdAction { get; set; }
    }

    public class BalanceAlertThresholdAction
    {
        public BalanceAlertThreshold Threshold { get; set; }

        public List<VRAction> Actions { get; set; }
    }

    public abstract class BalanceAlertThreshold
    {
        public int ConfigId { get; set; }

        public abstract Decimal GetThreshold(IBalanceAlertThresholdContext context);
    }

    public interface IBalanceAlertThresholdContext
    {
        dynamic Account { get; }
    }
}
