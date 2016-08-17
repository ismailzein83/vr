using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Entities;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.MainExtensions.GenericRuleCriteriaFieldValues;
using Vanrise.GenericData.Transformation.Entities;

namespace TOne.WhS.DBSync.Business.SwitchMigration
{
    public class SwitchMappingRulesMigrator
    {
        private string ConnectionString { get; set; }
        private List<SwitchMappingRules> SwitchMappingRules { get; set; }
        private Dictionary<string, CarrierAccount> CarrierAccounts { get; set; }

        public SwitchMappingRulesMigrator(string connectionString)
        {
            ConnectionString = connectionString;
        }
        public List<SwitchMappingRules> LoadSwitches()
        {
            SwitchMappingRulesManager manager = new SwitchMappingRulesManager(ConnectionString);
            SwitchMappingRules = manager.LoadSwitches();
            return SwitchMappingRules;
        }
        public void Migrate(string switchId, string parser, DateTime date)
        {
            List<InParsedMapping> inParsedMappings = new List<InParsedMapping>();
            List<OutParsedMapping> outParsedMappings = new List<OutParsedMapping>();
            var switchDictionnary = SwitchMappingRules.ToDictionary(it => it.Id.ToString(), it => it);
            SwitchContext context = new SwitchContext
            {
                ConnectionString = ConnectionString,
                SwitchId = switchId,
                Parser = parser,
                BED = date
            };
            SwitchMappingRules currentSwitch = switchDictionnary.ContainsKey(context.SwitchId)
                ? switchDictionnary[context.SwitchId]
                : null;

            if (currentSwitch == null) return;

            switch (context.Parser)
            {
                case "Teles":
                    TelesSwitchParser telesSwitchParser = new TelesSwitchParser(currentSwitch.Configuration);
                    telesSwitchParser.GetParsedMappings(out inParsedMappings, out outParsedMappings);
                    break;
            }
            MigrateToToneV2(inParsedMappings, outParsedMappings, context.BED, context.SwitchId);
        }

        #region private functions
        private Dictionary<string, Switch> GetSwitches()
        {
            SwitchManager manager = new SwitchManager();
            return manager.GetAllSwitches().Where(it => it.SourceId != null).ToDictionary(it => it.SourceId, it => it);
        }
        private Dictionary<string, CarrierAccount> GetAllCarriers()
        {
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            return carrierAccountManager.GetAllCarriers().Where(it => it.SourceId != null).ToDictionary(it => it.SourceId, it => it);
        }
        private object AddGenericRule(GenericRule rule)
        {
            GenericRuleDefinitionManager ruleDefinitionManager = new GenericRuleDefinitionManager();
            GenericRuleDefinition ruleDefinition = ruleDefinitionManager.GetGenericRuleDefinition(176);

            GenericRuleTypeConfigManager ruleTypeManager = new GenericRuleTypeConfigManager();
            GenericRuleTypeConfig ruleTypeConfig = ruleTypeManager.GetGenericRuleTypeById(ruleDefinition.SettingsDefinition.ConfigId);

            Type managerType = Type.GetType(ruleTypeConfig.RuleManagerFQTN);
            var manager = Activator.CreateInstance(managerType) as IGenericRuleManager;

            return manager.AddGenericRule(rule);
        }
        private void MigrateToToneV2(List<InParsedMapping> inParsedMappings, List<OutParsedMapping> outParsedMappings, DateTime date, string switchId)
        {
            Dictionary<string, CarrierAccount> carrierAccounts = GetAllCarriers();
            Dictionary<string, Switch> switches = GetSwitches();
            int currentSwitchId = switches.ContainsKey(switchId) ? switches[switchId].SwitchId : 0;
            #region customer
            foreach (var elt in inParsedMappings)
            {
                if (elt.InTrunk.Values.Count() == 0)
                    continue;
                MappingRule rule = GetRule(elt.CustomerId, elt.InTrunk, currentSwitchId, date, 1);
                AddGenericRule(rule);
            }
            #endregion

            #region supplier
            foreach (var elt in outParsedMappings)
            {
                if (elt.OutTrunk.Values.Count() == 0)
                    continue;
                MappingRule rule = GetRule(elt.SupplierId, elt.OutTrunk, currentSwitchId, date, 2);
                AddGenericRule(rule);
            }
            #endregion

        }
        private MappingRule GetRule(string carrierId, StaticValues trunk, int currentSwitchId, DateTime date, int carrierType)
        {
            MappingRule rule = new MappingRule
            {
                Settings = new MappingRuleSettings
                {
                    Value = CarrierAccounts.ContainsKey(carrierId) ? CarrierAccounts[carrierId].CarrierAccountId : 0
                },
                DefinitionId = 176,
                Criteria = new GenericRuleCriteria
                {
                    FieldsValues = new Dictionary<string, GenericRuleCriteriaFieldValues>()
                },
                RuleId = 0,
                Description = "Switch Migration - Teles Swicth",
                BeginEffectiveTime = date

            };
            rule.Criteria.FieldsValues["Type"] = new StaticValues
            {
                Values = ((new List<long> { carrierType }).Cast<Object>()).ToList()
            };
            rule.Criteria.FieldsValues["Switch"] = new StaticValues
            {
                Values = ((new List<int> { currentSwitchId }).Cast<Object>()).ToList()
            };
            rule.Criteria.FieldsValues["Trunk"] = trunk;
            return rule;
        }
        #endregion
    }


}
