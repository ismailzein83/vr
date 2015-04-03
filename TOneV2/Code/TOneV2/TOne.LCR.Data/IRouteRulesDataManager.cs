using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.LCR.Entities;

namespace TOne.LCR.Data
{
    public interface IRouteRulesDataManager : IDataManager
    {
        List<RouteRule> GetRouteRules(DateTime effectiveDate, bool isFuture, string codePrefix, IEnumerable<Int32> lstCustomerZoneIds, IEnumerable<Int32> lstSupplierZoneIds );

        void SaveRouteRule(RouteRule rule);

        List<RouteRule> GetAllRouteRule();

        RouteRule GetRouteRuleDetails(int RouteRuleId);
    }
}
