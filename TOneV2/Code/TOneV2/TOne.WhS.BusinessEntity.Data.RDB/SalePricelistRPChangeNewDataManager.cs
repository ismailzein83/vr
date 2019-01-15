using System.Linq;
using Vanrise.Entities;
using Vanrise.Data.RDB;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities.SalePricelistChanges;

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
        const string COL_LastModifiedTime = "LastModifiedTime";
        const string COL_CreatedTime = "CreatedTime";


        static SalePricelistRPChangeNewDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>
            {
                {COL_ZoneName, new RDBTableColumnDefinition {DataType = RDBDataType.NVarchar}},
                {COL_ZoneID, new RDBTableColumnDefinition {DataType = RDBDataType.BigInt}},
                {COL_RoutingProductId, new RDBTableColumnDefinition {DataType = RDBDataType.Int}},
                {COL_RecentRoutingProductId, new RDBTableColumnDefinition {DataType = RDBDataType.Int}},
                {COL_BED, new RDBTableColumnDefinition {DataType = RDBDataType.DateTime}},
                {COL_EED, new RDBTableColumnDefinition {DataType = RDBDataType.DateTime}},
                {COL_PriceListId, new RDBTableColumnDefinition {DataType = RDBDataType.BigInt}},
                {COL_CountryId, new RDBTableColumnDefinition {DataType = RDBDataType.Int}},
                {COL_ProcessInstanceID, new RDBTableColumnDefinition {DataType = RDBDataType.Int}},
                {COL_CustomerId, new RDBTableColumnDefinition {DataType = RDBDataType.Int}},
                {COL_LastModifiedTime, new RDBTableColumnDefinition {DataType = RDBDataType.DateTime}},
                {COL_CreatedTime, new RDBTableColumnDefinition {DataType = RDBDataType.DateTime}}
            };
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "TOneWhS_BE",
                DBTableName = "SalePricelistRPChange_New",
                Columns = columns,
                CreatedTimeColumnName = COL_CreatedTime,
                ModifiedTimeColumnName = COL_LastModifiedTime
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
            var lstAffectedCustomerIds = new List<int>();
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var whereContext = selectQuery.Where();
            whereContext.EqualsCondition(COL_ProcessInstanceID).Value(processInstanceId);

            var groupByContext = selectQuery.GroupBy();
            groupByContext.Select().Column(COL_CustomerId);

            queryContext.ExecuteReader(reader =>
            {
                while (reader.Read())
                {
                    lstAffectedCustomerIds.Add(reader.GetInt("OwnerID"));
                }
            });
            return lstAffectedCustomerIds;
        }

        #endregion

        #region Mapper
        RoutingProductPreview RoutingProductPreviewMapper(IRDBDataReader reader)
        {
            return new RoutingProductPreview
            {
                ZoneName = reader.GetString(COL_ZoneName),
                ZoneId = reader.GetNullableLong(COL_ZoneID),
                RoutingProductId = reader.GetIntWithNullHandling(COL_RoutingProductId),
                RecentRoutingProductId = reader.GetIntWithNullHandling(COL_RecentRoutingProductId),
                BED = reader.GetDateTime(COL_BED),
                EED = reader.GetNullableDateTime(COL_EED),
                CountryId = reader.GetIntWithNullHandling(COL_CountryId),
                CustomerId = reader.GetIntWithNullHandling(COL_CustomerId)
            };
        }
        #endregion
    }
}
