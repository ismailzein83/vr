using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Notification.Entities
{
    public class VRBalanceUpdateLastAlertInfoPayload
    {
        public IVREntityBalanceInfo EntityBalanceInfo { get; set; }

        public Decimal? LastExecutedAlertThreshold { get; set; }
    }

    public class VRBalanceUpdateLastAlertInfoPayloadBatch
    {
        public List<VRBalanceUpdateLastAlertInfoPayload> Items { get; set; }
    }

}
