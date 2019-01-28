using System;
using System.Collections.Generic;
using System.Text;


namespace TOne.WhS.Jazz.Entities
{
    public class RegionInfoFilter
    {
        public IEnumerable<IRegionFilter> Filters { get; set; }

    }

    public interface IRegionFilter
    {
        bool IsMatch(IRegionFilterContext context);
    }

    public interface IRegionFilterContext
    {
        Region Region { get; }
    }

    public class RegionFilterContext : IRegionFilterContext
    {
        public Region Region { get; set; }
    }
}