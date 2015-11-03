using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.CDRProcessing.Entities
{
    public class BillingFailedCDR : BillingCDRBase
    {
        static BillingFailedCDR()
        {
            Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(BillingFailedCDR), "ID", "Attempt", "CustomerId", "SupplierId", "DurationInSeconds",
                "Alert", "Connect", "Disconnect", "CGPN", "PortOut", "PortIn", "ReleaseCode", "ReleaseSource", "SaleZoneID", "SupplierZoneID", "OriginatingZoneID", "SaleCode"
                , "SupplierCode");
        }
        public BillingFailedCDR(BillingCDRBase copy) : base(copy) { }
        public BillingFailedCDR() { }
    }
}
