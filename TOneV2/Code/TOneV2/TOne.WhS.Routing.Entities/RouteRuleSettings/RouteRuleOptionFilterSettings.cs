using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Routing.Entities
{
    public abstract class RouteOptionFilterSettings
    {
        public abstract Guid ConfigId { get; }

        public abstract void Execute(IRouteOptionFilterExecutionContext context);
    }    
}
