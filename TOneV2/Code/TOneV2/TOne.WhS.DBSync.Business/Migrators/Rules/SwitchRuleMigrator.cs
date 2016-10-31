using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Data.SQL;
using TOne.WhS.DBSync.Entities;
using Vanrise.GenericData.Normalization;
using Vanrise.GenericData.Transformation;

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
        Dictionary<string, Switch> _allSwitches;
        public SwitchRuleMigrator(RuleMigrationContext context)
            : base(context)
        {
            MappingRuleManager mappingRuleManager = new MappingRuleManager();
            _mappingRuleTypeId = mappingRuleManager.GetRuleTypeId();

            NormalizationRuleManager normalizationRuleManager = new NormalizationRuleManager();
            _normalizationRuleTypeId = normalizationRuleManager.GetRuleTypeId();

            var dbTableSwitches = Context.MigrationContext.DBTables[DBTableName.Switch];
            _allSwitches = (Dictionary<string, Switch>)dbTableSwitches.Records;

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
                        //switchMigrator = new MVTSSwitchMigrator();
                        break;
                    default:
                        TotalRowsFailed++;
                        switchMigrator = null;
                        break;
                }

                if (switchMigrator != null)
                {
                    rules.AddRange(switchMigrator.GetSourceRules());
                }
            }
            return rules;
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
    }
}
