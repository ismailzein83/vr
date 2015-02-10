using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vanrise.Queueing;

namespace TOne.CDR.Entities
{
    public class CDRBatch : PersistentQueueItem
    {
        static CDRBatch()
        {
            Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(CDRBatch), "CDRs");
            //Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(TABS.CDR), "CDRID", "AlertDateTime", "AttemptDateTime", "CAUSE_TO",
            //        "CAUSE_TO_RELEASE_CODE", "CDPN", "CDPNOut", "CGPN", "ConnectDateTime", "DisconnectDateTime", "Duration",
            //        "DurationInSeconds", "Extra_Fields", "IDonSwitch", "IN_CARRIER", "IN_CIRCUIT", "IN_IP", "IN_TRUNK",
            //        "IsRerouted", "OUT_CARRIER", "OUT_CIRCUIT", "OUT_IP", "OUT_TRUNK", "OverridedSwitchID", "SIP",
            //        "Tag", "Sub");
        }

        public List<TABS.CDR> CDRs { get; set; }

        public override string GenerateDescription()
        {
            return String.Format("Batch of {0} CDRs", CDRs.Count);
        }
    }
}
