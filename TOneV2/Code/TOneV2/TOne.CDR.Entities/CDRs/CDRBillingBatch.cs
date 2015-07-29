using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Queueing.Entities;

namespace TOne.CDR.Entities
{
    public class CDRBillingBatch : PersistentQueueItem
    {
        static CDRBillingBatch()
        {
            Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(CDRBillingBatch), "SwitchId", "CDRs");
            Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(BillingCDRBase), "IsValid", "IsRerouted", "CDPNOut", "OurCode", "SupplierCode", "SubscriberID",
                "ID", "Attempt", "Alert", "Connect", "Disconnect", "DurationInSeconds", "CDPN",
                "CGPN", "Port_OUT", "Port_IN", "ReleaseCode", "ReleaseSource", "SwitchID", "SwitchCdrID",
                "Tag", "Extra_Fields", "CustomerID", "SupplierID", "OurZoneID", "SupplierZoneID", "OriginatingZoneID","SIP");
        }

        public int SwitchId { get; set; }

        public List<BillingCDRBase> CDRs { get; set; }

        public override string GenerateDescription()
        {
            return String.Format("Batch of {0} Billing CDRs", CDRs.Count);
        }
    }
}
