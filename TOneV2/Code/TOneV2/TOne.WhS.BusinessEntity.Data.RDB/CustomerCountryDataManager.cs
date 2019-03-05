using Vanrise.Data.RDB;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Entities;
namespace TOne.WhS.BusinessEntity.Data.RDB
{
    public class CustomerCountryDataManager : ICustomerCountryDataManager
    {
        #region RDB

        static string TABLE_ALIAS = "cc";
        static string TABLE_NAME = "TOneWhS_BE_CustomerCountry";
        public const string COL_ID = "ID";
        const string COL_CustomerID = "CustomerID";
        const string COL_CountryID = "CountryID";
        const string COL_BED = "BED";
        const string COL_EED = "EED";
        const string COL_ProcessInstanceID = "ProcessInstanceID";
        const string COL_LastModifiedTime = "LastModifiedTime";
        const string COL_CreatedTime = "CreatedTime";

        const string COL_StateBackupID = "StateBackupID";

        static CustomerCountryDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_CustomerID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_CountryID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_BED, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_EED, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_ProcessInstanceID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });

            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "TOneWhS_BE",
                DBTableName = "CustomerCountry",
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

        #region ICustomerCountryDataManager Members

        public IEnumerable<CustomerCountry2> GetAll()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
            return queryContext.GetItems(CustomerCountryMapper);
        }

        public bool AreAllCustomerCountriesUpdated(ref object updateHandle)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
        }

        #endregion

        #region StateBackup 

        public void BackupBySNPId(RDBQueryContext queryContext, long stateBackupId, string backupDatabaseName, int sellingNumberPlanId)
        {
            var customerCountryBackupDataManager = new CustomerCountryBackupDataManager();
            var insertQuery = customerCountryBackupDataManager.GetInsertQuery(queryContext, backupDatabaseName);

            var selectQuery = insertQuery.FromSelect();

            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            var selectColumns = selectQuery.SelectColumns();
            selectColumns.Column(COL_ID, COL_ID);
            selectColumns.Column(COL_CustomerID, COL_CustomerID);
            selectColumns.Column(COL_CountryID, COL_CountryID);
            selectColumns.Column(COL_BED, COL_BED);
            selectColumns.Column(COL_EED, COL_EED);
            selectColumns.Expression(CustomerCountryBackupDataManager.COL_StateBackupID).Value(stateBackupId);
            selectColumns.Column(COL_ProcessInstanceID, COL_ProcessInstanceID);
            selectColumns.Column(COL_LastModifiedTime, COL_LastModifiedTime);

            var joinContext = selectQuery.Join();
            var carrierAccountDataManager = new CarrierAccountDataManager();
            string carrierAccountTableAlias = "ca";
            carrierAccountDataManager.JoinCarrierAccount(joinContext, carrierAccountTableAlias, TABLE_ALIAS, COL_CustomerID, true);

            var whereContext = selectQuery.Where();
            whereContext.EqualsCondition(carrierAccountTableAlias, CarrierAccountDataManager.COL_SellingNumberPlanID).Value(sellingNumberPlanId);
        }

        public void BackupByOwner(RDBQueryContext queryContext, long stateBackupId, string backupDatabaseName, IEnumerable<int> customerIds)
        {
            var customerCountryBackupDataManager = new CustomerCountryBackupDataManager();
            var insertQuery = customerCountryBackupDataManager.GetInsertQuery(queryContext, backupDatabaseName);

            var selectQuery = insertQuery.FromSelect();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);

            var selectColumns = selectQuery.SelectColumns();
            selectColumns.Column(COL_ID, COL_ID);
            selectColumns.Column(COL_CustomerID, COL_CustomerID);
            selectColumns.Column(COL_CountryID, COL_CountryID);
            selectColumns.Column(COL_BED, COL_BED);
            selectColumns.Column(COL_EED, COL_EED);
            selectColumns.Column(COL_ProcessInstanceID, COL_ProcessInstanceID);
            selectColumns.Expression(CustomerCountryBackupDataManager.COL_StateBackupID).Value(stateBackupId);
            selectColumns.Column(COL_LastModifiedTime, COL_LastModifiedTime);

            var whereContext = selectQuery.Where();
            whereContext.ListCondition(COL_CustomerID, RDBListConditionOperator.IN, customerIds);
        }
        public void SetDeleteQueryBySNPId(RDBQueryContext queryContext, long sellingNumberPlanId)
        {
            var deleteQuery = queryContext.AddDeleteQuery();
            deleteQuery.FromTable(TABLE_NAME);

            var joinContext = deleteQuery.Join(TABLE_ALIAS);
            string carrierAccountTableAlias = "ca";
            var carrierAccountDataManager = new CarrierAccountDataManager();
            carrierAccountDataManager.JoinCarrierAccount(joinContext, carrierAccountTableAlias, TABLE_ALIAS, COL_CustomerID, false);

            var whereContext = deleteQuery.Where();
            whereContext.EqualsCondition(carrierAccountTableAlias, CarrierAccountDataManager.COL_SellingNumberPlanID).Value(sellingNumberPlanId);
        }
        public void SetDeleteQueryByOwner(RDBQueryContext queryContext, IEnumerable<int> customerIds)
        {
            var deleteQuery = queryContext.AddDeleteQuery();
            deleteQuery.FromTable(TABLE_NAME);

            var whereContext = deleteQuery.Where();
            whereContext.ListCondition(COL_CustomerID, RDBListConditionOperator.IN, customerIds);
        }
        public void SetRestoreQuery(RDBQueryContext queryContext, long stateBackupId, string backupDatabaseName)
        {
            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);

            var customerCountryBackupDataManager = new CustomerCountryBackupDataManager();
            customerCountryBackupDataManager.AddSelectQuery(insertQuery, backupDatabaseName, stateBackupId);
        }
        #endregion

        #region Mappers

        private CustomerCountry2 CustomerCountryMapper(IRDBDataReader reader)
        {
            return new CustomerCountry2
            {
                CustomerCountryId = reader.GetInt(COL_ID),
                CustomerId = reader.GetInt(COL_CustomerID),
                CountryId = reader.GetInt(COL_CountryID),
                BED = reader.GetDateTime(COL_BED),
                EED = reader.GetNullableDateTime(COL_EED),
                ProcessInstanceId = reader.GetNullableLong(COL_ProcessInstanceID)
            };
        }

        #endregion

        #region Public Methods
        public void BuildUpdateQuery(RDBUpdateQuery updateQuery, long processInstanceID, string joinTableAlias, string columnName)
        {
            updateQuery.FromTable(TABLE_NAME);
            updateQuery.Column(COL_EED).Column(joinTableAlias, COL_EED);
            updateQuery.Where().EqualsCondition(joinTableAlias, columnName).Value(processInstanceID);
        }
        #endregion
    }
}
