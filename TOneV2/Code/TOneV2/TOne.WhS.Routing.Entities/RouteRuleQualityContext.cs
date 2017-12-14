using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Routing.Entities
{
    public class RouteRuleQualityContext : IRouteRuleQualityContext
    {
       public List<Guid> QualityConfigurationIds { get; set; }
       public bool IsDefault { get; set; }
    }
}
