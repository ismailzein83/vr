using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Notification.Entities
{
    public class VRBalanceUpdateRuleInfoPayload
    {
        public IVREntityBalanceInfo EntityBalanceInfo { get; set; }

        public long? AlertRuleId { get; set; }

        public Decimal? NextAlertThreshold { get; set; }
    }

    public class VRBalanceUpdateRuleInfoPayloadBatch
    {
        public VRBalanceUpdateRuleInfoPayloadBatch()
        {
            this.Items = new List<VRBalanceUpdateRuleInfoPayload>();
        }
        public List<VRBalanceUpdateRuleInfoPayload> Items { get; set; }
    }
}
