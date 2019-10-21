using System.Collections.Generic;
using TOne.WhS.Routing.Entities;
using Vanrise.Common;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace TOne.WhS.Routing.Data.RDB
{
    public class RPQualityConfigurationDataManager : RoutingDataManager, IRPQualityConfigurationDataManager
    {
        private static string DBTABLE_SCHEMA = "dbo";
        internal static string DBTABLE_NAME = "RPQualityConfigurationData";
        private static string TABLE_NAME = "dbo_RPQualityConfigurationData";
        private static string TABLE_ALIAS = "rpqcd";

        private const string COL_QualityConfigurationId = "QualityConfigurationId";
        private const string COL_SaleZoneId = "SaleZoneId";
        private const string COL_SupplierId = "SupplierId";
        private const string COL_Quality = "Quality";
        internal const string COL_VersionNumber = "VersionNumber";

        internal static Dictionary<string, RoutingTableColumnDefinition> s_RPQualityConfigurationDataColumnDefinitions;

        static RPQualityConfigurationDataManager()
        {
            s_RPQualityConfigurationDataColumnDefinitions = BuildRPQualityConfigurationDataColumnDefinitions();
            Dictionary<string, RDBTableColumnDefinition> columns = Helper.GetRDBTableColumnDefinitions(s_RPQualityConfigurationDataColumnDefinitions);

            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = DBTABLE_SCHEMA,
                DBTableName = DBTABLE_NAME,
                Columns = columns,
                IdColumnName = COL_QualityConfigurationId
            });
        }

        #region Public Methods

        public object InitialiazeStreamForDBApply()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var bulkInsertQueryContext = queryContext.StartBulkInsert();
            bulkInsertQueryContext.IntoTable(TABLE_NAME, '^', COL_QualityConfigurationId, COL_SaleZoneId, COL_SupplierId, COL_Quality);
            return bulkInsertQueryContext;
        }

        public void WriteRecordToStream(RPQualityConfigurationData record, object dbApplyStream)
        {
            RDBBulkInsertQueryContext bulkInsertQueryContext = dbApplyStream.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream");

            decimal qualityData = record.QualityData >= 0 ? decimal.Round(record.QualityData, 8) : 0;

            var recordContext = bulkInsertQueryContext.WriteRecord();
            recordContext.Value(record.QualityConfigurationId);
            recordContext.Value(record.SaleZoneId);
            recordContext.Value(record.SupplierId);
            recordContext.Value(qualityData);
            //recordContext.Value(record.VersionNumber);
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

        public IEnumerable<RPQualityConfigurationData> GetRPQualityConfigurationData()
        {
            RDBQueryContext queryContext = new RDBQueryContext(GetDataProvider());

            RDBSelectQuery selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            return queryContext.GetItems<RPQualityConfigurationData>(RPQualityConfigurationDataMapper);
        }

        #endregion

        #region Private Methods

        private RPQualityConfigurationData RPQualityConfigurationDataMapper(IRDBDataReader reader)
        {
            return new RPQualityConfigurationData()
            {
                QualityConfigurationId = reader.GetGuid("QualityConfigurationId"),
                SaleZoneId = reader.GetLong("SaleZoneId"),
                SupplierId = reader.GetInt("SupplierId"),
                QualityData = reader.GetDecimal("Quality")
            };
        }

        private static Dictionary<string, RoutingTableColumnDefinition> BuildRPQualityConfigurationDataColumnDefinitions()
        {
            var columnDefinitions = new Dictionary<string, RoutingTableColumnDefinition>();
            columnDefinitions.Add(COL_QualityConfigurationId, new RoutingTableColumnDefinition(COL_QualityConfigurationId, RDBDataType.UniqueIdentifier, true));
            columnDefinitions.Add(COL_SaleZoneId, new RoutingTableColumnDefinition(COL_SaleZoneId, RDBDataType.BigInt, true));
            columnDefinitions.Add(COL_SupplierId, new RoutingTableColumnDefinition(COL_SupplierId, RDBDataType.Int, true));
            columnDefinitions.Add(COL_Quality, new RoutingTableColumnDefinition(COL_Quality, RDBDataType.Decimal, 20, 8, true));
            columnDefinitions.Add(COL_VersionNumber, new RoutingTableColumnDefinition(COL_VersionNumber, RDBDataType.Int, true));
            return columnDefinitions;
        }

        #endregion
    }
}