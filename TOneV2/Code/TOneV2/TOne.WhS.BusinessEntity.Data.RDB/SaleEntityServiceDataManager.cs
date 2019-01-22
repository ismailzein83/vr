using System;
using Vanrise.Common;
using Vanrise.Data.RDB;
using Vanrise.Entities;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data.RDB
{
    public class SaleEntityServiceDataManager : ISaleEntityServiceDataManager
    {
        #region RDB

        static string TABLE_ALIAS = "ses";
        static string TABLE_NAME = "TOneWhS_BE_SaleEntityService";
        public const string COL_ID = "ID";
        const string COL_PriceListID = "PriceListID";
        const string COL_ZoneID = "ZoneID";
        const string COL_Services = "Services";
        const string COL_BED = "BED";
        const string COL_EED = "EED";
        const string COL_SourceID = "SourceID";
        const string COL_LastModifiedTime = "LastModifiedTime";
        const string COL_CreatedTime = "CreatedTime";


        static SaleEntityServiceDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_PriceListID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_ZoneID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_Services, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_BED, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_EED, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_SourceID, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 50 });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });

            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "TOneWhS_BE",
                DBTableName = "SaleEntityService",
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

        #region Members
        public IEnumerable<SaleEntityDefaultService> GetEffectiveSaleEntityDefaultServices(DateTime? effectiveOn)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var whereContext = selectQuery.Where();
            whereContext.NullCondition(COL_ZoneID);

            if (effectiveOn.HasValue)
                BEDataUtility.SetEffectiveDateCondition(whereContext, TABLE_ALIAS, COL_BED, COL_EED, effectiveOn.Value);
            else
                whereContext.FalseCondition();//effectiveOn should be required

            return queryContext.GetItems(SaleEntityDefaultServiceMapper);
        }

        public IEnumerable<SaleEntityZoneService> GetEffectiveSaleEntityZoneServices(SalePriceListOwnerType ownerType, int ownerId, DateTime? effectiveOn)
        {
            var salePriceListDataManager = new SalePriceListDataManager();
            string salePriceListTableAlias = "sp";
            var queryContext = new RDBQueryContext(GetDataProvider());

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var joinQuery = selectQuery.Join();
            salePriceListDataManager.JoinSalePriceList(joinQuery, salePriceListTableAlias, TABLE_ALIAS, COL_PriceListID, true);

            var whereQuery = selectQuery.Where();
            whereQuery.NotNullCondition(COL_ZoneID);
            whereQuery.EqualsCondition(salePriceListTableAlias, SalePriceListDataManager.COL_OwnerType).Value((int)ownerType);
            whereQuery.EqualsCondition(salePriceListTableAlias, SalePriceListDataManager.COL_OwnerID).Value(ownerId);

            if (effectiveOn.HasValue)
                BEDataUtility.SetEffectiveDateCondition(whereQuery, TABLE_ALIAS, COL_BED, COL_EED, effectiveOn.Value);
            else
                whereQuery.FalseCondition();//effectiveOn should be required

            return queryContext.GetItems(SaleEntityZoneServiceMapper);
        }

        public IEnumerable<SaleEntityZoneService> GetEffectiveSaleEntityZoneServicesByOwner(IEnumerable<RoutingCustomerInfoDetails> customerInfos, DateTime? effectiveOn, bool isEffectiveInFuture)
        {
            SalePriceListDataManager salePriceListDataManager = new SalePriceListDataManager();
            string salePriceListTableAlias = "sp";
            var queryContext = new RDBQueryContext(GetDataProvider());

            var tempTableQuery = queryContext.CreateTempTable();
            tempTableQuery.AddColumn(SalePriceListDataManager.COL_OwnerID, RDBDataType.Int, true);
            tempTableQuery.AddColumn(SalePriceListDataManager.COL_OwnerType, RDBDataType.Int, true);

            var insertMultipleRowsQuery = queryContext.AddInsertMultipleRowsQuery();
            insertMultipleRowsQuery.IntoTable(tempTableQuery);

            HashSet<int> addedSellingProductIds = new HashSet<int>();
            foreach (var queryItem in customerInfos)
            {
                var rowContext = insertMultipleRowsQuery.AddRow();
                rowContext.Column(SalePriceListDataManager.COL_OwnerID).Value(queryItem.CustomerId);
                rowContext.Column(SalePriceListDataManager.COL_OwnerType).Value((int)SalePriceListOwnerType.Customer);

                if (addedSellingProductIds.Contains(queryItem.SellingProductId))
                    continue;

                var rowSellingProductContext = insertMultipleRowsQuery.AddRow();
                rowSellingProductContext.Column(SalePriceListDataManager.COL_OwnerID).Value(queryItem.SellingProductId);
                rowSellingProductContext.Column(SalePriceListDataManager.COL_OwnerType).Value((int)SalePriceListOwnerType.SellingProduct);

                addedSellingProductIds.Add(queryItem.SellingProductId);
            }

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var joinContext = selectQuery.Join();
            salePriceListDataManager.JoinSalePriceList(joinContext, salePriceListTableAlias, TABLE_ALIAS, COL_PriceListID, true);

            var joinStatement = joinContext.Join(tempTableQuery, "customerInfo");
            var joinCondition = joinStatement.On();
            joinCondition.EqualsCondition(salePriceListTableAlias, SalePriceListDataManager.COL_OwnerID, "customerInfo", SalePriceListDataManager.COL_OwnerID);
            joinCondition.EqualsCondition(salePriceListTableAlias, SalePriceListDataManager.COL_OwnerType, "customerInfo", SalePriceListDataManager.COL_OwnerType);

            var whereQuery = selectQuery.Where();

            if (effectiveOn.HasValue)
            {
                if (isEffectiveInFuture)
                    BEDataUtility.SetFutureDateCondition(whereQuery, TABLE_ALIAS, COL_BED, COL_EED, DateTime.Now);
                else
                    BEDataUtility.SetEffectiveAfterDateCondition(whereQuery, TABLE_ALIAS, COL_BED, COL_EED, effectiveOn.Value);
            }
            else
                whereQuery.FalseCondition();

            return queryContext.GetItems(SaleEntityZoneServiceMapper);
        }

        public IEnumerable<SaleEntityZoneService> GetSaleZonesServicesEffectiveAfter(int sellingNumberPlanId, DateTime effectiveOn)
        {
            SaleZoneDataManager saleZoneDataManager = new SaleZoneDataManager();
            string saleZoneTableAlias = "sz";
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var joinContext = selectQuery.Join();
            saleZoneDataManager.JoinSaleZone(joinContext, saleZoneTableAlias, TABLE_ALIAS, COL_ZoneID, true);

            var whereQuery = selectQuery.Where();
            whereQuery.EqualsCondition(saleZoneTableAlias, SaleZoneDataManager.COL_SellingNumberPlanID).Value(sellingNumberPlanId);

            var orCondition = whereQuery.ChildConditionGroup(RDBConditionGroupOperator.OR);
            orCondition.NullCondition(COL_EED);
            orCondition.GreaterThanCondition(COL_EED).Value(effectiveOn);

            whereQuery.NotNullCondition(COL_ZoneID);
            return queryContext.GetItems(SaleEntityZoneServiceMapper);
        }

        public IEnumerable<SaleZoneRoutingProduct> GetSaleZoneRoutingProductsEffectiveAfter(int sellingNumberPlanId, DateTime effectiveOn)
        {
            SaleEntityRoutingProductDataManager saleEntityRoutingProductDataManager = new SaleEntityRoutingProductDataManager();
            return saleEntityRoutingProductDataManager.GetSaleZoneRoutingProductsEffectiveAfter(sellingNumberPlanId, effectiveOn);
        }

        public IEnumerable<SaleEntityDefaultService> GetDefaultServicesEffectiveAfter(SalePriceListOwnerType ownerType, int ownerId, DateTime minimumDate)
        {
            SalePriceListDataManager salePriceListDataManager = new SalePriceListDataManager();
            string salePriceListTableAlias = "sp";

            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var joinContext = selectQuery.Join();
            salePriceListDataManager.JoinSalePriceList(joinContext, salePriceListTableAlias, TABLE_ALIAS, COL_PriceListID, true);

            var whereQuery = selectQuery.Where();
            whereQuery.EqualsCondition(salePriceListTableAlias, SalePriceListDataManager.COL_OwnerType).Value((int)ownerType);
            whereQuery.EqualsCondition(salePriceListTableAlias, SalePriceListDataManager.COL_OwnerID).Value(ownerId);
            whereQuery.NullCondition(COL_ZoneID);

            var orCondition = whereQuery.ChildConditionGroup(RDBConditionGroupOperator.OR);
            orCondition.NullCondition(COL_EED);
            orCondition.GreaterThanCondition(COL_EED).Value(minimumDate);

            return queryContext.GetItems(SaleEntityDefaultServiceMapper);
        }

        public IEnumerable<SaleEntityZoneService> GetZoneServicesEffectiveAfter(SalePriceListOwnerType ownerType, int ownerId, DateTime minimumDate)
        {
            SalePriceListDataManager salePriceListDataManager = new SalePriceListDataManager();
            string salePriceListTableAlias = "sp";

            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var joinContext = selectQuery.Join();
            salePriceListDataManager.JoinSalePriceList(joinContext, salePriceListTableAlias, TABLE_ALIAS, COL_PriceListID, true);

            var whereQuery = selectQuery.Where();
            whereQuery.EqualsCondition(salePriceListTableAlias, SalePriceListDataManager.COL_OwnerType).Value((int)ownerType);
            whereQuery.EqualsCondition(salePriceListTableAlias, SalePriceListDataManager.COL_OwnerID).Value(ownerId);
            whereQuery.NotNullCondition(COL_ZoneID);

            var orCondition = whereQuery.ChildConditionGroup(RDBConditionGroupOperator.OR);
            orCondition.NullCondition(COL_EED);
            orCondition.GreaterThanCondition(COL_EED).Value(minimumDate);

            return queryContext.GetItems(SaleEntityZoneServiceMapper);
        }

        public bool AreSaleEntityServicesUpdated(ref object updateHandle)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
        }
        #endregion

        #region Not Used Functions
        public IEnumerable<SaleEntityZoneService> GetFilteredSaleEntityZoneService(SaleEntityZoneServiceQuery query)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Mappers

        private SaleEntityDefaultService SaleEntityDefaultServiceMapper(IRDBDataReader reader)
        {
            return new SaleEntityDefaultService
            {
                SaleEntityServiceId = reader.GetInt(COL_ID),
                PriceListId = reader.GetInt(COL_PriceListID),
                Services = Serializer.Deserialize<List<ZoneService>>(reader.GetString(COL_Services)),
                BED = reader.GetDateTime(COL_BED),
                EED = reader.GetNullableDateTime(COL_EED)
            };
        }

        private SaleEntityZoneService SaleEntityZoneServiceMapper(IRDBDataReader reader)
        {
            return new SaleEntityZoneService
            {
                SaleEntityServiceId = reader.GetLong(COL_ID),
                PriceListId = reader.GetInt(COL_PriceListID),
                ZoneId = reader.GetLongWithNullHandling(COL_ZoneID),
                Services = Serializer.Deserialize<List<ZoneService>>(reader.GetString(COL_Services)),
                BED = reader.GetDateTime(COL_BED),
                EED = reader.GetNullableDateTime(COL_EED)
            };
        }

        private SaleZoneRoutingProduct SaleZoneRoutingProductMapper(IRDBDataReader reader)
        {
            return new SaleZoneRoutingProduct
            {
                //SaleEntityRoutingProductId = reader.GetLong("ID"),
                //RoutingProductId = (int)reader["RoutingProductID"],
                //OwnerId = (int)reader["OwnerID"],
                //OwnerType = GetReaderValue<SalePriceListOwnerType>(reader, "OwnerType"),
                //SaleZoneId = (long)reader["ZoneID"],
                //BED = (DateTime)reader["BED"],
                //EED = GetReaderValue<DateTime?>(reader, "EED")
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
