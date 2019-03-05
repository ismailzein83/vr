using Vanrise.Data.RDB;
using Vanrise.Entities;
using System.Collections.Generic;


namespace TOne.WhS.BusinessEntity.Data.RDB
{
    public class SaleCodeBackupDataManager
    {
        #region RDB

        static string TABLE_ALIAS = "szbkup";
        static string TABLE_NAME = "TOneWhS_BE_Bkup_SaleCode";
        const string COL_ID = "ID";
        const string COL_Code = "Code";
        const string COL_ZoneID = "ZoneID";
        const string COL_CodeGroupID = "CodeGroupID";
        const string COL_BED = "BED";
        const string COL_EED = "EED";
        const string COL_SourceID = "SourceID";
        internal const string COL_StateBackupID = "StateBackupID";
        const string COL_ProcessInstanceID = "ProcessInstanceID";
        const string COL_LastModifiedTime = "LastModifiedTime";


        static SaleCodeBackupDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_Code, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 20 });
            columns.Add(COL_ZoneID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_CodeGroupID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_BED, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_EED, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_SourceID, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 50 });
            columns.Add(COL_StateBackupID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_ProcessInstanceID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "TOneWhS_BE_Bkup",
                DBTableName = "SaleCode",
                Columns = columns,
                ModifiedTimeColumnName = COL_LastModifiedTime
            });
        }
        #endregion

        #region Public Methods
        public RDBInsertQuery GetInsertQuery(RDBQueryContext queryContext, string backupDatabaseName)
        {
            var insertCustomerQuery = queryContext.AddInsertQuery();
            insertCustomerQuery.IntoTable(new RDBTableDefinitionQuerySource(backupDatabaseName, TABLE_NAME));
            return insertCustomerQuery;
        }
        public void AddSelectQuery(RDBInsertQuery insertQuery, string backupDatabaseName, long stateBackupId)
        {
            var selectQuery = insertQuery.FromSelect();
            selectQuery.From(new RDBTableDefinitionQuerySource(backupDatabaseName, TABLE_NAME), TABLE_ALIAS, null, true);
            var selectColumns = selectQuery.SelectColumns();

            selectColumns.Column(COL_ID, COL_ID);
            selectColumns.Column(COL_Code, COL_Code);
            selectColumns.Column(COL_ZoneID, COL_ZoneID);
            selectColumns.Column(COL_CodeGroupID, COL_CodeGroupID);
            selectColumns.Column(COL_BED, COL_BED);
            selectColumns.Column(COL_EED, COL_EED);
            selectColumns.Column(COL_SourceID, COL_SourceID);
            selectColumns.Column(COL_ProcessInstanceID, COL_ProcessInstanceID);
            selectColumns.Column(COL_LastModifiedTime, COL_LastModifiedTime);

            var whereQuery = selectQuery.Where();
            whereQuery.EqualsCondition(COL_StateBackupID).Value(stateBackupId);
        }
        #endregion
    }
}
