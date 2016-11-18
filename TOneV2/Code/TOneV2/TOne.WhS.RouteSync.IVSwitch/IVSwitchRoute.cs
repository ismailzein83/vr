using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.RouteSync.IVSwitch
{
    public class IVSwitchRoute
    {
        public string Destination { get; set; }
        public int? RouteId { get; set; }
        public string TimeFrame { get; set; }
        public int Preference { get; set; }
        public int HuntStop { get; set; }
        public string HuntStopRc { get; set; }
        public decimal MinProfit { get; set; }
        public int StateId { get; set; }
        public DateTime WakeUpTime { get; set; }
        public string Description { get; set; }
        public int RoutingMode { get; set; }
        public int TotalBkts { get; set; }
        public int BktSerial { get; set; }
        public int BktCapacity { get; set; }
        public int BktToken { get; set; }
        public int PScore { get; set; }
        public decimal? Flag1 { get; set; }
        public decimal Flag2 { get; set; }
        public int Flag3 { get; set; }
        public int Flag4 { get; set; }
        public decimal Flag5 { get; set; }
        public int TechPrefix { get; set; }
        public IVSwitchRoute()
        {
            RoutingMode = 8;
            TotalBkts = 1;
            BktSerial = 1;
            BktCapacity = 1;
            BktToken = 1;
            TimeFrame = "* * * * *";
        }
    }
}
