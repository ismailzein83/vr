using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.CDRProcessing.Entities
{
    public class BillingMainCDR : BillingCDRBase
    {
        static BillingMainCDR()
        {
            Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(BillingMainCDR), "ID", "Attempt", "CustomerId", "SupplierId");
        }
    }
}
