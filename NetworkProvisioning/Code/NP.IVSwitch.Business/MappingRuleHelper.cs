using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using Vanrise.Entities;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.MainExtensions.GenericRuleCriteriaFieldValues;
using Vanrise.GenericData.Transformation;
using Vanrise.GenericData.Transformation.Entities;

namespace NP.IVSwitch.Business
{
    public enum MappingRuleCarrierType
    {
        Customer = 1,
        Supplier = 2
    }
    public class MappingRuleHelper
    {
        private int _carrierId;
        private int _criteriaCarrierId;
        private MappingRuleCarrierType _carrierType;
        private string _carrierName;
        private Guid _definitionId;

        public MappingRuleHelper(int carrierId, int criteriaCarrierId, long typeId, string carrierName)
        {
            _carrierId = carrierId;
            _criteriaCarrierId = criteriaCarrierId;
            _carrierType = (MappingRuleCarrierType)typeId;
            _carrierName = carrierName;
            ConfigManager configManager = new ConfigManager();
            _definitionId = _carrierType == MappingRuleCarrierType.Customer
                ? configManager.GetCustomerRuleDefinitionId()
                : configManager.GetSupplierRuleDefinitionId();
        }

        private bool TryGetMatchingMappingRule(out MappingRule matchedRule)
        {
            matchedRule = new MappingRule();
            MappingRuleManager mappingRuleManager = new MappingRuleManager();
            var genericRules = mappingRuleManager.GetGenericRulesByDefinitionId(_definitionId);
            foreach (var rule in genericRules)
            {
                MappingRule mappingRule = (MappingRule)rule;
                int carrierAccountId = Convert.ToInt32(mappingRule.Settings.Value);
                if (carrierAccountId != _carrierId) continue;
                GenericRuleCriteriaFieldValues criteriaFieldValues;
                if (mappingRule.Criteria.FieldsValues.TryGetValue("Type", out criteriaFieldValues))
                {
                    var typeValue = criteriaFieldValues.GetValues();
                    if (typeValue.Contains((long)_carrierType))
                    {
                        matchedRule = mappingRule;
                        return true;
                    }
                }
            }
            return false;
        }

        public bool BuildRule()
        {
            MappingRule toCompareRule;
            if (TryGetMatchingMappingRule(out toCompareRule))
                return UpdateRule(toCompareRule);
            return CreateRule();
        }
        private bool CreateRule()
        {
            var tempSiwtch = Helper.GetSwitch();
            if (tempSiwtch == null) return false;
            MappingRule createdRule = new MappingRule
            {
                Settings = new MappingRuleSettings
                {
                    Value = _carrierId
                },
                DefinitionId = _definitionId,
                Criteria = new GenericRuleCriteria
                {
                    FieldsValues = new Dictionary<string, GenericRuleCriteriaFieldValues>()
                },
                RuleId = 0,
                Description = _carrierName,
                BeginEffectiveTime = new DateTime(2000, 1, 1)
            };
            createdRule.Criteria.FieldsValues["Carrier"] = new StaticValues
            {
                Values = new List<object> { _criteriaCarrierId.ToString() }
            };

            createdRule.Criteria.FieldsValues["Type"] = new StaticValues
            {
                Values = new List<Object> { _carrierType }
            };
            createdRule.Criteria.FieldsValues["Switch"] = new StaticValues
            {
                Values = new List<object> { tempSiwtch.SwitchId }
            };
            return AddRule(createdRule);
        }

        private bool UpdateRule(MappingRule rule)
        {
            List<object> carrierList = new List<object> { _carrierId.ToString() };
            GenericRuleCriteriaFieldValues criteriaFieldValues;
            if (rule.Criteria.FieldsValues.TryGetValue("Carrier", out criteriaFieldValues))
            {
                var value = criteriaFieldValues.GetValues();
                carrierList.AddRange(value);
            }
            rule.Criteria.FieldsValues["Carrier"] = new StaticValues
            {
                Values = carrierList
            };
            return Update(rule);
        }
        private bool Update(MappingRule rule)
        {
            var manager = GetManager();
            return manager.TryUpdateGenericRule(rule);
        }
        private bool AddRule(GenericRule rule)
        {
            var manager = GetManager();
            return manager.TryAddGenericRule(rule);
        }
        private IGenericRuleManager GetManager()
        {
            GenericRuleDefinitionManager ruleDefinitionManager = new GenericRuleDefinitionManager();
            GenericRuleDefinition ruleDefinition = ruleDefinitionManager.GetGenericRuleDefinition(_definitionId);

            GenericRuleTypeConfigManager ruleTypeManager = new GenericRuleTypeConfigManager();
            GenericRuleTypeConfig ruleTypeConfig = ruleTypeManager.GetGenericRuleTypeById(ruleDefinition.SettingsDefinition.ConfigId);

            Type managerType = Type.GetType(ruleTypeConfig.RuleManagerFQTN);
            return Activator.CreateInstance(managerType) as IGenericRuleManager;
        }
    }
}
