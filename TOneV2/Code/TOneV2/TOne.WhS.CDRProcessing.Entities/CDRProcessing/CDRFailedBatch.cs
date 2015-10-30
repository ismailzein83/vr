using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Integration.Entities;

namespace TOne.WhS.CDRProcessing.Entities
{
    public class CDRFailedBatch: MappedBatchItem
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

        public override int GetRecordCount()
        {
            return FailedCDRs.Count();
        }
    }
}
