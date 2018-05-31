using System;

namespace TOne.WhS.RouteSync.Ericsson.Entities
{
    public class RouteCase
    {
        public int RCNumber { get; set; }

        public string RouteCaseOptionsAsString { get; set; }
	}

    public class RouteCaseOption
    {
        public int? Percentage { get; set; }
        public int Priority { get; set; }
        public string OutTrunk { get; set; }
        public TrunkType Type { get; set; }
        public int BNT { get; set; }
        public short SP { get; set; }
        public int? TrunkPercentage { get; set; }
        public bool IsBackup { get; set; }
        public int GroupID { get; set; }
    }
}