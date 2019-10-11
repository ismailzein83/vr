using System.Collections.Generic;
using Vanrise.Common;

namespace TOne.WhS.RouteSync.Huawei.SoftX3000.Entities
{
    public class RouteCaseOption: IPercentageItem
    {
        public int SRT { get; set; }
        public int? Percentage { get; set; }

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

    public class RouteOptions
    {
        public long RouteId { get; set; }
        public string RouteOptionsAsString { get; set; }
    }
}