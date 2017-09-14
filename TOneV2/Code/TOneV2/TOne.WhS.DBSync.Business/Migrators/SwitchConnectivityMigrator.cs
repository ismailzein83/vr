using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Data.SQL;
using TOne.WhS.DBSync.Entities;
using System.Linq;

namespace TOne.WhS.DBSync.Business
{
    public class SwitchConnectivityMigrator : Migrator<SourceCarrierSwitchConnectivity, SwitchConnectivity>
    {
        SwitchConnectivityDBSyncDataManager dbSyncDataManager;
        SourceCarrierSwitchConnectivityDataManager dataManager;
        Dictionary<string, CarrierAccount> allCarrierAccounts;
        Dictionary<string, Switch> allSwitches;

        public SwitchConnectivityMigrator(MigrationContext context)
            : base(context)
        {
            dbSyncDataManager = new SwitchConnectivityDBSyncDataManager(Context.UseTempTables);
            dataManager = new SourceCarrierSwitchConnectivityDataManager(Context.ConnectionString);
            TableName = dbSyncDataManager.GetTableName();
            var dbTableCarrierAccount = Context.DBTables[DBTableName.CarrierAccount];
            allCarrierAccounts = (Dictionary<string, CarrierAccount>)dbTableCarrierAccount.Records;
            var dbTableSwitch = Context.DBTables[DBTableName.Switch];
            allSwitches = (Dictionary<string, Switch>)dbTableSwitch.Records;
        }

        public override void Migrate(MigrationInfoContext context)
        {
            base.Migrate(context);
        }

        public override void AddItems(List<SwitchConnectivity> itemsToAdd)
        {
            dbSyncDataManager.ApplySwitchesConnectivityToTemp(itemsToAdd);
            TotalRowsSuccess = itemsToAdd.Count;
        }

        public override IEnumerable<SourceCarrierSwitchConnectivity> GetSourceItems()
        {
            return dataManager.GetSourceCarrierSwitchesConnectivity();
        }

        public override SwitchConnectivity BuildItemFromSource(SourceCarrierSwitchConnectivity sourceItem)
        {
            CarrierAccount carrierAccount = null;
            if (allCarrierAccounts != null)
                allCarrierAccounts.TryGetValue(sourceItem.CarrierAccountId, out carrierAccount);

            Switch switchItem = null;
            if (allSwitches != null)
                allSwitches.TryGetValue(sourceItem.SwitchId.ToString(), out switchItem);

            if (carrierAccount != null && switchItem != null)
            {
                List<SwitchConnectivityTrunk> trunks = new List<SwitchConnectivityTrunk>();
                List<string> details = !string.IsNullOrEmpty(sourceItem.Details) ? sourceItem.Details.Split(',').ToList() : new List<string>();

                foreach (string detail in details)
                {
                    trunks.Add(new SwitchConnectivityTrunk() { Name = detail });
                }

                return new SwitchConnectivity
                {
                    SourceId = sourceItem.SourceId,
                    CarrierAccountId = carrierAccount.CarrierAccountId,
                    SwitchId = switchItem.SwitchId,
                    Name = sourceItem.Name,
                    BED = sourceItem.BED,
                    EED = sourceItem.EED,
                    Settings = new SwitchConnectivitySettings()
                    {
                        ConnectionType = (SwitchConnectivityType)sourceItem.ConnectionType,
                        InChannelCount = sourceItem.NumberOfChannelsIn,
                        OutChannelCount = sourceItem.NumberOfChannelsOut,
                        MaxMargin = (decimal)sourceItem.MarginTotal,
                        SharedChannelCount = sourceItem.NumberOfChannelsShared,
                        Trunks = trunks.Count() == 0 ? null : trunks
                    }
                };
            }

            Context.WriteWarning(string.Format("Failed migrating Switch Connectivity, Source Id: {0}", sourceItem.SourceId));
            TotalRowsFailed++;
            return null;

        }

        public override void FillTableInfo(bool useTempTables)
        {

        }
    }
}
