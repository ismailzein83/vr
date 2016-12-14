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
    public class MappingRuleHelper
    {
        private int CarrierId { get; set; }
        private int CriteriaCarrierId { get; set; }
        private long TypeId { get; set; }
        private string CarrierName { get; set; }
        private Guid DefinitionId { get; set; }

        public MappingRuleHelper(int carrierId, int criteriaCarrierId, long typeId, string carrierName)
        {
            CarrierId = carrierId;
            CriteriaCarrierId = criteriaCarrierId;
            TypeId = typeId;
            CarrierName = carrierName;
            ConfigManager configManager = new ConfigManager();
            DefinitionId = configManager.GetCustomerRuleDefinitionId();
        }
        public bool BuildRule()
        {
            return GenerateRule();
        }
        private bool AreSettingMatched(MappingRule originalRule, int carrierId)
        {
            long originalCarrierId = (long)originalRule.Settings.Value;
            if (originalCarrierId != carrierId) return false;
            foreach (var elt in originalRule.Criteria.FieldsValues)
            {
                if (!elt.Key.Equals("Type")) continue;
                var value = elt.Value.GetValues();
                return value.Contains(TypeId);
            }
            return false;
        }
        private bool GenerateRule()
        {
            MappingRule tempRule = new MappingRule
            {
                Settings = new MappingRuleSettings
                {
                    Value = CarrierId
                },
                DefinitionId = DefinitionId,
                Criteria = new GenericRuleCriteria
                {
                    FieldsValues = new Dictionary<string, GenericRuleCriteriaFieldValues>()
                },
                RuleId = 0,
                Description = CarrierName,
                BeginEffectiveTime = new DateTime(2000, 1, 1)
            };
            MappingRuleManager mappingRuleManager = new MappingRuleManager();
            var mappingRules = mappingRuleManager.GetGenericRulesByDefinitionId(DefinitionId);
            foreach (var rule in mappingRules)
            {
                MappingRule mappingRule = (MappingRule)rule;
                if (AreSettingMatched(mappingRule, CarrierId))
                    return UpdateRule(mappingRule, CriteriaCarrierId.ToString());
            }
            return CreateRule(tempRule, CriteriaCarrierId);

        }
        private bool CreateRule(MappingRule originRule, int endpointId)
        {
            MappingRule createdRule = originRule;
            var tempSiwtch = Helper.GetSwitch();
            if (tempSiwtch == null) return false;
            createdRule.Criteria.FieldsValues["Carrier"] = new StaticValues
            {
                Values = ((new List<string> { endpointId.ToString() }).Cast<Object>()).ToList()
            };

            createdRule.Criteria.FieldsValues["Type"] = new StaticValues
            {
                Values = ((new List<long> { TypeId }).Cast<Object>()).ToList()
            };
            createdRule.Criteria.FieldsValues["Switch"] = new StaticValues
            {
                Values = ((new List<int> { tempSiwtch.SwitchId }).Cast<Object>()).ToList()
            };
            var output = AddRule(createdRule);
            switch (output.Result)
            {
                case InsertOperationResult.Succeeded:
                    return true;
                case InsertOperationResult.Failed:
                    return false;
                case InsertOperationResult.SameExists:
                    return false;
            }
            return false;
        }

        private bool UpdateRule(MappingRule rule, string endpointid)
        {
            List<string> carrierList = new List<string> { endpointid };
            foreach (var elt in rule.Criteria.FieldsValues)
            {
                if (!elt.Key.Equals("Carrier")) continue;

                var value = elt.Value.GetValues();
                carrierList.AddRange(value.Cast<string>());
            }
            rule.Criteria.FieldsValues["Carrier"] = new StaticValues
            {
                Values = (carrierList.Cast<Object>()).ToList()
            };
            var output = Update(rule);
            switch (output.Result)
            {
                case UpdateOperationResult.Succeeded:
                    return true;
                case UpdateOperationResult.Failed:
                    return false;
                case UpdateOperationResult.SameExists:
                    return false;
            }
            return false;
        }
        private UpdateOperationOutput<GenericRuleDetail> Update(MappingRule rule)
        {
            var manager = GetManager(DefinitionId);
            return manager.UpdateGenericRule(rule);
        }
        private InsertOperationOutput<GenericRuleDetail> AddRule(GenericRule rule)
        {
            var manager = GetManager(DefinitionId);
            return manager.AddGenericRule(rule);
        }
        private IGenericRuleManager GetManager(Guid ruleDefinitionGuid)
        {
            GenericRuleDefinitionManager ruleDefinitionManager = new GenericRuleDefinitionManager();
            GenericRuleDefinition ruleDefinition = ruleDefinitionManager.GetGenericRuleDefinition(ruleDefinitionGuid);

            GenericRuleTypeConfigManager ruleTypeManager = new GenericRuleTypeConfigManager();
            GenericRuleTypeConfig ruleTypeConfig = ruleTypeManager.GetGenericRuleTypeById(ruleDefinition.SettingsDefinition.ConfigId);

            Type managerType = Type.GetType(ruleTypeConfig.RuleManagerFQTN);
            return Activator.CreateInstance(managerType) as IGenericRuleManager;
        }
    }
}
