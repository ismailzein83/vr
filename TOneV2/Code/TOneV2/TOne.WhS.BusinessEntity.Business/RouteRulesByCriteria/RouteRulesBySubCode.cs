using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class RouteRulesBySubCode<T> : RouteRulesByOneId<T, string> where T : IRouteCriteria
    {
        protected override bool IsRuleMatched(T rule, out IEnumerable<string> ids)
        {
            if (rule.RouteCriteria.HasCodeFilter() && !rule.RouteCriteria.HasCustomerFilter())
            {
                ids = rule.RouteCriteria.Codes.Where(code => code.WithSubCodes).Select(code => code.Code);
                return ids.Count() > 0;
            }
            else
            {
                ids = null;
                return false;
            }
        }

        protected override bool IsIdAvailable(int? customerId, int? productId, string code, long saleZoneId, out string id)
        {
            id = code;
            return (id != null);
        }

        public override T GetMostMatchedRule(int? customerId, int? productId, string code, long saleZoneId)
        {
            if (code != null)
            {
                StringBuilder codeIterator = new StringBuilder(code);
                while (codeIterator.Length > 1)
                {
                    string parentCode = codeIterator.ToString();
                    T rule = base.GetMostMatchedRule(customerId, productId, parentCode, saleZoneId);
                    if (rule != null)
                        return rule;
                    codeIterator.Remove(codeIterator.Length - 1, 1);
                }
            }
            return default(T);
        }
    }
}
