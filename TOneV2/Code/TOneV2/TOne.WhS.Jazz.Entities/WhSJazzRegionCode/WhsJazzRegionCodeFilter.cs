using System;
using System.Collections.Generic;
using System.Text;


namespace TOne.WhS.Jazz.Entities
{
    public class WhSJazzRegionCodeInfoFilter
    {
        public IEnumerable<IWhSJazzRegionCodeFilter> Filters { get; set; }

    }

    public interface IWhSJazzRegionCodeFilter
    {
        bool IsMatch(IWhSJazzRegionCodeFilterContext context);
    }

    public interface IWhSJazzRegionCodeFilterContext
    {
        WhSJazzRegionCode RegionCode { get; }
    }

    public class WhSJazzRegionCodeFilterContext : IWhSJazzRegionCodeFilterContext
    {
        public WhSJazzRegionCode RegionCode { get; set; }
    }
}