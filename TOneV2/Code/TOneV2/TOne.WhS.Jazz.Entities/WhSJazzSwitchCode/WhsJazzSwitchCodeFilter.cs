using System;
using System.Collections.Generic;
using System.Text;


namespace TOne.WhS.Jazz.Entities
{
    public class WhSJazzSwitchCodeInfoFilter
    {
        public IEnumerable<IWhSJazzSwitchCodeFilter> Filters { get; set; }

    }

    public interface IWhSJazzSwitchCodeFilter
    {
        bool IsMatch(IWhSJazzSwitchCodeFilterContext context);
    }

    public interface IWhSJazzSwitchCodeFilterContext
    {
        WhSJazzSwitchCode SwitchCode { get; }
    }

    public class WhSJazzSwitchCodeFilterContext : IWhSJazzSwitchCodeFilterContext
    {
        public WhSJazzSwitchCode SwitchCode { get; set; }
    }
}