using System;
using System.Collections.Generic;
using System.Text;


namespace TOne.WhS.Jazz.Entities
{
    public class WhSJazzAccountCodeInfoFilter
    {
        public IEnumerable<IWhSJazzAccountCodeFilter> Filters { get; set; }

    }
    
    public interface IWhSJazzAccountCodeFilter
    {
        bool IsMatch(IWhSJazzAccountCodeFilterContext context);
    }

    public interface IWhSJazzAccountCodeFilterContext
    {
        WhSJazzAccountCode AccountCode { get; }
    }

    public class WhSJazzAccountCodeFilterContext : IWhSJazzAccountCodeFilterContext
    {
        public WhSJazzAccountCode AccountCode { get; set; }
    }
}