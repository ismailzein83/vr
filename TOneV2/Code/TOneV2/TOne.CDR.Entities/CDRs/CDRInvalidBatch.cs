using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Queueing.Entities;

namespace TOne.CDR.Entities
{
    public class CDRInvalidBatch : PersistentQueueItem
    {
        static CDRInvalidBatch()
        {
            Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(CDRInvalidBatch), "InvalidCDRs");
            Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(BillingCDRInvalid), "ID", "Attempt", "Alert", "Connect", "Disconnect", "DurationInSeconds",
                            "CDPN", "CGPN", "Port_OUT", "Port_IN", "ReleaseCode", "ReleaseSource", "SwitchID",
                            "SwitchCdrID", "Tag", "OurZoneID", "SupplierZoneID", "OriginatingZoneID", "SIP", "Extra_Fields",
                            "CustomerID", "SupplierID", "OurCode", "SupplierCode", "IsValid", "IsRerouted", "CDPNOut","SubscriberID");
        }

        public List<BillingCDRInvalid> InvalidCDRs { get; set; }

        public override string GenerateDescription()
        {
            return String.Format("Batch of {0} Invalid CDRs", InvalidCDRs.Count);
        }
    }
}