using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data
{
    public interface IRouteRuleDataManager : IDataManager
    {
        Vanrise.Entities.BigResult<Entities.RouteRule> GetFilteredRouteRules(Vanrise.Entities.DataRetrievalInput<Entities.RouteRuleQuery> input);

        RouteRule GetRouteRule(int routeRuleId);

        bool Insert(RouteRule routeRule, out int insertedId);

        bool Update(RouteRule routeRule);

        bool Delete(int routeRuleId);
    }
}
