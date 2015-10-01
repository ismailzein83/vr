using PSTN.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace PSTN.BusinessEntity.Business.Normalization.RulesByCriteria
{
    public class RulesBySwitchTrunkLength : NormalizationRulesByCriteria
    {
        public override bool IsEmpty()
        {
            return _structuredRules.Count == 0;
        }

        Dictionary<string, List<NormalizationRule>> _structuredRules = new Dictionary<string, List<NormalizationRule>>();

        public override void SetSource(List<NormalizationRule> rules)
        {
            foreach(var rule in rules)
            {
                var criteria = rule.Criteria;
                if(criteria.SwitchIds != null && criteria.SwitchIds.Count > 0 && criteria.TrunkIds != null && criteria.TrunkIds.Count > 0 && criteria.PhoneNumberLength.HasValue)
                {
                    foreach(var switchId in criteria.SwitchIds)
                    {
                        foreach(var trunkId in criteria.TrunkIds)
                        {                           
                            string key = GetKey(switchId, trunkId, criteria.PhoneNumberLength.Value);
                            List<NormalizationRule> matchRules = _structuredRules.GetOrCreateItem(key);
                            matchRules.Add(rule);
                        }
                    }
                }
            }
        }

        private string GetKey(int switchId, int trunkId, int phoneNumberLength)
        {
            return String.Format("{0}_{1}_{2}", switchId, trunkId, phoneNumberLength);
        }

        public override NormalizationRule GetMostMatchedRule(int switchId, int trunkId, string phoneNumber)
        {
            int phoneNumberLength = phoneNumber.Length;
            string key = GetKey(switchId, trunkId, phoneNumberLength);
            List<NormalizationRule> matchRules;
            if (_structuredRules.TryGetValue(key, out matchRules))
            {
                if (matchRules.Count > 0)
                    return matchRules[0];
            }
            return null;
        }
    }
}
