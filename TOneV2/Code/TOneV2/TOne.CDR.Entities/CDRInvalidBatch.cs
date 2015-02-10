using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Queueing;

namespace TOne.CDR.Entities
{
    public class CDRInvalidBatch : PersistentQueueItem
    {
        public List<TABS.Billing_CDR_Invalid> InvalidCDRs { get; set; }

        public override string GenerateDescription()
        {
            return String.Format("Batch of {0} Invalid CDRs", InvalidCDRs.Count);
        }
    }
}