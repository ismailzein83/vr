using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Data.SQL;
using TOne.WhS.DBSync.Entities;
using Vanrise.Entities;
using Vanrise.Common;
using System.Data.SqlClient;
using System.Linq;

namespace TOne.WhS.DBSync.Business
{
    public class CarrierAccountStatusHistoryMigrator : Migrator<SourceCarrierAccountStatusHistory, CarrierAccountStatusHistory>
    {
        CarrierAccountStatusHistoryDBSyncDataManager dbSyncDataManager;
        Dictionary<string, CarrierAccount> allCarrierAccounts;

        MigrationContext _context;
        public CarrierAccountStatusHistoryMigrator(MigrationContext context)
            : base(context)
        {
            dbSyncDataManager = new CarrierAccountStatusHistoryDBSyncDataManager(Context.UseTempTables);
            TableName = dbSyncDataManager.GetTableName();
            var dbTableCarrierAccount = Context.DBTables[DBTableName.CarrierAccount];
            allCarrierAccounts = (Dictionary<string, CarrierAccount>)dbTableCarrierAccount.Records;
            _context = context;
        }

        public override void Migrate(MigrationInfoContext context)
        {
            base.Migrate(context);
            FinalizeRelatedEntities();
        }

        public override void AddItems(List<CarrierAccountStatusHistory> itemsToAdd)
        {
            dbSyncDataManager.ApplyCarrierAccountsStatusHistoryToTemp(itemsToAdd);
            TotalRowsSuccess = itemsToAdd.Count;
        }

        public override IEnumerable<SourceCarrierAccountStatusHistory> GetSourceItems()
        {
            List<SourceCarrierAccountStatusHistory> sourceItems = null;
            if (allCarrierAccounts != null)
            {
                sourceItems = allCarrierAccounts.Values.Select(itm => new SourceCarrierAccountStatusHistory() { CarrierAccountId = itm.CarrierAccountId, ActivationStatus = itm.CarrierAccountSettings.ActivationStatus }).ToList();
            }
            return sourceItems;
        }

        public override CarrierAccountStatusHistory BuildItemFromSource(SourceCarrierAccountStatusHistory sourceItem)
        {
            return new CarrierAccountStatusHistory()
            {
                CarrierAccountId = sourceItem.CarrierAccountId,
                Status = sourceItem.ActivationStatus
            };
        }


        public override void FillTableInfo(bool useTempTables)
        {
            DBTable dbTableCarrierAccountStatusHistory = Context.DBTables[DBTableName.CarrierAccountStatusHistory];
            if (dbTableCarrierAccountStatusHistory != null)
                dbTableCarrierAccountStatusHistory.Records = dbSyncDataManager.GetCarrierAccountsStatusHistory(useTempTables);
        }
    }
}
