using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class RouteRulesBySubCode : RouteRulesByOneId<string>
    {
        protected override bool IsRuleMatched(IRouteCriteria rule, out IEnumerable<string> ids)
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

        public override IRouteCriteria GetMostMatchedRule(int? customerId, int? productId, string code, long saleZoneId)
        {
            if (code != null)
            {
                StringBuilder codeIterator = new StringBuilder(code);
                while (codeIterator.Length > 1)
                {
                    string parentCode = codeIterator.ToString();
                    IRouteCriteria rule = base.GetMostMatchedRule(customerId, productId, parentCode, saleZoneId);
                    if (rule != null)
                        return rule;
                    codeIterator.Remove(codeIterator.Length - 1, 1);
                }
            }
            return null;
        }
    }
}
