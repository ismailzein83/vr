using System;
using System.Linq;
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
        const string COL_ID = "ID";
        const string COL_OwnerType = "OwnerType";
        const string COL_OwnerID = "OwnerID";
        const string COL_ZoneID = "ZoneID";
        const string COL_RoutingProductID = "RoutingProductID";
        const string COL_BED = "BED";
        const string COL_EED = "EED";
        const string COL_LastModifiedTime = "LastModifiedTime";


        static SaleEntityRoutingProductDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>
            {
                {COL_ID, new RDBTableColumnDefinition {DataType = RDBDataType.BigInt}},
                {COL_OwnerType, new RDBTableColumnDefinition {DataType = RDBDataType.Int}},
                {COL_OwnerID, new RDBTableColumnDefinition {DataType = RDBDataType.Int}},
                {COL_ZoneID, new RDBTableColumnDefinition {DataType = RDBDataType.BigInt}},
                {COL_RoutingProductID, new RDBTableColumnDefinition {DataType = RDBDataType.Int}},
                {COL_BED, new RDBTableColumnDefinition {DataType = RDBDataType.DateTime}},
                {COL_EED, new RDBTableColumnDefinition {DataType = RDBDataType.DateTime}},
                {COL_LastModifiedTime, new RDBTableColumnDefinition {DataType = RDBDataType.DateTime}}
            };
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "TOneWhS_BE",
                DBTableName = "SaleEntityRoutingProduct",
                Columns = columns,
                IdColumnName = COL_ID,
                ModifiedTimeColumnName = COL_LastModifiedTime

            });
        }
        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("TOneWhS_BE", "TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString");
        }
        #endregion ISaleEntityRoutingProductDataManager

        #region Members

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
            RDBQueryContext queryContext = GetRoutingProducts(customerIds, effectiveOn, isEffectiveInFuture, true);
            return queryContext.GetItems(DefaultRoutingProductMapper);
        }

        public IEnumerable<DefaultRoutingProduct> GetEffectiveDefaultRoutingProducts(DateTime effectiveOn)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var whereContext = selectQuery.Where();
            whereContext.NullCondition(COL_ZoneID);

            whereContext.LessOrEqualCondition(COL_BED).Value(effectiveOn);
            var orCondition = whereContext.ChildConditionGroup(RDBConditionGroupOperator.OR);
            orCondition.NullCondition(COL_EED);
            orCondition.GreaterThanCondition(COL_EED).Value(effectiveOn);

            return queryContext.GetItems(DefaultRoutingProductMapper);
        }

        public IEnumerable<SaleZoneRoutingProduct> GetSaleZoneRoutingProducts(IEnumerable<int> customerIds, DateTime? effectiveOn, bool isEffectiveInFuture)
        {
            RDBQueryContext queryContext = GetRoutingProducts(customerIds, effectiveOn, isEffectiveInFuture, false);
            return queryContext.GetItems(SaleZoneRoutingProductMapper);
        }

        public IEnumerable<SaleZoneRoutingProduct> GetEffectiveZoneRoutingProducts(SalePriceListOwnerType ownerType, int ownerId, DateTime effectiveOn)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var whereContext = selectQuery.Where();
            whereContext.EqualsCondition(COL_OwnerType).Value((int)ownerType);
            whereContext.EqualsCondition(COL_OwnerID).Value(ownerId);
            whereContext.ConditionIfColumnNotNull(COL_ZoneID);

            whereContext.LessOrEqualCondition(COL_BED).Value(effectiveOn);
            var orCondition = whereContext.ChildConditionGroup(RDBConditionGroupOperator.OR);
            orCondition.NullCondition(COL_EED);
            orCondition.GreaterThanCondition(COL_EED).Value(effectiveOn);

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
            selectQuery.From(TABLE_NAME, TABLE_ALIAS);
            selectQuery.SelectColumns().Columns(COL_ID, COL_OwnerType, COL_OwnerID, COL_RoutingProductID, COL_BED, COL_EED);

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
            selectQuery.From(TABLE_NAME, TABLE_ALIAS);
            selectQuery.SelectColumns().Columns(COL_ID, COL_OwnerType, COL_OwnerID, COL_RoutingProductID, COL_BED, COL_EED);

            var whereContext = selectQuery.Where();
            whereContext.EqualsCondition(COL_OwnerType).Value((int)ownerType);
            whereContext.EqualsCondition(COL_OwnerID).Value(ownerId);
            whereContext.ConditionIfColumnNotNull(COL_ZoneID);

            var orCondition = whereContext.ChildConditionGroup(RDBConditionGroupOperator.OR);
            orCondition.NullCondition(COL_EED);
            var andCondition = orCondition.ChildConditionGroup();
            andCondition.GreaterThanCondition(COL_EED).Value(minimumDate);
            andCondition.NotEqualsCondition(COL_BED).Column(COL_EED);

            return queryContext.GetItems(SaleZoneRoutingProductMapper);
        }

        public IEnumerable<DefaultRoutingProduct> GetAllDefaultRoutingProductsByOwner(SalePriceListOwnerType ownerType, int ownerId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var whereContext = selectQuery.Where();
            whereContext.NullCondition(COL_ZoneID);
            whereContext.EqualsCondition(COL_OwnerID).Value(ownerId);
            whereContext.EqualsCondition(COL_OwnerType).Value((int)ownerType);

            return queryContext.GetItems(DefaultRoutingProductMapper);
        }

        public IEnumerable<SaleZoneRoutingProduct> GetAllZoneRoutingProductsByOwner(SalePriceListOwnerType ownerType, int ownerId, IEnumerable<long> saleZoneIds)
        {
            if (saleZoneIds == null || !saleZoneIds.Any())
                throw new Vanrise.Entities.MissingArgumentValidationException("saleZoneIds were not passed");

            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS);
            selectQuery.SelectColumns().Columns(COL_ID, COL_OwnerType, COL_OwnerID, COL_ZoneID, COL_RoutingProductID, COL_BED, COL_EED);

            var whereContext = selectQuery.Where();
            whereContext.ConditionIfColumnNotNull(COL_ZoneID);
            whereContext.ListCondition(COL_ZoneID, RDBListConditionOperator.IN, saleZoneIds);
            whereContext.EqualsCondition(COL_OwnerType).Value((int)ownerType);
            whereContext.EqualsCondition(COL_OwnerID).Value(ownerId);

            return queryContext.GetItems(SaleZoneRoutingProductMapper);
        }

        public IEnumerable<DefaultRoutingProduct> GetAllDefaultRoutingProductsBySellingProductsAndCustomer(IEnumerable<int> sellingProductIds, int customerId)
        {
            return null;
            //if (sellingProductIds == null || !sellingProductIds.Any())
            //    throw new Vanrise.Entities.MissingArgumentValidationException("sellingProductIds were not passed");

            //var queryContext = new RDBQueryContext(GetDataProvider());
            //var selectQuery = queryContext.AddSelectQuery();
            //selectQuery.From(TABLE_NAME, TABLE_ALIAS);
            //selectQuery.SelectColumns().Columns(COL_ID, COL_OwnerType, COL_OwnerID, COL_RoutingProductID, COL_BED, COL_EED);

            //var whereContext = selectQuery.Where();
            //whereContext.NullCondition(COL_ZoneID);

            //var orCondition = whereContext.ChildConditionGroup(RDBConditionGroupOperator.OR);

            //var firstAndCondition = 

        }

        public IEnumerable<SaleZoneRoutingProduct> GetAllZoneRoutingProductsBySellingProductsAndCustomer(IEnumerable<int> sellingProductIds, int customerId,
            IEnumerable<long> saleZoneIds)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<SaleZoneRoutingProduct> GetExistingZoneRoutingProductsByZoneIds(SalePriceListOwnerType ownerType, int ownerId, IEnumerable<long> zoneIds, DateTime minEED)
        {
            return null;
        }

        public IEnumerable<DefaultRoutingProduct> GetAllDefaultRoutingProducts(int sellingProductId, int customerId, IEnumerable<long> zoneIds)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<SaleZoneRoutingProduct> GetAllSaleZoneRoutingProducts(int sellingProductId, int customerId, IEnumerable<long> zoneIds)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DefaultRoutingProduct> GetAllDefaultRoutingProductsByOwners(IEnumerable<int> sellingProductIds, IEnumerable<int> customerIds)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<SaleZoneRoutingProduct> GetAllZoneRoutingProductsByOwners(IEnumerable<int> sellingProductIds, IEnumerable<int> customerIds,
            IEnumerable<long> zoneIds)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DefaultRoutingProduct> GetAllDefaultRoutingProductsByOwners(SalePriceListOwnerType ownerType, IEnumerable<int> ownerIds)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<SaleZoneRoutingProduct> GetAllZoneRoutingProductsByOwners(SalePriceListOwnerType ownerType, IEnumerable<int> ownerIds,
            IEnumerable<long> zoneIds)
        {
            throw new NotImplementedException();
        }

        public bool Update(ZoneRoutingProductToEdit zoneRoutingProductToEdit, long reservedId, List<ZoneRoutingProductToChange> routingProductToChange)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Mappers

        private DefaultRoutingProduct DefaultRoutingProductMapper(IRDBDataReader reader)
        {
            return new DefaultRoutingProduct
            {
                SaleEntityRoutingProductId = reader.GetInt("ID"),
                RoutingProductId = reader.GetInt("RoutingProductID"),
                OwnerType = (SalePriceListOwnerType)reader.GetInt("OwnerType"),
                OwnerId = reader.GetInt("OwnerID"),
                BED = reader.GetDateTime("BED"),
                EED = reader.GetNullableDateTime("EED")
            };
        }

        private SaleZoneRoutingProduct SaleZoneRoutingProductMapper(IRDBDataReader reader)
        {
            return new SaleZoneRoutingProduct
            {
                SaleEntityRoutingProductId = reader.GetInt("ID"),
                RoutingProductId = reader.GetInt("RoutingProductID"),
                OwnerType = (SalePriceListOwnerType)reader.GetInt("OwnerType"),
                OwnerId = reader.GetInt("OwnerID"),
                BED = reader.GetDateTime("BED"),
                EED = reader.GetNullableDateTime("EED"),
                SaleZoneId = reader.GetLongWithNullHandling("ZoneID")
            };
        }

        #endregion

        #region private Methods

        private RDBQueryContext GetRoutingProducts(IEnumerable<int> customerIds, DateTime? effectiveOn, bool isEffectiveInFuture, bool isDefault)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            if (customerIds != null && customerIds.Any())
            {
                var selectQuery = queryContext.AddSelectQuery();
                selectQuery.From(TABLE_NAME, TABLE_ALIAS);
                selectQuery.SelectColumns().Columns(COL_ID, COL_OwnerType, COL_OwnerID, COL_ZoneID, COL_RoutingProductID, COL_BED, COL_EED);

                var whereContext = selectQuery.Where();
                whereContext.ListCondition(COL_OwnerID, RDBListConditionOperator.IN, customerIds);

                SetRoutingProductWhereContext(whereContext, effectiveOn, isEffectiveInFuture, isDefault, (int)SalePriceListOwnerType.Customer);
            }

            var secondSelectQuery = queryContext.AddSelectQuery();
            secondSelectQuery.From(TABLE_NAME, TABLE_ALIAS);
            secondSelectQuery.SelectColumns().Columns(COL_ID, COL_OwnerType, COL_OwnerID, COL_ZoneID, COL_RoutingProductID, COL_BED, COL_EED);
            var secondWhereContext = secondSelectQuery.Where();
            SetRoutingProductWhereContext(secondWhereContext, effectiveOn, isEffectiveInFuture, isDefault, (int)SalePriceListOwnerType.SellingProduct);

            return queryContext;
        }

        private void SetRoutingProductWhereContext(RDBConditionContext conditionContext, DateTime? effectiveOn, bool isEffectiveInFuture, bool isDefault, int ownerType)
        {
            var orCondition = conditionContext.ChildConditionGroup(RDBConditionGroupOperator.OR);
            if (isEffectiveInFuture)
            {
                orCondition.ConditionIfColumnNotNull(COL_EED).GreaterThanCondition(COL_BED).DateNow();
            }
            else if (effectiveOn.HasValue)
            {
                var andCondition = orCondition.ChildConditionGroup();
                andCondition.ConditionIfColumnNotNull(COL_BED).LessOrEqualCondition(COL_BED).Value(effectiveOn.Value);
                andCondition.ConditionIfColumnNotNull(COL_EED).GreaterThanCondition(COL_EED).Value(effectiveOn.Value);
            }

            conditionContext.EqualsCondition(COL_OwnerType).Value(ownerType);
            if (isDefault)
                conditionContext.NullCondition(COL_ZoneID);
            else
                conditionContext.ConditionIfColumnNotNull(COL_ZoneID);
        }

        private RDBQueryContext GetAllRoutingProducts(bool isDefault)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var whereContext = selectQuery.Where();

            if (isDefault)
                whereContext.NullCondition(COL_ZoneID);
            else
                whereContext.ConditionIfColumnNotNull(COL_ZoneID);

            var orCondition = whereContext.ChildConditionGroup(RDBConditionGroupOperator.OR);
            orCondition.NullCondition(COL_EED);
            orCondition.NotEqualsCondition(COL_EED).Column(COL_BED);

            return queryContext;
        }
        #endregion
    }
}
