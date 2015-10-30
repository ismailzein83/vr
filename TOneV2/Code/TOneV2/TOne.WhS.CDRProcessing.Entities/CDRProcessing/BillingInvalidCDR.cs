using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.CDRProcessing.Entities
{
    public class BillingInvalidCDR : BillingCDRBase
    {
        static BillingInvalidCDR()
        {
            Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(BillingInvalidCDR), "ID", "Attempt", "CustomerId", "SupplierId");
        }
    }
}
