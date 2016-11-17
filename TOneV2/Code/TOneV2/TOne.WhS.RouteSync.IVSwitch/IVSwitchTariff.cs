using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.RouteSync.IVSwitch
{
    public class IVSwitchTariff
    {
        public string DestinationCode { get; set; }
        public string TimeFrame { get; set; }
        public string DestinationName { get; set; }
        public int InitPeiod { get; set; }
        public int NextPeriod { get; set; }
        public decimal? InitCharge { get; set; }
        public decimal NextCharge { get; set; }
    }
}
