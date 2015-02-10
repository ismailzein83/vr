using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Queueing;

namespace TOne.CDR.Entities
{
    public class CDRBillingBatch : PersistentQueueItem
    {
        static CDRBillingBatch()
        {
            Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(CDRBillingBatch), "SwitchId", "CDRs");
            Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(TABS.Billing_CDR_Base), "IsValid", "IsRerouted", "CDPNOut", "PDDInSeconds", "OurCode", "SupplierCode",
                "SubscriberID", "ID", "Attempt", "Alert", "Connect", "Disconnect", "DurationInSeconds",
                "CDPN", "CGPN", "Port_OUT", "Port_IN", "ReleaseCode", "ReleaseSource", "Switch",
                "SwitchCdrID", "Tag", "Extra_Fields", "CustomerID", "SupplierID", "Customer", "Supplier",
                "OurZone", "SupplierZone", "OriginatingZone", "SIP", "UserTrackingEnabled", "Identifier");
        }

      public int SwitchId { get; set; }

      public List<TABS.Billing_CDR_Base> CDRs { get; set; }

      public override string GenerateDescription()
      {
          return String.Format("Batch of {0} Billing CDRs", CDRs.Count);
      }
    }
}
