using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Queueing.Entities;

namespace TOne.WhS.CDRProcessing.Entities
{
    public class CDR 
    {

        static CDR()
        {
            Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(CDR), "ID", "Attempt", "InCarrier", "InTrunk", "CDPN", "OutCarrier", "OutTrunk", "DurationInSeconds",
                "Alert", "Connect", "Disconnect", "CGPN", "PortOut", "PortIn", "ReleaseCode", "ReleaseSource","SwitchID");
        }
        public DateTime Attempt { get; set; }
        public long ID { get; set; }
        public int SwitchID { get; set; }
        public string InCarrier { get; set; }
        public string InTrunk { get; set; }
        public int DurationInSeconds { get; set; }
        public string OutCarrier { get; set; }
        public string OutTrunk { get; set; }
        public DateTime? Alert { get; set; }
        public DateTime? Connect { get; set; }
        public DateTime? Disconnect { get; set; }
        public String CDPN { get; set; }
        public String CGPN { get; set; }
        public string PortOut { get; set; }
        public string PortIn { get; set; }
        public String ReleaseCode { get; set; }
        public String ReleaseSource { get; set; }

    }
}
