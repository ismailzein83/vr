using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Routing.Entities
{
    public interface IRouteRuleQualityContext
    {
        List<Guid> QualityConfigurationIds { set; }
        bool IsDefault { set; }
    }
}
