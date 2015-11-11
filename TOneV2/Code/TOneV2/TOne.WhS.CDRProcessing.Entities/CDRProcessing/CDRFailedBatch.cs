using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Integration.Entities;
using Vanrise.Queueing.Entities;

namespace TOne.WhS.CDRProcessing.Entities
{
    public class CDRFailedBatch : PersistentQueueItem
    {
        static CDRFailedBatch()
        {
            BillingFailedCDR cdr = new BillingFailedCDR();
            Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(CDRFailedBatch), "FailedCDRs");
        }
        public List<BillingFailedCDR> FailedCDRs { get; set; }
        public override string GenerateDescription()
        {
            return String.Format("CDRFailedBatch of {0} CDRs", FailedCDRs.Count());
        }

    }
}
