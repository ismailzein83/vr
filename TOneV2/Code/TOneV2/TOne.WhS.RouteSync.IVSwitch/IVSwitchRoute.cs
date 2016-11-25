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
            Flag1 = 0;
            Flag2 = 0;
            Flag3 = 0;
            Flag4 = 0;
            Flag5 = 0;
            TechPrefix=0;
        }

        public override string ToString()
        {
            return string.Format(
                    @"{1}{23}{2}{23}{3}{23}{4}{23}{5}{23}{6}{23}{7}{23}{8}{23}{9}{23}{10}{23}{11}{23}{12}{23}{13}{23}{14}{23}{15}{23}{16}{23}{17}{23}{18}{23}{19}{23}{20}{23}{21}{23}{22}",
                    string.Empty, Destination, RoutingMode, TimeFrame,
                    Preference, HuntStop, HuntStopRc, MinProfit,
                    StateId, WakeUpTime, Description
                    , RoutingMode, TotalBkts, BktSerial, BktCapacity,
                    BktToken, PScore, Flag1, Flag2, Flag3,
                    Flag4, Flag5, TechPrefix
                    , "\t");
        }
    }
}
