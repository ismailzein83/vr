using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class RouteRulesByCustomerSubCode : RouteRulesByTwoIds<int, string>
    {
        protected override bool IsRuleMatched(RouteRule rule, out IEnumerable<int> ids1, out IEnumerable<string> ids2)
        {
            if (rule.Criteria.HasCustomerFilter() && rule.Criteria.HasCodeFilter())
            {
                CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
                ids1 = carrierAccountManager.GetCustomerIds(rule.Criteria.CustomersGroupConfigId.Value, rule.Criteria.CustomerGroupSettings);
                ids2 = rule.Criteria.Codes.Where(code => code.WithSubCodes).Select(code => code.Code);
                return ids2.Count() > 0;
            }
            else
            {
                ids1 = null;
                ids2 = null;
                return false;
            }
        }

        protected override bool AreIdsAvailable(int? customerId, int? productId, string code, long saleZoneId, out int id1, out string id2)
        {
            if (customerId != null && code != null)
            {
                id1 = customerId.Value;
                id2 = code;
                return true;
            }
            else
            {
                id1 = 0;
                id2 = null;
                return false;
            }
        }

        public override RouteRule GetMostMatchedRule(int? customerId, int? productId, string code, long saleZoneId)
        {
            if (code != null)
            {
                StringBuilder codeIterator = new StringBuilder(code);
                while (codeIterator.Length > 1)
                {
                    string parentCode = codeIterator.ToString();
                    RouteRule rule = base.GetMostMatchedRule(customerId, productId, parentCode, saleZoneId);
                    if (rule != null)
                        return rule;
                    codeIterator.Remove(codeIterator.Length - 1, 1);
                }
            }
            return null;
        }
    }
}
