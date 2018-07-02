using System;
using System.Collections.Generic;
using System.Xml;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Business.SwitchMigration;
using TOne.WhS.DBSync.Data.SQL;
using TOne.WhS.DBSync.Entities;
using TOne.WhS.RouteSync.Entities;
using System.Linq;
using Vanrise.GenericData.Transformation;
using Vanrise.GenericData.Transformation.Entities;
using Vanrise.Rules.Entities;
using Vanrise.Common;

namespace TOne.WhS.DBSync.Business
{
    public class SwitchMigrator : Migrator<SourceSwitch, Switch>
    {
        SwitchDBSyncDataManager dbSyncDataManager;
        SourceSwitchDataManager dataManager;
        readonly Dictionary<string, CarrierAccount> _allCarrierAccounts;
        readonly Dictionary<string, CarrierProfile> _allCarrierProfiles;
        readonly int _ruleTypeId;
        public SwitchMigrator(MigrationContext context)
            : base(context)
        {
            dbSyncDataManager = new SwitchDBSyncDataManager(Context.UseTempTables);
            dataManager = new SourceSwitchDataManager(Context.ConnectionString);
            TableName = dbSyncDataManager.GetTableName();

            var dbTableCarrierAccount = Context.DBTables[DBTableName.CarrierAccount];
            _allCarrierAccounts = (Dictionary<string, CarrierAccount>)dbTableCarrierAccount.Records;

            var dbTableCarrierProfile = Context.DBTables[DBTableName.CarrierProfile];
            _allCarrierProfiles = (Dictionary<string, CarrierProfile>)dbTableCarrierProfile.Records;

            MappingRuleManager mappingRuleManager = new MappingRuleManager();
            _ruleTypeId = mappingRuleManager.GetRuleTypeId();
        }

        public override void Migrate(MigrationInfoContext context)
        {
            base.Migrate(context);
        }

        public override void AddItems(List<Switch> itemsToAdd)
        {
            dbSyncDataManager.ApplySwitchesToTemp(itemsToAdd.OrderBy(s => s.SourceId));
            if (Context.UseTempTables)
                dbSyncDataManager.FixSwitchIds();

            TotalRowsSuccess = itemsToAdd.Count;
        }

        public override IEnumerable<SourceSwitch> GetSourceItems()
        {
            return dataManager.GetSourceSwitches();
        }

        public override Switch BuildItemFromSource(SourceSwitch sourceItem)
        {
            throw new NotImplementedException();
        }

        public override void FillTableInfo(bool useTempTables)
        {
            DBTable dbTableSwitch = Context.DBTables[DBTableName.Switch];
            if (dbTableSwitch != null)
                dbTableSwitch.Records = dbSyncDataManager.GetSwitches(useTempTables);
        }

        public override List<Switch> BuildAllItemsFromSource(IEnumerable<SourceSwitch> sourceItems)
        {
            var switches = new List<Switch>();
            int switchId = 0;
            var mappingRules = new List<MappingRule>();
            var ruleMigrator = new RuleMigrator(Context);

            foreach (var sourceItem in sourceItems)
            {
                switchId++;
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(sourceItem.Configuration);
                if (xml.DocumentElement == null)
                    return null;

                string switchManager = xml.DocumentElement.Attributes["SwitchManager"].InnerText;

                SwitchMigrationParser parser = null;
                switch (switchManager)
                {
                    case "TABS.Addons.MvtsProSwitchLibraryMultipleQueue.SwitchManager": parser = new MVTSSwitchMigrationParser(sourceItem.Configuration); break;
                    case "TABS.Addons.FIKARSwitchLibrary.SwitchManager": parser = new IVSwitchMigrationParser(sourceItem.Configuration); break;
                    case "TABS.Addons.TelesSwitchLibrary.SwitchManager": parser = new TelesSwitchMigrationParser(sourceItem.Configuration, sourceItem.Name); break;
                    case "TABS.Addons.CloudXPointSSWSwitchLibrary.SwitchManager": parser = new IVSwitchMigrationParser(sourceItem.Configuration); break;
                    default: break;
                }
                Switch migratedSwitch = new Switch
                {
                    Name = sourceItem.Name,
                    SourceId = sourceItem.SourceId
                };

                if (parser != null)
                {
                    SwitchData switchData = parser.GetSwitchData(Context, switchId, _allCarrierAccounts, _allCarrierProfiles);
                    migratedSwitch.Settings = new SwitchSettings
                    {
                        RouteSynchronizer = switchData.SwitchRouteSynchronizer
                    };
                    if (switchData.MappingRules != null)
                        mappingRules.AddRange(switchData.MappingRules);
                }
                migratedSwitch.SwitchId = switchId;
                switches.Add(migratedSwitch);
            }
            if (mappingRules.Any())
            {
                List<Rule> sourceRules = GetRules(mappingRules);
                ruleMigrator.AddItems(sourceRules);
            }
            return switches;
        }

        private List<Rule> GetRules(List<MappingRule> mappingRules)
        {
            return mappingRules.Select(mappingRule => new Rule
                {
                    BED = RuleMigrator.s_defaultRuleBED,
                    EED = null,
                    TypeId = _ruleTypeId,
                    RuleDetails = Serializer.Serialize(mappingRule)
                }
            ).ToList();
        }
        public override bool IsBuildAllItemsOnce
        {
            get { return true; }
        }
    }
}
