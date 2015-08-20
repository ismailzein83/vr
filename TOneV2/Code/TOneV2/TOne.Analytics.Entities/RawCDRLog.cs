using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Entities
{
    public class RawCDRLog
    {
        public int CDRID { get; set; }
        public string SwitchName { get; set; }
        public long IDonSwitch { get; set; }
        public string Tag { get; set; }
        public DateTime AttemptDateTime { get; set; }
        public DateTime AlertDateTime { get; set; }
        public DateTime ConnectDateTime { get; set; }
        public DateTime DisconnectDateTime { get; set; }
        public Decimal DurationInSeconds { get; set; }
        public string IN_TRUNK { get; set; }
        public long IN_CIRCUIT { get; set; }
        public string IN_CARRIER { get; set; }
        public string IN_IP { get; set; }
        public string OUT_TRUNK { get; set; }
        public int OUT_CIRCUIT { get; set; }
        public string OUT_CARRIER { get; set; }
        public string OUT_IP { get; set; }
        public string CGPN { get; set; }
        public string CDPN { get; set; }
        public string CDPNOut { get; set; }
        public string CAUSE_FROM_RELEASE_CODE { get; set; }
        public string CAUSE_FROM { get; set; }
        public string CAUSE_TO_RELEASE_CODE { get; set; }

        public string CAUSE_TO { get; set; }
        public string Extra_Fields { get; set; }

        public char IsRerouted { get; set; }

    }
}
