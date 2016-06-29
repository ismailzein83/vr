using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Data.SQL;
using TOne.WhS.DBSync.Entities;
using Vanrise.Entities;
using Vanrise.Common;
using System.Linq;
using TOne.WhS.BusinessEntity.Business;

namespace TOne.WhS.DBSync.Business
{
    public class CustomerZoneMigrator : Migrator<SourceCustomerZone, CustomerZones>
    {
        CustomerZoneDBSyncDataManager dbSyncDataManager;
        bool _UseTempTables;
        public CustomerZoneMigrator(MigrationContext context)
            : base(context)
        {
            dbSyncDataManager = new CustomerZoneDBSyncDataManager(Context.UseTempTables, context.SellingProductId);
            _UseTempTables = context.UseTempTables;
            TableName = dbSyncDataManager.GetTableName();
        }

        public override void Migrate(MigrationInfoContext context)
        {
            base.Migrate(context);
        }

        public override void AddItems(List<CustomerZones> itemsToAdd)
        {
            dbSyncDataManager.ApplyCustomerZoneToTemp(itemsToAdd);
            TotalRows = itemsToAdd.Count;

        }

        public override IEnumerable<SourceCustomerZone> GetSourceItems()
        {
            return dbSyncDataManager.GetCustomerZones(_UseTempTables).Values;
        }

        public override CustomerZones BuildItemFromSource(SourceCustomerZone sourceItem)
        {
            return new CustomerZones
                {
                    CustomerId = sourceItem.CustomerId,
                    Countries = sourceItem.Countries,
                    StartEffectiveTime = sourceItem.StartEffectiveTime
                };
        }

        public override void FillTableInfo(bool useTempTables)
        {

        }

    }
}
