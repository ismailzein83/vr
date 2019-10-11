using System.Collections.Generic;
using Vanrise.Common;

namespace TOne.WhS.RouteSync.Huawei.SoftX3000.Entities
{
    public class RouteAnalysis
    {
        public int RSSC { get; set; }
        public RouteCaseOptionsType RouteOptionsType { get; set; }
        public List<RouteCaseOption> RouteCaseOptions { get; set; }
    }

    public enum RouteCaseOptionsType { Sequence = 0, Percentage = 1 }
}