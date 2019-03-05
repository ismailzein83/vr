using Vanrise.Data.RDB;
using Vanrise.Entities;
using System.Collections.Generic;

namespace TOne.WhS.BusinessEntity.Data.RDB
{
    public class CustomerCountryBackupDataManager
    {
        #region RDB

        static string TABLE_ALIAS = "ccbkup";
        static string TABLE_NAME = "TOneWhS_BE_Bkup_CustomerCountry";
        const string COL_ID = "ID";
        const string COL_CustomerID = "CustomerID";
        const string COL_CountryID = "CountryID";
        const string COL_BED = "BED";
        const string COL_EED = "EED";
        const string COL_ProcessInstanceID = "ProcessInstanceID";
        const string COL_LastModifiedTime = "LastModifiedTime";

        internal const string COL_StateBackupID = "StateBackupID";

        static CustomerCountryBackupDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_CustomerID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_CountryID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_BED, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_EED, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_StateBackupID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_ProcessInstanceID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "TOneWhS_BE_Bkup",
                DBTableName = "CustomerCountry",
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

        public void AddSelectQuery(RDBInsertQuery insertQuery, string backupDataBaseName, long stateBackupId)
        {
            var selectQuery = insertQuery.FromSelect();
            selectQuery.From(new RDBTableDefinitionQuerySource(backupDataBaseName, TABLE_NAME), TABLE_ALIAS, null, true);

            var selectColumns = selectQuery.SelectColumns();

            selectColumns.Column(COL_ID, COL_ID);
            selectColumns.Column(COL_CustomerID, COL_CustomerID);
            selectColumns.Column(COL_CountryID, COL_CountryID);
            selectColumns.Column(COL_BED, COL_BED);
            selectColumns.Column(COL_EED, COL_EED);
            selectColumns.Column(COL_ProcessInstanceID, COL_ProcessInstanceID);
            selectColumns.Column(COL_LastModifiedTime, COL_LastModifiedTime);

            var whereContext = selectQuery.Where();
            whereContext.EqualsCondition(COL_StateBackupID).Value(stateBackupId);
        }
        #endregion
    }
}
