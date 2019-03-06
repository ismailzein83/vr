using System.Linq;
using Vanrise.Entities;
using Vanrise.Data.RDB;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.BusinessEntity.Entities.SalePricelistChanges;
using Vanrise.Common;

namespace TOne.WhS.BusinessEntity.Data.RDB
{
    public class SalePricelistRPChangeNewDataManager
    {
        #region RDB

        static string TABLE_ALIAS = "sprdc";
        static string TABLE_NAME = "TOneWhS_BE_SalePricelistRPChange_New";
        const string COL_ZoneName = "ZoneName";
        const string COL_ZoneID = "ZoneID";
        const string COL_RoutingProductId = "RoutingProductId";
        const string COL_RecentRoutingProductId = "RecentRoutingProductId";
        const string COL_BED = "BED";
        const string COL_EED = "EED";
        const string COL_PriceListId = "PriceListId";
        const string COL_CountryId = "CountryId";
        const string COL_ProcessInstanceID = "ProcessInstanceID";
        const string COL_CustomerId = "CustomerId";


        static SalePricelistRPChangeNewDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ZoneName, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_ZoneID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_RoutingProductId, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_RecentRoutingProductId, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_BED, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_EED, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_PriceListId, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_CountryId, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_ProcessInstanceID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_CustomerId, new RDBTableColumnDefinition { DataType = RDBDataType.Int });

            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "TOneWhS_BE",
                DBTableName = "SalePricelistRPChange_New",
                Columns = columns
            });
        }

        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("TOneWhS_BE", "TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString");
        }

        #endregion

        #region Public Methods

        public IEnumerable<RoutingProductPreview> GetSalePriceListRPChangeNewByCustomerId(long processInstanceId, List<int> customerIds)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var whereContext = selectQuery.Where();
            whereContext.EqualsCondition(COL_ProcessInstanceID).Value(processInstanceId);

            if (customerIds != null && customerIds.Any())
                whereContext.ListCondition(COL_CustomerId, RDBListConditionOperator.IN, customerIds);

            return queryContext.GetItems(RoutingProductPreviewMapper);
        }
        public IEnumerable<int> GetAffectedCustomerIds(long processInstanceId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().Column(COL_CustomerId);

            var whereContext = selectQuery.Where();
            whereContext.EqualsCondition(COL_ProcessInstanceID).Value(processInstanceId);

            var groupByContext = selectQuery.GroupBy();
            groupByContext.Select().Column(COL_CustomerId);

            var lstAffectedCustomerIds = new List<int>();
            queryContext.ExecuteReader(reader =>
            {
                while (reader.Read())
                {
                    lstAffectedCustomerIds.Add(reader.GetInt(COL_CustomerId));
                }
            });
            return lstAffectedCustomerIds;
        }
        public void DeleteRecords(RDBDeleteQuery deleteQuery, long processInstanceId)
        {
            deleteQuery.FromTable(TABLE_NAME);
            deleteQuery.Where().EqualsCondition(COL_ProcessInstanceID).Value(processInstanceId);
        }

        public void BuildSelectQuery(RDBSelectQuery selectQuery, long processInstanceId)
        {
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().Columns(COL_ZoneName, COL_ZoneID, COL_RoutingProductId, COL_RecentRoutingProductId, COL_BED, COL_EED,
                COL_PriceListId, COL_CountryId, COL_CustomerId);
            selectQuery.Where().EqualsCondition(COL_ProcessInstanceID).Value(processInstanceId);
        }
        public void Bulk(IEnumerable<SalePricelistRPChange> routingProductChanges, long processInstanceId)
        {
            object dbApplyStream = InitialiazeStreamForDBApply();

            foreach (var routingProductChange in routingProductChanges)
            {
                WriteRecordToStream(routingProductChange, processInstanceId, dbApplyStream);
            }

            object prepareToApplyInfo = FinishDBApplyStream(dbApplyStream);
            ApplyForDB(prepareToApplyInfo);
        }

        #endregion
        #region Bulk Methods
        private object InitialiazeStreamForDBApply()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var streamForBulkInsert = queryContext.StartBulkInsert();
            streamForBulkInsert.IntoTable(TABLE_NAME, '^', COL_ZoneName, COL_ZoneID, COL_RoutingProductId, COL_RecentRoutingProductId, COL_BED,
                COL_EED, COL_PriceListId, COL_CountryId, COL_ProcessInstanceID, COL_CustomerId);
            return streamForBulkInsert;
        }
        private void WriteRecordToStream(SalePricelistRPChange record, long processInstanceId, object dbApplyStream)
        {
            RDBBulkInsertQueryContext bulkInsertContext = dbApplyStream.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream");
            var recordContext = bulkInsertContext.WriteRecord();

            recordContext.Value(record.ZoneName);

            if (record.ZoneId.HasValue)
                recordContext.Value(record.ZoneId.Value);
            else
                recordContext.Value(string.Empty);

            recordContext.Value(record.RoutingProductId);

            if (record.RecentRoutingProductId.HasValue)
                recordContext.Value(record.RecentRoutingProductId.Value);
            else
                recordContext.Value(string.Empty);

            recordContext.Value(record.BED);

            if (record.EED.HasValue)
                recordContext.Value(record.EED.Value);
            else
                recordContext.Value(string.Empty);

            recordContext.Value(record.PriceListId);
            recordContext.Value(record.CountryId);
            recordContext.Value(processInstanceId);
            recordContext.Value(record.CustomerId);
        }
        private object FinishDBApplyStream(object dbApplyStream)
        {
            RDBBulkInsertQueryContext bulkInsertContext = dbApplyStream.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream");
            bulkInsertContext.CloseStream();
            return bulkInsertContext;
        }
        private void ApplyForDB(object preparedObject)
        {
            preparedObject.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream").Apply();

        }

        #endregion
        #region Mapper
        RoutingProductPreview RoutingProductPreviewMapper(IRDBDataReader reader)
        {
            return new RoutingProductPreview
            {
                ZoneName = reader.GetString(COL_ZoneName),
                ZoneId = reader.GetNullableLong(COL_ZoneID),
                RoutingProductId = reader.GetInt(COL_RoutingProductId),
                RecentRoutingProductId = reader.GetInt(COL_RecentRoutingProductId),
                BED = reader.GetDateTime(COL_BED),
                EED = reader.GetNullableDateTime(COL_EED),
                CountryId = reader.GetInt(COL_CountryId),
                CustomerId = reader.GetInt(COL_CustomerId)
            };
        }
        #endregion
    }
}
