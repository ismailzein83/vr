using System.Collections.Generic;
using Vanrise.Common;

namespace TOne.WhS.RouteSync.Huawei.SoftX3000.Entities
{
    public class RouteCaseToAdd
    {
        public string RAN { get; set; }

        public int RSSC { get; set; }
    }

    public class RouteCase : RouteCaseToAdd
    {
        public long RouteCaseId { get; set; }
        public long RouteId { get; set; }
    }
}