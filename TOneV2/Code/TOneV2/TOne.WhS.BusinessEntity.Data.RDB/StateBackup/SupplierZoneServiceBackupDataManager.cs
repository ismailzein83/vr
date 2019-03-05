using Vanrise.Data.RDB;
using Vanrise.Entities;
using System.Collections.Generic;

namespace TOne.WhS.BusinessEntity.Data.RDB
{
    public class SupplierZoneServiceBackupDataManager
    {
        #region RDB

        static string TABLE_ALIAS = "supzbkup";
        static string TABLE_NAME = "TOneWhS_BE_Bkup_SupplierZoneService";
        const string COL_ID = "ID";
        const string COL_ZoneID = "ZoneID";
        const string COL_PriceListID = "PriceListID";
        const string COL_SupplierID = "SupplierID";
        const string COL_ReceivedServicesFlag = "ReceivedServicesFlag";
        const string COL_EffectiveServiceFlag = "EffectiveServiceFlag";
        const string COL_BED = "BED";
        const string COL_EED = "EED";
        const string COL_SourceID = "SourceID";
        const string COL_LastModifiedTime = "LastModifiedTime";

        internal const string COL_StateBackupID = "StateBackupID";

        static SupplierZoneServiceBackupDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_ZoneID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_PriceListID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_SupplierID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_ReceivedServicesFlag, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar });
            columns.Add(COL_EffectiveServiceFlag, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar });
            columns.Add(COL_BED, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_EED, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_SourceID, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 50 });
            columns.Add(COL_StateBackupID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "TOneWhS_BE_Bkup",
                DBTableName = "SupplierZoneService",
                Columns = columns,
                ModifiedTimeColumnName = COL_LastModifiedTime
            });
        }

        #endregion

        #region Public Methods

        public RDBInsertQuery GetInsertQuery(RDBQueryContext queryContext, string backupDatabaseName)
        {
            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(new RDBTableDefinitionQuerySource(backupDatabaseName, TABLE_NAME));
            return insertQuery;
        }
        public void AddSelectQuery(RDBInsertQuery insertQuery, string backupDatabaseName, long stateBackupId)
        {
            var selectQuery = insertQuery.FromSelect();
            selectQuery.From(new RDBTableDefinitionQuerySource(backupDatabaseName, TABLE_NAME), TABLE_ALIAS, null, true);
            var selectColumns = selectQuery.SelectColumns();

            selectColumns.Column(COL_ID, COL_ID);
            selectColumns.Column(COL_ZoneID, COL_ZoneID);
            selectColumns.Column(COL_ReceivedServicesFlag, COL_ReceivedServicesFlag);
            selectColumns.Column(COL_EffectiveServiceFlag, COL_EffectiveServiceFlag);
            selectColumns.Column(COL_BED, COL_BED);
            selectColumns.Column(COL_EED, COL_EED);
            selectColumns.Column(COL_SourceID, COL_SourceID);
            selectColumns.Column(COL_PriceListID, COL_PriceListID);
            selectColumns.Column(COL_SupplierID, COL_SupplierID);
            selectColumns.Column(COL_LastModifiedTime, COL_LastModifiedTime);

            var whereQuery = selectQuery.Where();
            whereQuery.EqualsCondition(COL_StateBackupID).Value(stateBackupId);
        }
        #endregion
    }
}
