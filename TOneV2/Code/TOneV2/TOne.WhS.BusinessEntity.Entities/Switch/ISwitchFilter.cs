using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public interface ISwitchFilter
    {
        bool IsMatched(ISwitchFilterContext context);
    }

    public interface ISwitchFilterContext
    {
        Switch Switch { get; }
    }

    public class SwitchFilterContext : ISwitchFilterContext
    {
        public Switch Switch { get; set; }
    }
}