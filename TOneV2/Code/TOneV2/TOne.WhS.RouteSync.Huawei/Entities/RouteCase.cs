using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.RouteSync.Huawei.Entities
{
    public class RouteCase
    {
        public int RCNumber { get; set; }
        public string RSName { get; set; }
        public string RouteCaseAsString { get; set; }
    }

    public class RTANA
    {
        public int RSSN { get; set; }
        public bool IsSequence { get; set; }
        public List<RouteCaseOption> RouteCaseOptions { get; set; }
    }

    public class RouteCaseOption
    {
        public string RouteName { get; set; }
        public string ISUP { get; set; }
        public int? Percentage { get; set; }
    }

    //ToBeDeleted???
    //public enum RouteCaseOptionsType { Sequence = 0, Percentage = 1 }
}