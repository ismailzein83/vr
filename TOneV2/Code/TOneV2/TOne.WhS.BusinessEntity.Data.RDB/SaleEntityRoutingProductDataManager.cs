using System;
using System.Linq;
using Vanrise.Entities;
using Vanrise.Data.RDB;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data.RDB
{
    public class SaleEntityRoutingProductDataManager : ISaleEntityRoutingProductDataManager
    {
        #region RDB

        static string TABLE_ALIAS = "serp";
        static string TABLE_NAME = "TOneWhS_BE_SaleEntityRoutingProduct";
        public const string COL_ID = "ID";
        const string COL_OwnerType = "OwnerType";
        const string COL_OwnerID = "OwnerID";
        const string COL_ZoneID = "ZoneID";
        const string COL_RoutingProductID = "RoutingProductID";
        const string COL_BED = "BED";
        private const string COL_EED = "EED";
        const string COL_LastModifiedTime = "LastModifiedTime";
        const string COL_CreatedTime = "CreatedTime";
        static SaleEntityRoutingProductDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_OwnerType, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_OwnerID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_ZoneID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_RoutingProductID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_BED, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_EED, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });

            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "TOneWhS_BE",
                DBTableName = "SaleEntityRoutingProduct",
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

        #region ISaleEntityRoutingProductDataManager Members

        public List<SaleZoneRoutingProduct> GetAllZoneRoutingProducts()
        {
            RDBQueryContext queryContext = GetAllRoutingProducts(false);
            return queryContext.GetItems(SaleZoneRoutingProductMapper);
        }

        public List<DefaultRoutingProduct> GetAllDefaultRoutingProducts()
        {
            RDBQueryContext queryContext = GetAllRoutingProducts(true);
            return queryContext.GetItems(DefaultRoutingProductMapper);
        }

        public IEnumerable<DefaultRoutingProduct> GetDefaultRoutingProducts(IEnumerable<int> customerIds, DateTime? effectiveOn, bool isEffectiveInFuture)
        {
            List<DefaultRoutingProduct> routingProducts = new List<DefaultRoutingProduct>();
            RDBQueryContext queryContext = GetRoutingProducts(customerIds, effectiveOn, isEffectiveInFuture, true);
            queryContext.ExecuteReader(reader =>
            {
                while (reader.Read())
                {
                    routingProducts.Add(DefaultRoutingProductMapper(reader));
                }

                reader.NextResult();
                while (reader.Read())
                {
                    routingProducts.Add(DefaultRoutingProductMapper(reader));
                }
            });

            return routingProducts;
        }

        public IEnumerable<DefaultRoutingProduct> GetEffectiveDefaultRoutingProducts(DateTime effectiveOn)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var whereContext = selectQuery.Where();
            whereContext.NullCondition(COL_ZoneID);

            BEDataUtility.SetEffectiveDateCondition(whereContext, TABLE_ALIAS, COL_BED, COL_EED, effectiveOn);

            return queryContext.GetItems(DefaultRoutingProductMapper);
        }

        public IEnumerable<SaleZoneRoutingProduct> GetSaleZoneRoutingProducts(IEnumerable<int> customerIds, DateTime? effectiveOn, bool isEffectiveInFuture)
        {
            RDBQueryContext queryContext = GetRoutingProducts(customerIds, effectiveOn, isEffectiveInFuture, false);
            List<SaleZoneRoutingProduct> routingProducts = new List<SaleZoneRoutingProduct>();
            queryContext.ExecuteReader(reader =>
            {
                while (reader.Read())
                {
                    routingProducts.Add(SaleZoneRoutingProductMapper(reader));
                }

                reader.NextResult();
                while (reader.Read())
                {
                    routingProducts.Add(SaleZoneRoutingProductMapper(reader));
                }
            });
            return routingProducts;
        }

        public IEnumerable<SaleZoneRoutingProduct> GetEffectiveZoneRoutingProducts(SalePriceListOwnerType ownerType, int ownerId, DateTime effectiveOn)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var whereContext = selectQuery.Where();
            whereContext.EqualsCondition(COL_OwnerType).Value((int)ownerType);
            whereContext.EqualsCondition(COL_OwnerID).Value(ownerId);
            whereContext.NotNullCondition(COL_ZoneID);

            BEDataUtility.SetEffectiveDateCondition(whereContext, TABLE_ALIAS, COL_BED, COL_EED, effectiveOn);

            return queryContext.GetItems(SaleZoneRoutingProductMapper);
        }

        public bool AreSaleEntityRoutingProductUpdated(ref object updateHandle)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
        }

        public IEnumerable<DefaultRoutingProduct> GetDefaultRoutingProductsEffectiveAfter(SalePriceListOwnerType ownerType, int ownerId, DateTime minimumDate)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var whereContext = selectQuery.Where();
            whereContext.EqualsCondition(COL_OwnerType).Value((int)ownerType);
            whereContext.EqualsCondition(COL_OwnerID).Value(ownerId);
            whereContext.NullCondition(COL_ZoneID);

            var orCondition = whereContext.ChildConditionGroup(RDBConditionGroupOperator.OR);
            orCondition.NullCondition(COL_EED);
            orCondition.GreaterThanCondition(COL_EED).Value(minimumDate);

            return queryContext.GetItems(DefaultRoutingProductMapper);
        }

        public IEnumerable<SaleZoneRoutingProduct> GetSaleZoneRoutingProductsEffectiveAfter(SalePriceListOwnerType ownerType, int ownerId, DateTime minimumDate)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var whereContext = selectQuery.Where();
            whereContext.EqualsCondition(COL_OwnerType).Value((int)ownerType);
            whereContext.EqualsCondition(COL_OwnerID).Value(ownerId);
            whereContext.NotNullCondition(COL_ZoneID);

            BEDataUtility.SetEffectiveAfterDateCondition(whereContext, TABLE_ALIAS, COL_BED, COL_EED, minimumDate);

            return queryContext.GetItems(SaleZoneRoutingProductMapper);
        }

        public IEnumerable<DefaultRoutingProduct> GetAllDefaultRoutingProductsByOwners(IEnumerable<int> sellingProductIds, IEnumerable<int> customerIds)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var whereCondition = selectQuery.Where();
            whereCondition.NullCondition(COL_ZoneID);

            var dateOrCondition = whereCondition.ChildConditionGroup(RDBConditionGroupOperator.OR);
            dateOrCondition.NullCondition(COL_EED);
            dateOrCondition.GreaterThanCondition(COL_EED).Column(COL_BED);

            var ownerOrCondition = whereCondition.ChildConditionGroup(RDBConditionGroupOperator.OR);

            var sellingProductAndCondition = ownerOrCondition.ChildConditionGroup();
            sellingProductAndCondition.EqualsCondition(COL_OwnerType).Value((int)SalePriceListOwnerType.SellingProduct);
            sellingProductAndCondition.ListCondition(COL_OwnerID, RDBListConditionOperator.IN, sellingProductIds);

            var customerAndCondition = ownerOrCondition.ChildConditionGroup();
            customerAndCondition.EqualsCondition(COL_OwnerType).Value((int)SalePriceListOwnerType.Customer);
            customerAndCondition.ListCondition(COL_OwnerID, RDBListConditionOperator.IN, customerIds);

            return queryContext.GetItems(DefaultRoutingProductMapper);

        }

        public IEnumerable<SaleZoneRoutingProduct> GetAllZoneRoutingProductsByOwners(IEnumerable<int> sellingProductIds, IEnumerable<int> customerIds, IEnumerable<long> zoneIds)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var whereCondition = selectQuery.Where();

            var dateOrCondition = whereCondition.ChildConditionGroup(RDBConditionGroupOperator.OR);
            dateOrCondition.NullCondition(COL_EED);
            dateOrCondition.GreaterThanCondition(COL_EED).Column(COL_BED);

            var zoneCondition = whereCondition.ChildConditionGroup();
            zoneCondition.NotNullCondition(COL_ZoneID);
            zoneCondition.ListCondition(COL_ZoneID, RDBListConditionOperator.IN, zoneIds);

            var ownerOrCondition = whereCondition.ChildConditionGroup(RDBConditionGroupOperator.OR);

            var sellingProductAndCondition = ownerOrCondition.ChildConditionGroup();
            sellingProductAndCondition.EqualsCondition(COL_OwnerType).Value((int)SalePriceListOwnerType.SellingProduct);
            sellingProductAndCondition.ListCondition(COL_OwnerID, RDBListConditionOperator.IN, sellingProductIds);

            var customerAndCondition = ownerOrCondition.ChildConditionGroup();
            customerAndCondition.EqualsCondition(COL_OwnerType).Value((int)SalePriceListOwnerType.Customer);
            customerAndCondition.ListCondition(COL_OwnerID, RDBListConditionOperator.IN, customerIds);

            return queryContext.GetItems(SaleZoneRoutingProductMapper);
        }

        public IEnumerable<DefaultRoutingProduct> GetAllDefaultRoutingProductsByOwners(SalePriceListOwnerType ownerType, IEnumerable<int> ownerIds)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var whereContext = selectQuery.Where();
            whereContext.NullCondition(COL_ZoneID);
            whereContext.EqualsCondition(COL_OwnerType).Value((int)ownerType);
            whereContext.ListCondition(COL_OwnerID, RDBListConditionOperator.IN, ownerIds);

            return queryContext.GetItems(DefaultRoutingProductMapper);
        }

        public IEnumerable<SaleZoneRoutingProduct> GetAllZoneRoutingProductsByOwners(SalePriceListOwnerType ownerType, IEnumerable<int> ownerIds, IEnumerable<long> zoneIds)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var whereContext = selectQuery.Where();
            whereContext.EqualsCondition(COL_OwnerType).Value((int)ownerType);
            whereContext.ListCondition(COL_OwnerID, RDBListConditionOperator.IN, ownerIds);
            whereContext.NotNullCondition(COL_ZoneID);
            whereContext.ListCondition(COL_ZoneID, RDBListConditionOperator.IN, zoneIds);

            return queryContext.GetItems(SaleZoneRoutingProductMapper);
        }

        public bool Update(ZoneRoutingProductToEdit zoneRoutingProductToEdit, long reservedId, List<ZoneRoutingProductToChange> routingProductToChange)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var tempTableQuery = queryContext.CreateTempTable();
            tempTableQuery.AddColumn(COL_RoutingProductID, RDBDataType.BigInt, true);
            tempTableQuery.AddColumn(COL_EED, RDBDataType.DateTime, false);

            var insertMultipleRowsQuery = queryContext.AddInsertMultipleRowsQuery();
            insertMultipleRowsQuery.IntoTable(tempTableQuery);

            foreach (var queryItem in routingProductToChange)
            {
                var rowContext = insertMultipleRowsQuery.AddRow();
                rowContext.Column(COL_EED).Value(queryItem.EED);
                rowContext.Column(COL_RoutingProductID).Value(queryItem.ZoneRoutingProductId);
            }

            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            var joinContext = updateQuery.Join(TABLE_ALIAS);
            joinContext.JoinOnEqualOtherTableColumn(tempTableQuery, "zrpToUpdate", COL_RoutingProductID, TABLE_ALIAS, COL_ID);

            updateQuery.Column(COL_EED).Column("zrpToUpdate", COL_EED);

            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);
            insertQuery.Column(COL_ID).Value(reservedId);
            insertQuery.Column(COL_OwnerType).Value((int)zoneRoutingProductToEdit.OwnerType);
            insertQuery.Column(COL_OwnerID).Value(zoneRoutingProductToEdit.OwnerId);
            insertQuery.Column(COL_ZoneID).Value(zoneRoutingProductToEdit.ZoneId);
            insertQuery.Column(COL_RoutingProductID).Value(zoneRoutingProductToEdit.ChangedRoutingProductId);
            insertQuery.Column(COL_BED).Value(zoneRoutingProductToEdit.BED);

            return queryContext.ExecuteNonQuery(true) > 0;
        }

        #endregion

        #region Not Used Fucntions
        public IEnumerable<DefaultRoutingProduct> GetAllDefaultRoutingProductsByOwner(SalePriceListOwnerType ownerType, int ownerId)
        {
            throw new NotImplementedException();
        }
        public IEnumerable<SaleZoneRoutingProduct> GetAllZoneRoutingProductsByOwner(SalePriceListOwnerType ownerType, int ownerId, IEnumerable<long> saleZoneIds)
        {
            throw new NotImplementedException();
        }
        public IEnumerable<DefaultRoutingProduct> GetAllDefaultRoutingProductsBySellingProductsAndCustomer(IEnumerable<int> sellingProductIds, int customerId)
        {
            throw new NotImplementedException();
        }
        public IEnumerable<SaleZoneRoutingProduct> GetAllZoneRoutingProductsBySellingProductsAndCustomer(IEnumerable<int> sellingProductIds, int customerId, IEnumerable<long> saleZoneIds)
        {
            throw new NotImplementedException();
        }
        public IEnumerable<SaleZoneRoutingProduct> GetExistingZoneRoutingProductsByZoneIds(SalePriceListOwnerType ownerType, int ownerId, IEnumerable<long> zoneIds, DateTime minEED)
        {
            throw new NotImplementedException();
        }
        public IEnumerable<DefaultRoutingProduct> GetAllDefaultRoutingProducts(int sellingProductId, int customerId, IEnumerable<long> zoneIds)
        {
            throw new NotImplementedException();
        }
        public IEnumerable<SaleZoneRoutingProduct> GetAllSaleZoneRoutingProducts(int sellingProductId, int customerId, IEnumerable<long> zoneIds)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Mappers

        private DefaultRoutingProduct DefaultRoutingProductMapper(IRDBDataReader reader)
        {
            return new DefaultRoutingProduct
            {
                SaleEntityRoutingProductId = reader.GetLong(COL_ID),
                RoutingProductId = reader.GetInt(COL_RoutingProductID),
                OwnerType = (SalePriceListOwnerType)reader.GetInt(COL_OwnerType),
                OwnerId = reader.GetInt(COL_OwnerID),
                BED = reader.GetDateTime(COL_BED),
                EED = reader.GetNullableDateTime(COL_EED)
            };
        }

        private SaleZoneRoutingProduct SaleZoneRoutingProductMapper(IRDBDataReader reader)
        {
            return new SaleZoneRoutingProduct
            {
                SaleEntityRoutingProductId = reader.GetLong(COL_ID),
                RoutingProductId = reader.GetInt(COL_RoutingProductID),
                OwnerType = (SalePriceListOwnerType)reader.GetInt(COL_OwnerType),
                OwnerId = reader.GetInt(COL_OwnerID),
                BED = reader.GetDateTime(COL_BED),
                EED = reader.GetNullableDateTime(COL_EED),
                SaleZoneId = reader.GetLongWithNullHandling(COL_ZoneID)
            };
        }

        #endregion

        #region Private Methods

        private RDBQueryContext GetRoutingProducts(IEnumerable<int> customerIds, DateTime? effectiveOn, bool isEffectiveInFuture, bool isDefault)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            if (customerIds != null && customerIds.Any())
            {
                var selectQuery = queryContext.AddSelectQuery();
                selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
                selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

                var whereContext = selectQuery.Where();
                whereContext.ListCondition(COL_OwnerID, RDBListConditionOperator.IN, customerIds);

                SetRoutingProductWhereContext(whereContext, effectiveOn, isEffectiveInFuture, isDefault, (int)SalePriceListOwnerType.Customer);
            }

            var secondSelectQuery = queryContext.AddSelectQuery();
            secondSelectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            secondSelectQuery.SelectColumns().Columns(COL_ID, COL_OwnerType, COL_OwnerID, COL_ZoneID, COL_RoutingProductID, COL_BED, COL_EED);
            var secondWhereContext = secondSelectQuery.Where();
            SetRoutingProductWhereContext(secondWhereContext, effectiveOn, isEffectiveInFuture, isDefault, (int)SalePriceListOwnerType.SellingProduct);

            return queryContext;
        }

        private void SetRoutingProductWhereContext(RDBConditionContext conditionContext, DateTime? effectiveOn, bool isEffectiveInFuture, bool isDefault, int ownerType)
        {
            BEDataUtility.SetDateCondition(conditionContext, TABLE_ALIAS, COL_BED, COL_EED, isEffectiveInFuture, effectiveOn);

            conditionContext.EqualsCondition(COL_OwnerType).Value(ownerType);
            if (isDefault)
                conditionContext.NullCondition(COL_ZoneID);
            else
                conditionContext.NotNullCondition(COL_ZoneID);
        }

        private RDBQueryContext GetAllRoutingProducts(bool isDefault)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var whereContext = selectQuery.Where();

            if (isDefault)
                whereContext.NullCondition(COL_ZoneID);
            else
                whereContext.NotNullCondition(COL_ZoneID);

            var orCondition = whereContext.ChildConditionGroup(RDBConditionGroupOperator.OR);
            orCondition.NullCondition(COL_EED);
            orCondition.NotEqualsCondition(COL_EED).Column(COL_BED);

            return queryContext;
        }
        #endregion

        #region Public Methods
        public IEnumerable<SaleZoneRoutingProduct> GetSaleZoneRoutingProductsEffectiveAfter(int sellingNumberPlanId, DateTime effectiveOn)
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
            return queryContext.GetItems(SaleZoneRoutingProductMapper);

        }

        public void BuildInsertQuery(RDBInsertQuery insertQuery)
        {
            insertQuery.IntoTable(TABLE_NAME);
        }

        public void BuildUpdateQuery(RDBUpdateQuery updateQuery, long processInstanceID, string joinTableAlias, string columnName)
        {
            updateQuery.FromTable(TABLE_NAME);
            updateQuery.Column(COL_EED).Column(joinTableAlias, COL_EED);
            updateQuery.Where().EqualsCondition(joinTableAlias, columnName).Value(processInstanceID);
        }
        #endregion
    }
}
