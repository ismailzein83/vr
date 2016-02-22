using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Entities
{
    public class RawCDRLog
    {
        public DateTime Attempt { get; set; }
        public DateTime Alert { get; set; }
        public DateTime Connect { get; set; }
        public DateTime Disconnect { get; set; }
        public Decimal DurationInSeconds { get; set; }
        public string InTrunk { get; set; }
        public string PortIn { get; set; }
        public string OutTrunk { get; set; }
        public string PortOut { get; set; }
        public string CGPN { get; set; }
        public string CDPN { get; set; }
        public CDRDirection DirectionType { get; set; } 
        public int DataSourceId { get; set; }
        public int ServiceTypeId { get; set; }
        public CDRType CDRType { get; set; }

    }
}
