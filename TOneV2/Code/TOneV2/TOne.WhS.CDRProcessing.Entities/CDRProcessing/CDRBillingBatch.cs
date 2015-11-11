using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Integration.Entities;
using Vanrise.Queueing.Entities;

namespace TOne.WhS.CDRProcessing.Entities
{
    public class CDRBillingBatch : PersistentQueueItem
   {
       static CDRBillingBatch()
       {           
           BillingCDRBase cdr = new BillingCDRBase();
           Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(CDRBillingBatch), "CDRs");
       }
       public List<BillingCDRBase> CDRs { get; set; }


       public override string GenerateDescription()
       {
           return String.Format("Batch of {0} CDRs", CDRs.Count());
       }
   }
}
