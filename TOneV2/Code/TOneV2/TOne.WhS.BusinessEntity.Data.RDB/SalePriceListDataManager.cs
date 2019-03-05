using System;
using System.Linq;
using Vanrise.Entities;
using Vanrise.Data.RDB;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data.RDB
{
    public class SalePriceListDataManager : ISalePriceListDataManager
    {
        #region RDB

        static string TABLE_ALIAS = "sp";
        static string TABLE_NAME = "TOneWhS_BE_SalePriceList";
        const string COL_ID = "ID";
        const string COL_CurrencyID = "CurrencyID";
        const string COL_EffectiveOn = "EffectiveOn";
        const string COL_PriceListType = "PriceListType";
        const string COL_SourceID = "SourceID";
        const string COL_ProcessInstanceID = "ProcessInstanceID";
        const string COL_FileID = "FileID";
        const string COL_IsSent = "IsSent";
        const string COL_CreatedTime = "CreatedTime";
        const string COL_UserID = "UserID";
        const string COL_Description = "Description";
        const string COL_PricelistStateBackupID = "PricelistStateBackupID";
        const string COL_PricelistSource = "PricelistSource";
        const string COL_LastModifiedTime = "LastModifiedTime";

        internal const string COL_OwnerType = "OwnerType";
        internal const string COL_OwnerID = "OwnerID";

        static SalePriceListDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_OwnerType, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_OwnerID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_CurrencyID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_EffectiveOn, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_PriceListType, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_SourceID, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 50 });
            columns.Add(COL_ProcessInstanceID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_FileID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_IsSent, new RDBTableColumnDefinition { DataType = RDBDataType.Boolean });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_UserID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_Description, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_PricelistStateBackupID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_PricelistSource, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "TOneWhS_BE",
                DBTableName = "SalePriceList",
                Columns = columns,
                IdColumnName = COL_ID,
                CreatedTimeColumnName = COL_CreatedTime,
                ModifiedTimeColumnName = COL_LastModifiedTime

            });
        }
        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("TOneWhS_BE", "TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString");
        }
        #endregion

        #region ISalePriceListDataManager Members
        public List<SalePriceList> GetPriceLists()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
            return queryContext.GetItems(SalePriceListMapper);
        }

        public bool ArGetSalePriceListsUpdated(ref object updateHandle)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
        }

        public bool SetCustomerPricelistsAsSent(IEnumerable<int> customerIds, int? priceListId)
        {
            if (customerIds == null || !customerIds.Any())
                return false;

            if (!priceListId.HasValue)
                return false;

            int recordsEffected = 0;

            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);

            updateQuery.Column(COL_IsSent).Value(true);
            var whereContext = updateQuery.Where();
            whereContext.EqualsCondition(COL_OwnerType).Value((int)SalePriceListOwnerType.Customer);
            whereContext.EqualsCondition(COL_ID).Value(priceListId.Value);
            whereContext.ListCondition(COL_OwnerID, RDBListConditionOperator.IN, customerIds);
            return queryContext.ExecuteNonQuery() > 0;
        }

        public void SavePriceListsToDb(IEnumerable<NewPriceList> newSalePriceLists)
        {
            SalePriceListNewDataManager salePriceListNewDataManager = new SalePriceListNewDataManager();
            salePriceListNewDataManager.BulkNewPriceLists(newSalePriceLists);
        }

        #endregion

        #region Not Used Function
        public bool Update(SalePriceList salePriceList)
        {
            throw new NotImplementedException();
        }

        public bool Insert(SalePriceList salePriceList)
        {
            throw new NotImplementedException();
        }
        public void SavePriceListsToDb(List<SalePriceList> salePriceLists)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Mappers
        SalePriceList SalePriceListMapper(IRDBDataReader reader)
        {
            SalePriceList salePriceList = new SalePriceList
            {
                OwnerId = reader.GetInt(COL_OwnerID),
                CurrencyId = reader.GetInt(COL_CurrencyID),
                PriceListId = reader.GetInt(COL_ID),
                OwnerType = (SalePriceListOwnerType)reader.GetInt(COL_OwnerType),
                EffectiveOn = reader.GetDateTimeWithNullHandling(COL_EffectiveOn),
                PriceListType = (SalePriceListType?)reader.GetIntWithNullHandling(COL_PriceListType),
                ProcessInstanceId = reader.GetLongWithNullHandling(COL_ProcessInstanceID),
                FileId = reader.GetLongWithNullHandling(COL_FileID),
                CreatedTime = reader.GetDateTimeWithNullHandling(COL_CreatedTime),
                IsSent = reader.GetBooleanWithNullHandling(COL_IsSent),
                SourceId = reader.GetString(COL_SourceID),
                UserId = reader.GetIntWithNullHandling(COL_UserID),
                Description = reader.GetString(COL_Description),
                PricelistStateBackupId = reader.GetNullableLong(COL_PricelistStateBackupID),
                PricelistSource = (SalePricelistSource?)reader.GetNullableInt(COL_PricelistSource)
            };
            return salePriceList;
        }

        #endregion


        #region StateBackup

        public void BackupBySNPId(RDBQueryContext queryContext, long stateBackupId, string backupDatabaseName, int sellingNumberPlanId)
        {
            var salePriceListBackupDataManager = new SalePriceListBackupDataManager();
            var insertCustomerQuery = salePriceListBackupDataManager.GetInsertQuery(queryContext, backupDatabaseName);
            var selectCustomerQuery = insertCustomerQuery.FromSelect();

            SetSelectPriceListQuery(selectCustomerQuery, stateBackupId, sellingNumberPlanId, (int)SalePriceListOwnerType.Customer);

            var insertSPQuery = salePriceListBackupDataManager.GetInsertQuery(queryContext, backupDatabaseName);
            var selectSPQuery = insertSPQuery.FromSelect();

            SetSelectPriceListQuery(selectSPQuery, stateBackupId, sellingNumberPlanId, (int)SalePriceListOwnerType.Customer);
        }

        private void SetSelectPriceListQuery(RDBSelectQuery selectQuery, long stateBackupId, int sellingNumberPlanId, int ownerType)
        {
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            var selectColumns = selectQuery.SelectColumns();
            selectColumns.Column(COL_ID, COL_ID);
            selectColumns.Column(COL_OwnerType, COL_OwnerType);
            selectColumns.Column(COL_OwnerID, COL_OwnerID);
            selectColumns.Column(COL_CurrencyID, COL_CurrencyID);
            selectColumns.Column(COL_EffectiveOn, COL_EffectiveOn);
            selectColumns.Column(COL_PriceListType, COL_PriceListType);
            selectColumns.Column(COL_SourceID, COL_SourceID);
            selectColumns.Column(COL_ProcessInstanceID, COL_ProcessInstanceID);
            selectColumns.Column(COL_FileID, COL_FileID);
            selectColumns.Column(COL_IsSent, COL_IsSent);
            selectColumns.Column(COL_UserID, COL_UserID);
            selectColumns.Column(COL_CreatedTime, COL_CreatedTime);
            selectColumns.Column(COL_Description, COL_Description);
            selectColumns.Column(COL_PricelistStateBackupID, COL_PricelistStateBackupID);
            selectColumns.Column(COL_PricelistSource, COL_PricelistSource);
            selectColumns.Expression(SalePriceListBackupDataManager.COL_StateBackupID).Value(stateBackupId);
            selectColumns.Column(COL_LastModifiedTime, COL_LastModifiedTime);

            var joinContext = selectQuery.Join();

            var carrierAccountDataManager = new CarrierAccountDataManager();
            string carrierAccountTableAlias = "ca";
            carrierAccountDataManager.JoinCarrierAccount(joinContext, carrierAccountTableAlias, TABLE_ALIAS, COL_OwnerID, true);

            var sellingNumberPlanDataManager = new SellingNumberPlanDataManager();
            string sellingNumberTableAlias = "snp";
            sellingNumberPlanDataManager.JoinSellingNumberPlan(joinContext, sellingNumberTableAlias, carrierAccountTableAlias, CarrierAccountDataManager.COL_SellingNumberPlanID, true);

            var whereContext = selectQuery.Where();
            whereContext.EqualsCondition(carrierAccountTableAlias, CarrierAccountDataManager.COL_SellingNumberPlanID).Value(sellingNumberPlanId);
            whereContext.EqualsCondition(COL_OwnerType).Value(ownerType);

        }

        public void BackupByOwner(RDBQueryContext queryContext, long stateBackupId, string backupDatabaseName, IEnumerable<int> ownerIds, int ownerType)
        {
            var salePriceListBackupDataManager = new SalePriceListBackupDataManager();
            var insertQuery = salePriceListBackupDataManager.GetInsertQuery(queryContext, backupDatabaseName);

            var selectQuery = insertQuery.FromSelect();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);

            var selectColumns = selectQuery.SelectColumns();
            selectColumns.Column(COL_ID, COL_ID);
            selectColumns.Column(COL_OwnerType, COL_OwnerType);
            selectColumns.Column(COL_OwnerID, COL_OwnerID);
            selectColumns.Column(COL_CurrencyID, COL_CurrencyID);
            selectColumns.Column(COL_EffectiveOn, COL_EffectiveOn);
            selectColumns.Column(COL_PriceListType, COL_PriceListType);
            selectColumns.Column(COL_SourceID, COL_SourceID);
            selectColumns.Column(COL_ProcessInstanceID, COL_ProcessInstanceID);
            selectColumns.Column(COL_FileID, COL_FileID);
            selectColumns.Column(COL_IsSent, COL_IsSent);
            selectColumns.Column(COL_CreatedTime, COL_CreatedTime);
            selectColumns.Column(COL_UserID, COL_UserID);
            selectColumns.Column(COL_Description, COL_Description);
            selectColumns.Column(COL_PricelistStateBackupID, COL_PricelistStateBackupID);
            selectColumns.Column(COL_PricelistSource, COL_PricelistSource);
            selectColumns.Expression(SalePriceListBackupDataManager.COL_StateBackupID).Value(stateBackupId);
            selectColumns.Column(COL_LastModifiedTime, COL_LastModifiedTime);

            var whereQuery = selectQuery.Where();
            whereQuery.ListCondition(COL_OwnerID, RDBListConditionOperator.IN, ownerIds);
            whereQuery.EqualsCondition(COL_OwnerType).Value(ownerType);
        }

        public void SetDeleteQueryBySNPId(RDBQueryContext queryContext, long sellingNumberPlanId)
        {
            SetDeleteOwnerQuery(queryContext, (int)SalePriceListOwnerType.Customer, sellingNumberPlanId);
            SetDeleteOwnerQuery(queryContext, (int)SalePriceListOwnerType.SellingProduct, sellingNumberPlanId);
        }

        private void SetDeleteOwnerQuery(RDBQueryContext queryContext, int ownerType, long sellingNumberPlanId)
        {
            var salePriceListIdTempTableQuery = queryContext.CreateTempTable();
            salePriceListIdTempTableQuery.AddColumn(COL_ID, RDBDataType.BigInt, true);

            var insertToTempTableQuery = queryContext.AddInsertQuery();
            insertToTempTableQuery.IntoTable(salePriceListIdTempTableQuery);

            var fromSelectQuery = insertToTempTableQuery.FromSelect();
            fromSelectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);

            fromSelectQuery.SelectColumns().Column(COL_ID);

            var joinContextCarrierAccount = fromSelectQuery.Join();
            var carrierAccountDataManager = new CarrierAccountDataManager();
            string carrierAccountTableAlias = "ca";
            carrierAccountDataManager.JoinCarrierAccount(joinContextCarrierAccount, carrierAccountTableAlias, TABLE_ALIAS, COL_OwnerID, true);

            var whereContext = fromSelectQuery.Where();
            whereContext.EqualsCondition(carrierAccountTableAlias, CarrierAccountDataManager.COL_SellingNumberPlanID).Value(sellingNumberPlanId);

            whereContext.EqualsCondition(COL_OwnerType).Value(ownerType);

            var deleteQuery = queryContext.AddDeleteQuery();
            deleteQuery.FromTable(TABLE_NAME);

            var joinContext = deleteQuery.Join(TABLE_ALIAS);
            var joinStatement = joinContext.Join(salePriceListIdTempTableQuery, "custPl");
            var joinCondition = joinStatement.On();
            joinCondition.EqualsCondition("custPl", COL_ID, TABLE_ALIAS, COL_ID);
        }

        public void SetDeleteQueryByOwner(RDBQueryContext queryContext, IEnumerable<int> ownerIds, int ownerType)
        {
            var deleteQuery = queryContext.AddDeleteQuery();
            deleteQuery.FromTable(TABLE_NAME);

            var whereContext = deleteQuery.Where();
            whereContext.EqualsCondition(COL_OwnerType).Value(ownerType);
            whereContext.ListCondition(COL_OwnerID, RDBListConditionOperator.IN, ownerIds);
        }

        public void SetRestoreQuery(RDBQueryContext queryContext, long stateBackupId, string backupDatabaseName)
        {
            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);

            var salePriceListBackupDataManager = new SalePriceListBackupDataManager();
            salePriceListBackupDataManager.AddSelectQuery(insertQuery, backupDatabaseName, stateBackupId);
        }
        #endregion

        #region Public Methods
        public void JoinSalePriceList(RDBJoinContext joinContext, string priceListTableAlias, string originalTableAlias, string originalTablePricelistIdCol, bool withNoLockOnJoin)
        {
            var joinStatement = joinContext.Join(TABLE_NAME, priceListTableAlias);
            if (withNoLockOnJoin)
                joinStatement.WithNoLock();
            var joinCondition = joinStatement.On();
            joinCondition.EqualsCondition(originalTableAlias, originalTablePricelistIdCol, priceListTableAlias, COL_ID);
        }

        public void BuildInsertQuery(RDBInsertQuery insertQuery, long processInstanceId, long stateBackupId)
        {
            insertQuery.IntoTable(TABLE_NAME);
            insertQuery.Column(COL_ProcessInstanceID).Value(processInstanceId);
            insertQuery.Column(COL_UserID).Column(COL_UserID);
            insertQuery.Column(COL_PricelistStateBackupID).Value(stateBackupId);
            insertQuery.Column(COL_PricelistSource).Value((int)SalePricelistSource.NumberingPlan);
        }
        #endregion
    }
}
