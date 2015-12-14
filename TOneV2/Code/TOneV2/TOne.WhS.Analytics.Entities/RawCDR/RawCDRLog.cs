using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Analytics.Entities
{
    public class RawCDRLog
    {
        public long ID { get; set; }
        public int SwitchID { get; set; }
      //  public Int64 IDonSwitch { get; set; }
      //  public string Tag { get; set; }
        public DateTime Attempt { get; set; }
        public DateTime Alert { get; set; }
        public DateTime Connect { get; set; }
        public DateTime Disconnect { get; set; }
        public Decimal DurationInSeconds { get; set; }
        public string InTrunk { get; set; }
    //    public Int64 IN_CIRCUIT { get; set; }
        public string InCarrier { get; set; }
        public string PortIn { get; set; }
        public string OutTrunk { get; set; }
      //  public Int16 OUT_CIRCUIT { get; set; }
        public string OutCarrier { get; set; }
        public string PortOut { get; set; }
        public string CGPN { get; set; }
        public string CDPN { get; set; }
      //  public string CDPNOut { get; set; }
      //  public string CAUSE_FROM_RELEASE_CODE { get; set; }
      //  public string CAUSE_FROM { get; set; }
      //  public string CAUSE_TO_RELEASE_CODE { get; set; }

      //  public string CAUSE_TO { get; set; }
      //  public string Extra_Fields { get; set; }

      //  public string IsRerouted { get; set; }

    }
}
