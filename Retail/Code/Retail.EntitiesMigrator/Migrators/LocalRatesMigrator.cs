using Retail.EntitiesMigrator.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Retail.EntitiesMigrator.Migrators
{
    public class LocalRatesMigrator
    {
        public void Execute()
        {
            List<GenericRule> tariffRules = new List<GenericRule>();
            List<GenericRule> rateRules = new List<GenericRule>();
            var defaultRateRule = Helper.CreateRateValueRule(Helper.LocalRuleDefinition, GetDefaultCriteriaFieldValues(), new RateDetails { Rate = 0 });
            var defaultTariffRule = Helper.CreateTariffRule(Helper.LocalRuleDefinition, GetDefaultCriteriaFieldValues(), new RateDetails { FractionUnit = 60 });
            tariffRules.Add(defaultTariffRule);
            rateRules.Add(defaultRateRule);
            Helper.SaveTariffRules(tariffRules);
            Helper.SaveRateValueRules(rateRules);
        }

        private Dictionary<string, GenericRuleCriteriaFieldValues> GetDefaultCriteriaFieldValues()
        {
            Dictionary<string, GenericRuleCriteriaFieldValues> result = Helper.BuildCriteriaFieldsValues(Helper.LocalRuleDefinition.ServiceTypeId, Helper.LocalRuleDefinition.ChargingPolicyId, null);
            return result;
        }
    }
}
