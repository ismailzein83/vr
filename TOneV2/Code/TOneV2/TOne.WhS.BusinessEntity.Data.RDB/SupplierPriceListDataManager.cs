using Vanrise.Data.RDB;
using Vanrise.Entities;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data.RDB
{
    public class SupplierPriceListDataManager : ISupplierPriceListDataManager
    {
        #region RDB

        static string TABLE_ALIAS = "spr";
        static string TABLE_NAME = "TOneWhS_BE_SupplierPriceList";
        const string COL_ID = "ID";
        const string COL_CurrencyID = "CurrencyID";
        const string COL_FileID = "FileID";
        const string COL_EffectiveOn = "EffectiveOn";
        const string COL_PricelistType = "PricelistType";
        const string COL_CreatedTime = "CreatedTime";
        const string COL_SourceID = "SourceID";
        const string COL_ProcessInstanceID = "ProcessInstanceID";
        const string COL_SPLStateBackupID = "SPLStateBackupID";
        const string COL_UserID = "UserID";
        const string COL_LastModifiedTime = "LastModifiedTime";

        internal const string COL_SupplierID = "SupplierID";

        static SupplierPriceListDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_SupplierID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_CurrencyID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_FileID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_EffectiveOn, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_PricelistType, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_SourceID, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 50 });
            columns.Add(COL_ProcessInstanceID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_SPLStateBackupID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_UserID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });

            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "TOneWhS_BE",
                DBTableName = "SupplierPriceList",
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

        #region ISupplierPriceListDataManager Members

        public List<SupplierPriceList> GetPriceLists()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
            return queryContext.GetItems(SupplierPriceListMapper);
        }

        public bool ArGetPriceListsUpdated(ref object updateHandle)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
        }

        #endregion

        #region Not Used Functions

        public SupplierPriceList GetPriceList(int priceListId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var whereQuery = selectQuery.Where();
            whereQuery.EqualsCondition(COL_ID).Value(priceListId);
            return queryContext.GetItem(SupplierPriceListMapper);
        }

        #endregion

        #region StateBackup

        public void BackupBySupplierId(RDBQueryContext queryContext, long stateBackupId, string backupDatabaseName, int supplierId)
        {
            var supplierPriceListBackupDataManager = new SupplierPriceListBackupDataManager();
            var insertQuery = supplierPriceListBackupDataManager.GetInsertQuery(queryContext, backupDatabaseName);

            var selectQuery = insertQuery.FromSelect();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);

            var selectColumns = selectQuery.SelectColumns();
            selectColumns.Column(COL_ID, COL_ID);
            selectColumns.Column(COL_SupplierID, COL_SupplierID);
            selectColumns.Column(COL_CurrencyID, COL_CurrencyID);
            selectColumns.Column(COL_FileID, COL_FileID);
            selectColumns.Column(COL_EffectiveOn, COL_EffectiveOn);
            selectColumns.Column(COL_PricelistType, COL_PricelistType);
            selectColumns.Column(COL_CreatedTime, COL_CreatedTime);
            selectColumns.Column(COL_SourceID, COL_SourceID);
            selectColumns.Column(COL_ProcessInstanceID, COL_ProcessInstanceID);
            selectColumns.Column(COL_SPLStateBackupID, COL_SPLStateBackupID);
            selectColumns.Column(COL_UserID, COL_UserID);
            selectColumns.Expression(SupplierPriceListBackupDataManager.COL_StateBackupID).Value(stateBackupId);
            selectColumns.Column(COL_LastModifiedTime, COL_LastModifiedTime);

            var whereContext = selectQuery.Where();
            whereContext.EqualsCondition(COL_SupplierID).Value(supplierId);
        }
        public void SetDeleteQueryBySupplierId(RDBQueryContext queryContext, int supplierId)
        {
            var deleteQuery = queryContext.AddDeleteQuery();
            deleteQuery.FromTable(TABLE_NAME);
            deleteQuery.Where().EqualsCondition(COL_SupplierID).Value(supplierId);
        }
        public void GetRestoreQuery(RDBQueryContext queryContext, long stateBackupId, string backupDatabaseName)
        {
            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_ALIAS);
            var supplierPriceListBackupDataManager = new SupplierPriceListBackupDataManager();
            supplierPriceListBackupDataManager.AddSelectQuery(insertQuery, backupDatabaseName, stateBackupId);
        }
        #endregion

        #region Mappers
        SupplierPriceList SupplierPriceListMapper(IRDBDataReader reader)
        {
            return new SupplierPriceList
            {
                PriceListId = reader.GetInt(COL_ID),
                SupplierId = reader.GetInt(COL_SupplierID),
                CurrencyId = reader.GetInt(COL_CurrencyID),
                FileId = reader.GetNullableLong(COL_FileID),
                EffectiveOn = reader.GetDateTime(COL_EffectiveOn),
                PricelistType = (SupplierPricelistType?)reader.GetNullableInt(COL_PricelistType),
                CreateTime = reader.GetDateTime(COL_CreatedTime),
                ProcessInstanceId = reader.GetNullableLong(COL_ProcessInstanceID),
                SPLStateBackupId = reader.GetNullableLong(COL_SPLStateBackupID),
                UserId = reader.GetInt(COL_UserID),
            };
        }

        #endregion

        #region

        public void JoinSupplierPriceList(RDBJoinContext joinContext, string priceListTableAlias, string originalTableAlias, string originalTablePriceListIdCol, RDBJoinType joinType, bool withNoLock)
        {
            var joinStatement = joinContext.Join(TABLE_NAME, priceListTableAlias);
            joinStatement.JoinType(joinType);
            if (withNoLock)
                joinStatement.WithNoLock();

            var joinCondition = joinStatement.On();
            joinCondition.EqualsCondition(originalTableAlias, originalTablePriceListIdCol, priceListTableAlias, COL_ID);
        }
        #endregion
    }
}
