using System;
using System.Collections.Generic;
using System.Text;


namespace TOne.WhS.Jazz.Entities
{
    public class WhSJazzMarketCodeInfoFilter
    {
        public IEnumerable<IWhSJazzMarketCodeFilter> Filters { get; set; }

    }

    public interface IWhSJazzMarketCodeFilter
    {
        bool IsMatch(IWhSJazzMarketCodeFilterContext context);
    }

    public interface IWhSJazzMarketCodeFilterContext
    {
        WhSJazzMarketCode MarketCode { get; }
    }

    public class WhSJazzMarketCodeFilterContext : IWhSJazzMarketCodeFilterContext
    {
        public WhSJazzMarketCode MarketCode { get; set; }
    }
}