using System;
using System.Collections.Generic;
using System.Text;


namespace TOne.WhS.Jazz.Entities
{
    public class WhSJazzProductServiceCodeInfoFilter
    {
        public IEnumerable<IWhSJazzProductServiceCodeFilter> Filters { get; set; }

    }
    
    public interface IWhSJazzProductServiceCodeFilter
    {
        bool IsMatch(IWhSJazzProductServiceCodeFilterContext context);
    }

    public interface IWhSJazzProductServiceCodeFilterContext
    {
        WhSJazzProductServiceCode ProductServiceCode { get; }
    }

    public class WhSJazzProductServiceCodeFilterContext : IWhSJazzProductServiceCodeFilterContext
    {
        public WhSJazzProductServiceCode ProductServiceCode { get; set; }
    }
}