using System.Collections.Generic;
using System.Xml;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Data.SQL;
using TOne.WhS.DBSync.Entities;
using TOne.WhS.RouteSync.Entities;

namespace TOne.WhS.DBSync.Business
{
    public class SwitchMigrator : Migrator<SourceSwitch, Switch>
    {
        SwitchDBSyncDataManager dbSyncDataManager;
        SourceSwitchDataManager dataManager;
        readonly Dictionary<string, CarrierAccount> _allCarrierAccounts;
        public SwitchMigrator(MigrationContext context)
            : base(context)
        {
            dbSyncDataManager = new SwitchDBSyncDataManager(Context.UseTempTables);
            dataManager = new SourceSwitchDataManager(Context.ConnectionString);
            TableName = dbSyncDataManager.GetTableName();

            var dbTableCarrierAccount = Context.DBTables[DBTableName.CarrierAccount];
            _allCarrierAccounts = (Dictionary<string, CarrierAccount>)dbTableCarrierAccount.Records;
        }

        public override void Migrate(MigrationInfoContext context)
        {
            base.Migrate(context);
        }

        public override void AddItems(List<Switch> itemsToAdd)
        {
            dbSyncDataManager.ApplySwitchesToTemp(itemsToAdd);
            TotalRowsSuccess = itemsToAdd.Count;
        }

        public override IEnumerable<SourceSwitch> GetSourceItems()
        {
            return dataManager.GetSourceSwitches();
        }

        public override Switch BuildItemFromSource(SourceSwitch sourceItem)
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(sourceItem.Configuration);
            if (xml.DocumentElement == null)
                return null;

            string switchManager = xml.DocumentElement.Attributes["SwitchManager"].InnerText;

            SwitchMigrationParser parser = null;
            switch (switchManager)
            {
                case "TABS.Addons.MvtsProSwitchLibraryMultipleQueue.SwitchManager": parser = new MVTSSwitchMigrationParser(sourceItem.Configuration); break;
                default: break;
            }
            return new Switch
            {
                Name = sourceItem.Name,
                SourceId = sourceItem.SourceId,
                Settings = parser != null ? new SwitchSettings() { RouteSynchronizer = parser.GetSwitchRouteSynchronizer(_allCarrierAccounts) } : null
            };
        }

        public override void FillTableInfo(bool useTempTables)
        {
            DBTable dbTableSwitch = Context.DBTables[DBTableName.Switch];
            if (dbTableSwitch != null)
                dbTableSwitch.Records = dbSyncDataManager.GetSwitches(useTempTables);
        }
    }
}
