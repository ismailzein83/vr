using System;
using System.Collections.Generic;
using System.Text;


namespace TOne.WhS.Jazz.Entities
{
    public class SwitchCodeInfoFilter
    {
        public IEnumerable<ISwitchCodeFilter> Filters { get; set; }

    }

    public interface ISwitchCodeFilter
    {
        bool IsMatch(ISwitchCodeFilterContext context);
    }

    public interface ISwitchCodeFilterContext
    {
        SwitchCode SwitchCode { get; }
    }

    public class SwitchCodeFilterContext : ISwitchCodeFilterContext
    {
        public SwitchCode SwitchCode { get; set; }
    }
}