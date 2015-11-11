using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Integration.Entities;
using Vanrise.Queueing.Entities;

namespace TOne.WhS.CDRProcessing.Entities
{
    public class CDRInvalidBatch : PersistentQueueItem
    {
        static CDRInvalidBatch()
        {
            BillingInvalidCDR cdr = new BillingInvalidCDR();
            Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(CDRInvalidBatch), "InvalidCDRs");
        }
        public List<BillingInvalidCDR> InvalidCDRs { get; set; }
        public override string GenerateDescription()
        {
            return String.Format("CDRInvalidBatch of {0} CDRs", InvalidCDRs.Count());
        }

    }
}
