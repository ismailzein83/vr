using System;
using System.Linq;
using Vanrise.Common;
using Vanrise.Data.RDB;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data.RDB
{
    public class SaleEntityServiceDataManager : ISaleEntityServiceDataManager
    {
        #region RDB

        static string TABLE_ALIAS = "ses";
        static string TABLE_NAME = "TOneWhS_BE_SaleEntityService";
        const string COL_ID = "ID";
        const string COL_PriceListID = "PriceListID";
        const string COL_ZoneID = "ZoneID";
        const string COL_Services = "Services";
        const string COL_BED = "BED";
        const string COL_EED = "EED";
        const string COL_SourceID = "SourceID";
        const string COL_LastModifiedTime = "LastModifiedTime";


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
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "TOneWhS_BE",
                DBTableName = "SaleEntityService",
                Columns = columns,
                IdColumnName = COL_ID,
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
            selectQuery.From(TABLE_NAME, TABLE_ALIAS);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var whereContext = selectQuery.Where();
            whereContext.NullCondition(COL_ZoneID);

            if (effectiveOn.HasValue)
                BEDataUtility.SetEffectiveAfterDateCondition(whereContext, TABLE_ALIAS, COL_BED, COL_EED, effectiveOn.Value);
            else
                whereContext.FalseCondition();//effectiveOn should be required

            return queryContext.GetItems(SaleEntityDefaultServiceMapper);
        }

        public IEnumerable<SaleEntityZoneService> GetEffectiveSaleEntityZoneServices(SalePriceListOwnerType ownerType, int ownerId, DateTime? effectiveOn)
        {
            var salePriceListDataManager = new SalePriceListDataManager();
            var queryContext = new RDBQueryContext(GetDataProvider());

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var joinQuery = selectQuery.Join();
            salePriceListDataManager.JoinSalePriceList(joinQuery, "p", TABLE_ALIAS, COL_PriceListID);

            var whereQuery = selectQuery.Where();
            whereQuery.NotNullCondition(COL_ZoneID);
            whereQuery.EqualsCondition("p", SalePriceListDataManager.COL_OwnerType).Value((int)ownerType);
            whereQuery.EqualsCondition("p", SalePriceListDataManager.COL_OwnerID).Value(ownerId);

            if (effectiveOn.HasValue)
                BEDataUtility.SetEffectiveAfterDateCondition(whereQuery, TABLE_ALIAS, COL_BED, COL_EED, effectiveOn.Value);
            else
                whereQuery.FalseCondition();//effectiveOn should be required

            return queryContext.GetItems(SaleEntityZoneServiceMapper);
        }

        public IEnumerable<SaleEntityZoneService> GetEffectiveSaleEntityZoneServicesByOwner(IEnumerable<RoutingCustomerInfoDetails> customerInfos, DateTime? effectiveOn, bool isEffectiveInFuture)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<SaleEntityZoneService> GetSaleZonesServicesEffectiveAfter(int sellingNumberPlanId, DateTime effectiveOn)
        {
            SaleZoneDataManager saleZoneDataManager = new SaleZoneDataManager();
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var joinContext = selectQuery.Join();
            saleZoneDataManager.JoinSaleZone(joinContext, "sz", TABLE_ALIAS, COL_ZoneID);

            var whereQuery = selectQuery.Where();
            whereQuery.EqualsCondition("z", SaleZoneDataManager.COL_SellingNumberPlanID).Value(sellingNumberPlanId);

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
            throw new NotImplementedException();
        }

        public IEnumerable<SaleEntityZoneService> GetZoneServicesEffectiveAfter(SalePriceListOwnerType ownerType, int ownerId, DateTime minimumDate)
        {
            throw new NotImplementedException();
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
    }
}
