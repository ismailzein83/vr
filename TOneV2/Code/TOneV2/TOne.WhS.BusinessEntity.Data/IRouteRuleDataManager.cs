using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Data
{
    public interface IRouteRuleDataManager : IDataManager
    {
        Vanrise.Entities.BigResult<Entities.RouteRule> GetFilteredRouteRules(Vanrise.Entities.DataRetrievalInput<Entities.RouteRuleQuery> input);

        bool Delete(int routeRuleId);
    }
}
