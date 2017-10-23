using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Data.SQL;
using TOne.WhS.DBSync.Entities;

namespace TOne.WhS.DBSync.Business
{
    public class SwitchReleaseCauseMigrator : Migrator<SourceSwitchReleaseCause, SwitchReleaseCause>
    {

        SwitchReleaseCauseDBSyncDataManager dbSyncDataManager;
        Dictionary<string, Switch> allSwitches;
        SourceSwitchReleaseCauseDataManager dataManager;
        public SwitchReleaseCauseMigrator(MigrationContext context)
            : base(context)
        {
            var dbTableSwitch = Context.DBTables[DBTableName.Switch];
            allSwitches = (Dictionary<string, Switch>)dbTableSwitch.Records;

            dbSyncDataManager = new SwitchReleaseCauseDBSyncDataManager(Context.UseTempTables);
            dataManager = new SourceSwitchReleaseCauseDataManager(Context.ConnectionString);
            TableName = dbSyncDataManager.GetTableName();
        }

        public override void FillTableInfo(bool useTempTables)
        {
            DBTable dbTableSwitchReleaseCause = Context.DBTables[DBTableName.SwitchReleaseCause];
            if (dbTableSwitchReleaseCause != null)
                dbTableSwitchReleaseCause.Records = dbSyncDataManager.GetSwitchReleaseCauses(useTempTables);
        }

        public override void AddItems(List<SwitchReleaseCause> itemsToAdd)
        {
            dbSyncDataManager.ApplySwitchReleaseCausesToTemp(itemsToAdd, 1);
            TotalRowsSuccess = itemsToAdd.Count;
        }

        public override IEnumerable<SourceSwitchReleaseCause> GetSourceItems()
        {
            return dataManager.GetSourceSwitchReleaseCauses();
        }

        public override SwitchReleaseCause BuildItemFromSource(SourceSwitchReleaseCause sourceItem)
        {
            Switch vSwitch;
            if (!allSwitches.TryGetValue(sourceItem.SwitchID.ToString(), out vSwitch))
            {
                TotalRowsFailed++;
                Context.WriteWarning(string.Format("Failed Migrating Switch Release Cause, Source Id {0}. Switch not found, Switch source Id {1}", sourceItem.SourceId, sourceItem.SwitchID));
                return null;
            }

            return new SwitchReleaseCause
            {
                ReleaseCode = sourceItem.ReleaseCode,
                SwitchId = vSwitch.SwitchId,
                Settings = new SwitchReleaseCauseSetting
                {
                    Description = sourceItem.Description,
                    IsDelivered = sourceItem.IsDelivered
                },
                SourceId = sourceItem.SourceId
            };
        }
    }
}
