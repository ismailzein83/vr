using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Integration.Entities;

namespace TOne.WhS.CDRProcessing.Entities
{
   public class CDRBillingBatch: MappedBatchItem
   {
       static CDRBillingBatch()
       {
           BillingCDRBase cdr = new BillingCDRBase();
           Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(CDRBillingBatch), "CDRs");
       }
       public List<BillingCDRBase> CDRs { get; set; }

       public override int GetRecordCount()
       {
           return CDRs.Count() ;
       }

       public override string GenerateDescription()
       {
           return String.Format("Batch of {0} CDRs", CDRs.Count());
       }
   }
}
