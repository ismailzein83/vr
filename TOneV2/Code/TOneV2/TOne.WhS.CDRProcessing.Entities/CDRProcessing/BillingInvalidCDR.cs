using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.CDRProcessing.Entities
{
    public class BillingInvalidCDR
    {
        static BillingInvalidCDR()
        {
            BillingCDRBase BillingCDR = new BillingCDRBase();
            Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(BillingInvalidCDR), "BillingCDR");
        }
      
        public BillingCDRBase BillingCDR { get; set; }
    }
}
