using System;
using System.Collections.Generic;
using System.Text;


namespace TOne.WhS.Jazz.Entities
{
    public class MarketInfoFilter
    {
        public IEnumerable<IMarketFilter> Filters { get; set; }

    }

    public interface IMarketFilter
    {
        bool IsMatch(IMarketFilterContext context);
    }

    public interface IMarketFilterContext
    {
        Market Market { get; }
    }

    public class MarketFilterContext : IMarketFilterContext
    {
        public Market Market { get; set; }
    }
}