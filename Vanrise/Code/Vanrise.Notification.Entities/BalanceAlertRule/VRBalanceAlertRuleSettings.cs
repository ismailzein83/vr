using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Notification.Entities
{
    public class VRBalanceAlertRuleSettings : VRGenericAlertRuleExtendedSettings
    {
        public List<VRBalanceAlertThresholdAction> ThresholdActions { get; set; }
    }

    public class VRBalanceAlertThresholdAction
    {
        public Guid AlertLevelId { get; set; }

        public VRBalanceAlertThreshold Threshold { get; set; }

        public List<VRAction> Actions { get; set; }

        public List<VRAction> RollbackActions { get; set; }
    }

    public abstract class VRBalanceAlertThreshold
    {
        public abstract Guid ConfigId { get; }

        public abstract Decimal GetThreshold(IVRBalanceAlertThresholdContext context);
    }

    public interface IVRBalanceAlertThresholdContext
    {
        IVREntityBalanceInfo EntityBalanceInfo { get; }
    }
}
