using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.RouteSync.IVSwitch
{
    public class IVSwitchOption
    {
        public string Option { get; set; }
        public int? Priority { get; set; }
        public Decimal? Percentage { get; set; }
        public decimal ScaledDownPercentage { get; set; }
        public decimal Rate { get; set; }
        public int Serial { get; set; }
    }
}
