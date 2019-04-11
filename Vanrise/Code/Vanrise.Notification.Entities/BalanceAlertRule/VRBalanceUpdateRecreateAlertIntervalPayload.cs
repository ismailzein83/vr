using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Notification.Entities
{
    public class VRBalanceUpdateRecreateAlertIntervalPayload
    {
        public IVREntityBalanceInfo EntityBalanceInfo { get; set; }

        public TimeSpan? RecreateAlertAfter { get; set; }
    }

    public class VRBalanceUpdateRecreateAlertIntervalPayloadBatch
    {
        public List<VRBalanceUpdateRecreateAlertIntervalPayload> Items { get; set; }
    }
}
