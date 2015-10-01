using PSTN.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace PSTN.BusinessEntity.Business.Normalization.RulesByCriteria
{
    public class RulesBySwitchTrunkPrefix : NormalizationRulesByCriteria
    {
        public override bool IsEmpty()
        {
            return _structuredRules.Count == 0;
        }

        Dictionary<string, List<NormalizationRule>> _structuredRules = new Dictionary<string, List<NormalizationRule>>();
        int _minimumPrefixLength = int.MaxValue;
        int _maximumPrefixLength;

        public override void SetSource(List<NormalizationRule> rules)
        {
            foreach(var rule in rules)
            {
                var criteria = rule.Criteria;
                if(criteria.SwitchIds != null && criteria.SwitchIds.Count > 0 && criteria.TrunkIds != null && criteria.TrunkIds.Count > 0 && criteria.PhoneNumberPrefix != null)
                {
                    foreach(var switchId in criteria.SwitchIds)
                    {
                        foreach(var trunkId in criteria.TrunkIds)
                        {
                            int prefixLength = criteria.PhoneNumberPrefix.Length;
                            if (prefixLength < _minimumPrefixLength)
                                _minimumPrefixLength = prefixLength;
                            if (prefixLength > _maximumPrefixLength)
                                _maximumPrefixLength = prefixLength;
                            string key = GetKey(switchId, trunkId, criteria.PhoneNumberPrefix);
                            List<NormalizationRule> matchRules = _structuredRules.GetOrCreateItem(key);
                            matchRules.Add(rule);
                        }
                    }
                }
            }
        }

        private string GetKey(int switchId, int trunkId, string phoneNumberPrefix)
        {
            return String.Format("{0}_{1}_{2}", switchId, trunkId, phoneNumberPrefix);
        }

        public override NormalizationRule GetMostMatchedRule(int switchId, int trunkId, string phoneNumber)
        {
            string phoneNumberPrefix = phoneNumber.Substring(0, Math.Min(_maximumPrefixLength, phoneNumber.Length));
            return GetMostMatchedRulePrivate(switchId, trunkId, phoneNumberPrefix);
        }

        private NormalizationRule GetMostMatchedRulePrivate(int switchId, int trunkId, string phoneNumberPrefix)
        {
            string key = GetKey(switchId, trunkId, phoneNumberPrefix);
            List<NormalizationRule> matchRules;
            if(_structuredRules.TryGetValue(key, out matchRules))
            {
                if (matchRules.Count > 0)
                    return matchRules[0];
            }
            if(phoneNumberPrefix.Length > _minimumPrefixLength)
                return GetMostMatchedRulePrivate(switchId, trunkId, phoneNumberPrefix.Substring(0, phoneNumberPrefix.Length - 1));
            return null;
        }
    }
}
