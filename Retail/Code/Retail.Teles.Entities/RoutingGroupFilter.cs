using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Teles.Entities
{
    public abstract class RoutingGroupFilter
    {
        public abstract Guid ConfigId { get; }

        public abstract bool Evaluate(IRoutingGroupFilterContext context);
    }

    public interface IRoutingGroupFilterContext
    {
        string RoutingGroupName { get; }
    }
}
