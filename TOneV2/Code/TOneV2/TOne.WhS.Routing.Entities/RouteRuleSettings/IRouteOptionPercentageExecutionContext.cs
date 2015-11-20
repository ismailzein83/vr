using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Routing.Entities
{
    public interface IRouteOptionPercentageTarget
    {
        Decimal? Percentage { get; set; }
    }

    public interface IRouteOptionPercentageExecutionContext
    {
        IEnumerable<IRouteOptionPercentageTarget> Options { get; }
    }
}
