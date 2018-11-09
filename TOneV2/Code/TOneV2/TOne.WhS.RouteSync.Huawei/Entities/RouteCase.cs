using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace TOne.WhS.RouteSync.Huawei.Entities
{
    public class RouteCase
    {
        public int RCNumber { get; set; }
        public string RSName { get; set; }
        public string RouteCaseAsString { get; set; }
    }

    public class RouteAnalysis
    {
        public int RSSN { get; set; }
        public RouteCaseOptionsType RouteCaseOptionsType { get; set; }
        public List<RouteCaseOption> RouteCaseOptions { get; set; }
    }

    public class RouteCaseOption : IPercentageItem
    {
        public string RouteName { get; set; }
        public int? Percentage { get; set; }
        public string ISUP { get; set; }

        public decimal? GetInputPercentage()
        {
            return this.Percentage;
        }

        public void SetOutputPercentage(int? percentage)
        {
            this.Percentage = percentage;
        }

        public bool ShouldHavePercentage()
        {
            return true;
        }
    }

    public enum RouteCaseOptionsType { Sequence = 0, Percentage = 1 }
}