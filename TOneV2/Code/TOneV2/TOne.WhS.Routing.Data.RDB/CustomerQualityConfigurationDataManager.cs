using System.Collections.Generic;
using TOne.WhS.Routing.Entities;
using Vanrise.Common;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace TOne.WhS.Routing.Data.RDB
{
    public class CustomerQualityConfigurationDataManager : RoutingDataManager, ICustomerQualityConfigurationDataManager
    {
        #region Fields/Ctor

        private static string DBTABLE_SCHEMA = "dbo";
        internal static string DBTABLE_NAME = "CustomerQualityConfigurationData";
        private static string TABLE_NAME = "dbo_CustomerQualityConfigurationData";
        private static string TABLE_ALIAS = "cqcd";

        private const string COL_QualityConfigurationId = "QualityConfigurationId";
        private const string COL_SupplierZoneId = "SupplierZoneId";
        private const string COL_Quality = "Quality";
        internal const string COL_VersionNumber = "VersionNumber";

        internal static Dictionary<string, RoutingTableColumnDefinition> s_CustomerQualityConfigurationDataColumnDefinitions;

        static CustomerQualityConfigurationDataManager()
        {
            s_CustomerQualityConfigurationDataColumnDefinitions = BuildCustomerQualityConfigurationDataColumnDefinitions();
            Dictionary<string, RDBTableColumnDefinition> columns = Helper.GetRDBTableColumnDefinitions(s_CustomerQualityConfigurationDataColumnDefinitions);

            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = DBTABLE_SCHEMA,
                DBTableName = DBTABLE_NAME,
                Columns = columns,
                IdColumnName = COL_QualityConfigurationId
            });
        }

        #endregion

        #region Public Methods

        public object InitialiazeStreamForDBApply()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var bulkInsertQueryContext = queryContext.StartBulkInsert();
            bulkInsertQueryContext.IntoTable(TABLE_NAME, '^', COL_QualityConfigurationId, COL_SupplierZoneId, COL_Quality, COL_VersionNumber);
            return bulkInsertQueryContext;
        }

        public void WriteRecordToStream(CustomerRouteQualityConfigurationData record, object dbApplyStream)
        {
            RDBBulkInsertQueryContext bulkInsertQueryContext = dbApplyStream.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream");

            decimal qualityData = record.QualityData >= 0 ? decimal.Round(record.QualityData, 8) : 0;

            var recordContext = bulkInsertQueryContext.WriteRecord();
            recordContext.Value(record.QualityConfigurationId);
            recordContext.Value(record.SupplierZoneId);
            recordContext.Value(qualityData);
            recordContext.Value(record.VersionNumber);
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            RDBBulkInsertQueryContext bulkInsertQueryContext = dbApplyStream.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream");
            bulkInsertQueryContext.CloseStream();
            return bulkInsertQueryContext;
        }

        public void ApplyQualityConfigurationsToDB(object preparedObject)
        {
            preparedObject.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream").Apply();
        }

        public IEnumerable<CustomerRouteQualityConfigurationData> GetCustomerRouteQualityConfigurationsData()
        {
            RDBQueryContext queryContext = new RDBQueryContext(GetDataProvider());

            RDBSelectQuery selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            return queryContext.GetItems<CustomerRouteQualityConfigurationData>(CustomerRouteQualityConfigurationDataMapper);
        }

        public List<CustomerRouteQualityConfigurationData> GetCustomerRouteQualityConfigurationsDataAfterVersionNumber(int versionNumber)
        {
            RDBQueryContext queryContext = new RDBQueryContext(GetDataProvider());

            RDBSelectQuery selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var whereContext = selectQuery.Where();
            whereContext.GreaterThanCondition(COL_VersionNumber).ObjectValue(versionNumber);

            return queryContext.GetItems<CustomerRouteQualityConfigurationData>(CustomerRouteQualityConfigurationDataMapper);
        }

        public void UpdateCustomerRouteQualityConfigurationsData(List<CustomerRouteQualityConfigurationData> customerRouteQualityConfigurationData)
        {
            RDBQueryContext queryContext = new RDBQueryContext(GetDataProvider());

            string tempTableAlias = "temp_cqcd";
            var tempTableQuery = queryContext.CreateTempTable();
            Helper.AddRoutingTempTableColumns(tempTableQuery, s_CustomerQualityConfigurationDataColumnDefinitions, new List<string>() { COL_QualityConfigurationId, COL_SupplierZoneId });

            var insertMultipleRowsQuery = queryContext.AddInsertMultipleRowsQuery();
            insertMultipleRowsQuery.IntoTable(tempTableQuery);

            foreach (var itm in customerRouteQualityConfigurationData)
            {
                var rowContext = insertMultipleRowsQuery.AddRow();
                rowContext.Column(COL_QualityConfigurationId).Value(itm.QualityConfigurationId);
                rowContext.Column(COL_SupplierZoneId).Value(itm.SupplierZoneId);
                rowContext.Column(COL_Quality).Value(itm.SupplierZoneId);
                rowContext.Column(COL_VersionNumber).Value(itm.SupplierZoneId);
            }

            RDBUpdateQuery updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);

            var joinContext = updateQuery.Join(TABLE_ALIAS);
            var joinStatementContext = joinContext.Join(tempTableQuery, tempTableAlias);

            var joinCondition = joinStatementContext.On();
            joinCondition.EqualsCondition(TABLE_ALIAS, COL_QualityConfigurationId, tempTableAlias, COL_QualityConfigurationId);
            joinCondition.EqualsCondition(TABLE_ALIAS, COL_SupplierZoneId, tempTableAlias, COL_SupplierZoneId);

            updateQuery.Column(COL_Quality).Column(tempTableAlias, COL_Quality);
            updateQuery.Column(COL_VersionNumber).Column(tempTableAlias, COL_VersionNumber);

            queryContext.ExecuteNonQuery();
        }

        #endregion

        #region Private Methods

        private CustomerRouteQualityConfigurationData CustomerRouteQualityConfigurationDataMapper(IRDBDataReader reader)
        {
            return new CustomerRouteQualityConfigurationData()
            {
                QualityConfigurationId = reader.GetGuid("QualityConfigurationId"),
                SupplierZoneId = reader.GetLong("SupplierZoneId"),
                QualityData = reader.GetDecimal("Quality"),
                VersionNumber = reader.GetInt("VersionNumber")
            };
        }

        private static Dictionary<string, RoutingTableColumnDefinition> BuildCustomerQualityConfigurationDataColumnDefinitions()
        {
            var columnDefinitions = new Dictionary<string, RoutingTableColumnDefinition>();
            columnDefinitions.Add(COL_QualityConfigurationId, new RoutingTableColumnDefinition(COL_QualityConfigurationId, RDBDataType.UniqueIdentifier, true));
            columnDefinitions.Add(COL_SupplierZoneId, new RoutingTableColumnDefinition(COL_SupplierZoneId, RDBDataType.BigInt, true));
            columnDefinitions.Add(COL_Quality, new RoutingTableColumnDefinition(COL_Quality, RDBDataType.Decimal, 20, 8, true));
            columnDefinitions.Add(COL_VersionNumber, new RoutingTableColumnDefinition(COL_VersionNumber, RDBDataType.Int, true));
            return columnDefinitions;
        }

        #endregion
    }
}