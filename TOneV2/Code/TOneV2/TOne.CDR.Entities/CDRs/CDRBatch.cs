using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vanrise.Queueing.Entities;

namespace TOne.CDR.Entities
{
    public class CDRBatch : PersistentQueueItem
    {
        static CDRBatch()
        {
            Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(CDRBatch), "SwitchId", "CDRs");
            Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(TABS.CDR), "CDRID", "IDonSwitch", "Tag", "AttemptDateTime", "AlertDateTime",
            "ConnectDateTime", "DisconnectDateTime", "Duration", "DurationInSeconds", "IN_TRUNK", "IN_CIRCUIT", "IN_CARRIER",
            "IN_IP", "OUT_TRUNK", "OUT_CIRCUIT", "OUT_CARRIER", "OUT_IP", "CGPN", "CDPN",
            "CDPNOut", "CAUSE_FROM", "CAUSE_FROM_RELEASE_CODE", "CAUSE_TO", "CAUSE_TO_RELEASE_CODE", "Extra_Fields", "IsRerouted",
            "SIP", "OverridedSwitchID");
        }

        public int SwitchId { get; set; }

        public List<TABS.CDR> CDRs { get; set; }

        public override string GenerateDescription()
        {
            return String.Format("Batch of {0} CDRs", CDRs.Count);
        }

        public int GetRecordCount()
        {
            return this.CDRs != null ? this.CDRs.Count : 0;
        }

        public DateTime GetLastRecordTime()
        {
            return this.CDRs != null ? this.CDRs.Max(itm => itm.AttemptDateTime) : default(DateTime);
        }
    }
}
