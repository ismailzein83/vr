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

        bool InsertRouteRule(RouteRule rule, out int insertedId);

        bool UpdateRouteRule(RouteRule rule);

        bool DeleteRouteRule(int ruleId);

        List<RouteRule> GetAllRouteRule();

        RouteRule GetRouteRuleDetails(int RouteRuleId);

        #region Tone Old Route Rules Integration
        List<RouteRule> GetDifferentialRouteRules(DateTime lastRun);

        List<RouteRule> GetRouteRules(DateTime effectiveDate, bool isFuture, string codePrefix, IEnumerable<Int32> lstCustomerZoneIds, IEnumerable<Int32> lstSupplierZoneIds);

        CodeCustomers GetRulesRouteCodes(DateTime lastRun);

        #endregion
    }
}
