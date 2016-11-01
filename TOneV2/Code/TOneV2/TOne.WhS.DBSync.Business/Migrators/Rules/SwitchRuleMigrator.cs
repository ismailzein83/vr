using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Data.SQL;
using TOne.WhS.DBSync.Entities;
using Vanrise.Common;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.MainExtensions.GenericRuleCriteriaFieldValues;
using Vanrise.GenericData.Normalization;
using Vanrise.GenericData.Transformation;
using Vanrise.GenericData.Transformation.Entities;
using Vanrise.Rules.Entities;

namespace TOne.WhS.DBSync.Business.Migrators
{
    public class SwitchRuleMigrator : RuleBaseMigrator
    {
        public override string EntityName
        {
            get
            {
                return "Switch Rules Migrator";
            }
        }

        int _mappingRuleTypeId;
        int _normalizationRuleTypeId;
        readonly Dictionary<string, Switch> _allSwitches;
        readonly Dictionary<string, CarrierAccount> _allCarrierAccounts;
        public SwitchRuleMigrator(RuleMigrationContext context)
            : base(context)
        {
            MappingRuleManager mappingRuleManager = new MappingRuleManager();
            _mappingRuleTypeId = mappingRuleManager.GetRuleTypeId();

            NormalizationRuleManager normalizationRuleManager = new NormalizationRuleManager();
            _normalizationRuleTypeId = normalizationRuleManager.GetRuleTypeId();

            var dbTableSwitches = Context.MigrationContext.DBTables[DBTableName.Switch];
            _allSwitches = (Dictionary<string, Switch>)dbTableSwitches.Records;

            var dbTableCarrierAccount = Context.MigrationContext.DBTables[DBTableName.CarrierAccount];
            _allCarrierAccounts = (Dictionary<string, CarrierAccount>)dbTableCarrierAccount.Records;
        }

        public override IEnumerable<SourceRule> GetSourceRules()
        {
            List<SourceRule> rules = new List<SourceRule>();

            List<SourceSwitch> sourceSwitches = GetSourceSwitches();
            SwitchRulesMigrator switchMigrator = null;
            SwitchMigrationContext switchMigrationContext = new SwitchMigrationContext
            {
                MappingRuleTypeId = _mappingRuleTypeId,
                MigrationContext = Context,
                NormalizationRuleTypeId = _normalizationRuleTypeId
            };

            foreach (SourceSwitch sourceSwitch in sourceSwitches)
            {
                string switchManagerFQTN = GetSourceSwitchManagerFQTN(sourceSwitch);
                switchMigrationContext.SourceSwitch = sourceSwitch;
                switch (switchManagerFQTN)
                {
                    case "TABS.Addons.MvtsProSwitchLibraryMultipleQueue.SwitchManager":
                        switchMigrator = new MVTSSwitchMigrator();
                        break;
                    default:
                        TotalRowsFailed++;
                        switchMigrator = null;
                        break;
                }

                if (switchMigrator != null)
                {
                    switchMigrator.Context = switchMigrationContext;
                    switchMigrator.Execute();
                    rules.AddRange(GetMappingRules(switchMigrationContext));
                }
            }
            return rules;
        }

        #region Private Methods
        IEnumerable<SourceRule> GetMappingRules(SwitchMigrationContext switchMigrationContext)
        {
            List<MappingRule> mappingRules = new List<MappingRule>();
            Switch vrSwitch = _allSwitches[switchMigrationContext.SourceSwitch.SourceId];
            foreach (var switchMapping in switchMigrationContext.SwitchMappings)
            {
                mappingRules.Add(GetMappingRule(switchMapping.CarrierId, switchMapping.InMapping, vrSwitch, 1));
                mappingRules.Add(GetMappingRule(switchMapping.CarrierId, switchMapping.OutMapping, vrSwitch, 2));
            }
            return GetSourceRulesFromMappingRules(mappingRules);
        }
        IEnumerable<SourceRule> GetSourceRulesFromMappingRules(List<MappingRule> mappingRules)
        {
            List<SourceRule> rules = new List<SourceRule>();
            foreach (var mappingRule in mappingRules)
            {
                SourceRule rule = new SourceRule
                {
                    Rule = new Rule
                    {
                        BED = RuleMigrator.s_defaultRuleBED,
                        TypeId = _mappingRuleTypeId,
                        RuleDetails = Serializer.Serialize(mappingRule)
                    }
                };
                rules.Add(rule);
            }
            return rules;
        }
        MappingRule GetMappingRule(string carrierId, InOutMapping inOutMapping, Switch vrSwitch, long carrierType)
        {
            CarrierAccount carrier;
            if (!_allCarrierAccounts.TryGetValue(carrierId, out carrier))
            {
                TotalRowsFailed++;
                return null;
            }

            MappingRule rule = new MappingRule
            {
                Settings = new MappingRuleSettings
                {
                    Value = carrier.CarrierAccountId
                },
                DefinitionId = new Guid("E1ADF1F2-6BC3-4541-8DE4-E5F578A79372"),
                Criteria = new GenericRuleCriteria
                {
                    FieldsValues = new Dictionary<string, GenericRuleCriteriaFieldValues>()
                },
                RuleId = 0,
                Description = string.Format("Migrated Mapping Rule {0}, Switch {1}", Context.Counter++, vrSwitch.Name),
                BeginEffectiveTime = RuleMigrator.s_defaultRuleBED

            };
            rule.Criteria.FieldsValues["Type"] = new StaticValues
            {
                Values = ((new List<long> { carrierType }).Cast<Object>()).ToList()
            };
            rule.Criteria.FieldsValues["Switch"] = new StaticValues
            {
                Values = ((new List<int> { vrSwitch.SwitchId }).Cast<Object>()).ToList()
            };
            if (inOutMapping.InOutCarrier != null)
                rule.Criteria.FieldsValues["Carrier"] = new StaticValues { Values = inOutMapping.InOutCarrier };

            if (inOutMapping.InOutPrefix != null)
                rule.Criteria.FieldsValues["Prefix"] = new StaticValues { Values = inOutMapping.InOutPrefix };

            if (inOutMapping.InOutTrunk != null)
                rule.Criteria.FieldsValues["Trunk"] = new StaticValues { Values = inOutMapping.InOutTrunk };

            return rule;

        }
        string GetSourceSwitchManagerFQTN(SourceSwitch sourceSwitch)
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(sourceSwitch.Configuration);
            if (xml.DocumentElement == null)
                return null;

            return xml.DocumentElement.Attributes["SwitchManager"].InnerText;
        }
        List<SourceSwitch> GetSourceSwitches()
        {

            SourceSwitchDataManager sourceSwitchDataManager = new SourceSwitchDataManager(Context.MigrationContext.ConnectionString);
            List<SourceSwitch> sourceSwitches = sourceSwitchDataManager.GetSourceSwitches();
            List<SourceSwitch> switches = new List<SourceSwitch>();
            foreach (var sourceSwitch in sourceSwitches)
            {
                if (_allSwitches.ContainsKey(sourceSwitch.SourceId))
                    switches.Add(sourceSwitch);
            }
            return switches;
        }

        #endregion
    }
}
