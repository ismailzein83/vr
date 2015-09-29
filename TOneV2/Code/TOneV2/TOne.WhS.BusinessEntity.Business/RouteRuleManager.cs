using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class RouteRuleManager
    {
        public StructuredRouteRules<T> StructureRules<T>(List<T> rules) where T : IRouteCriteria
        {
            List<RouteRulesByCriteria<T>> routeRulesByCriteria = new List<RouteRulesByCriteria<T>>();
            routeRulesByCriteria.Add(new RouteRulesByCustomerCode<T>());
            routeRulesByCriteria.Add(new RouteRulesByCustomerSubCode<T>());
            routeRulesByCriteria.Add(new RouteRulesByCode<T>());
            routeRulesByCriteria.Add(new RouteRulesBySubCode<T>());
            routeRulesByCriteria.Add(new RouteRulesByCustomerZone<T>());
            routeRulesByCriteria.Add(new RouteRulesByProductZone<T>());
            routeRulesByCriteria.Add(new RouteRulesByZone<T>());
            routeRulesByCriteria.Add(new RouteRulesByCustomer<T>());
            routeRulesByCriteria.Add(new RouteRulesByProduct<T>());
            routeRulesByCriteria.Add(new RouteRulesByOthers<T>());
            StructuredRouteRules<T> structuredRouteRules = new StructuredRouteRules<T>();
            RouteRulesByCriteria<T> current = null;
            foreach (var r in routeRulesByCriteria)
            {
                r.SetSource(rules);
                if (!r.IsEmpty())
                {
                    if (current != null)
                        current.NextRuleSet = r;
                    else
                        structuredRouteRules.FirstRuleSet = r;
                    current = r;
                }                    
            }
            return structuredRouteRules;
        }

        public T GetMostMatchedRule<T>(StructuredRouteRules<T> rules, int? customerId, int? productId, string code, long saleZoneId) where T : IRouteCriteria
        {
            return GetMostMatchedRule(rules.FirstRuleSet, customerId, productId, code, saleZoneId);
        }

        T GetMostMatchedRule<T>(RouteRulesByCriteria<T> routeRulesByCriteria, int? customerId, int? productId, string code, long saleZoneId) where T : IRouteCriteria
        {
            if (routeRulesByCriteria == null)
                return default(T);
            T rule = routeRulesByCriteria.GetMostMatchedRule(customerId, productId, code, saleZoneId);
            if (rule != null)
                return rule;
            else
                return GetMostMatchedRule(routeRulesByCriteria.NextRuleSet, customerId, productId, code, saleZoneId);
        }

        public static bool IsAnyFilterExcludedInRuleCriteria(RouteCriteria ruleCriteria, int? customerId, string code, long zoneId)
        {
            return IsItemInList(code, ruleCriteria.ExcludedCodes);
        }

        static bool IsItemInList<T>(T item, List<T> list)
        {
            return item != null && list != null && list.Contains(item);
        }

        public Vanrise.Entities.IDataRetrievalResult<RouteRule> GetFilteredRouteRules(Vanrise.Entities.DataRetrievalInput<RouteRuleQuery> input)
        {
            IRouteRuleDataManager dataManager = BEDataManagerFactory.GetDataManager<IRouteRuleDataManager>();
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, dataManager.GetFilteredRouteRules(input));
        }

        public TOne.Entities.DeleteOperationOutput<object> DeleteRouteRule(int routeRuleId)
        {
            IRouteRuleDataManager dataManager = BEDataManagerFactory.GetDataManager<IRouteRuleDataManager>();

            TOne.Entities.DeleteOperationOutput<object> deleteOperationOutput = new TOne.Entities.DeleteOperationOutput<object>();
            deleteOperationOutput.Result = Vanrise.Entities.DeleteOperationResult.Failed;

            bool deleteActionSucc = dataManager.Delete(routeRuleId);

            if (deleteActionSucc)
            {
                deleteOperationOutput.Result = Vanrise.Entities.DeleteOperationResult.Succeeded;
            }

            return deleteOperationOutput;
        }
    }
}
