using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.MainExtensions.GenericRuleCriteriaFieldValues;
using Vanrise.GenericData.Transformation.Entities;

namespace TOne.WhS.DBSync.Business
{
    public class MappingRuleGenerator
    {
        public static MappingRule GetRule(int ownerId, StaticValues trunk, int currentSwitchId, DateTime? date, int carrierType)
        {
            MappingRule rule = new MappingRule
            {
                Settings = new MappingRuleSettings
                {
                    Value = ownerId
                },
                DefinitionId = new Guid("E1ADF1F2-6BC3-4541-8DE4-E5F578A79372"),
                Criteria = new GenericRuleCriteria
                {
                    FieldsValues = new Dictionary<string, GenericRuleCriteriaFieldValues>()
                },
                RuleId = 0,
                Description = "Switch Migration",
                BeginEffectiveTime = date ?? DateTime.MinValue
            };

            rule.Criteria.FieldsValues["Type"] = new StaticValues
            {
                Values = ((new List<long> { carrierType }).Cast<Object>()).ToList()
            };
            rule.Criteria.FieldsValues["Switch"] = new StaticValues
            {
                Values = ((new List<int> { currentSwitchId }).Cast<Object>()).ToList()
            };

            if (trunk != null)
                rule.Criteria.FieldsValues["Carrier"] = trunk;

            return rule;
        }
    }
}
