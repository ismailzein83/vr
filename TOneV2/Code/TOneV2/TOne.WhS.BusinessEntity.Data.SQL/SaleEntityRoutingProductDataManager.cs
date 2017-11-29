using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.BusinessEntity.Data.SQL
{
    public class SaleEntityRoutingProductDataManager : Vanrise.Data.SQL.BaseSQLDataManager, ISaleEntityRoutingProductDataManager
    {
        #region ctor/Local Variables
        public SaleEntityRoutingProductDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }
        #endregion

        #region Public Methods

        public List<SaleZoneRoutingProduct> GetAllZoneRoutingProducts()
        {
            return GetItemsSP("[TOneWhS_BE].[sp_SaleEntityRoutingProduct_GetAllZoneRPs]", SaleZoneRoutingProductMapper);
        }
        public IEnumerable<SaleZoneRoutingProduct> GetExistingZoneRoutingProductsByZoneIds(SalePriceListOwnerType ownerType, int ownerId, IEnumerable<long> zoneIds, DateTime minEED)
        {
            string zoneIdsParameter = string.Join(",", zoneIds);
            return GetItemsSP("[TOneWhS_BE].[sp_SaleEntityRoutingProduct_GetExistingByZoneIDs]", SaleZoneRoutingProductMapper, ownerType, ownerId, zoneIdsParameter, minEED);
        }
        public List<DefaultRoutingProduct> GetAllDefaultRoutingProducts()
        {
            return GetItemsSP("[TOneWhS_BE].[sp_SaleEntityRoutingProduct_GetAllDefaultRPs]", DefaultRoutingProductMapper);
        }

        public IEnumerable<DefaultRoutingProduct> GetDefaultRoutingProducts(IEnumerable<int> customerIds, DateTime? effectiveOn, bool isEffectiveInFuture)
        {
            DataTable dtActiveCustomers = CarrierAccountDataManager.BuildRoutingCustomerInfoTable(customerIds);
            return GetItemsSPCmd("[TOneWhS_BE].[sp_SaleEntityRoutingProduct_GetFilteredByOwner]", DefaultRoutingProductMapper, (cmd) =>
            {
                var dtPrm = new SqlParameter("@ActiveCustomersInfo", SqlDbType.Structured);
                dtPrm.Value = dtActiveCustomers;
                cmd.Parameters.Add(dtPrm);
                cmd.Parameters.Add(new SqlParameter("@CustomerOwnerType", SalePriceListOwnerType.Customer));
                cmd.Parameters.Add(new SqlParameter("@EffectiveTime", effectiveOn));
                cmd.Parameters.Add(new SqlParameter("@IsFuture", isEffectiveInFuture));
                cmd.Parameters.Add(new SqlParameter("@IsDefault", true));
            });
        }

        public IEnumerable<DefaultRoutingProduct> GetEffectiveDefaultRoutingProducts(DateTime effectiveOn)
        {
            return GetItemsSP("TOneWhS_BE.sp_SaleEntityRoutingProduct_GetEffectiveDefaults", DefaultRoutingProductMapper, effectiveOn);
        }
        public IEnumerable<SaleZoneRoutingProduct> GetSaleZoneRoutingProducts(IEnumerable<int> customerIds, DateTime? effectiveOn, bool isEffectiveInFuture)
        {
            DataTable dtActiveCustomers = CarrierAccountDataManager.BuildRoutingCustomerInfoTable(customerIds);
            return GetItemsSPCmd("[TOneWhS_BE].[sp_SaleEntityRoutingProduct_GetFilteredByOwner]", SaleZoneRoutingProductMapper, (cmd) =>
            {
                var dtPrm = new SqlParameter("@ActiveCustomersInfo", SqlDbType.Structured);
                dtPrm.Value = dtActiveCustomers;
                cmd.Parameters.Add(dtPrm);
                cmd.Parameters.Add(new SqlParameter("@CustomerOwnerType", SalePriceListOwnerType.Customer));
                cmd.Parameters.Add(new SqlParameter("@EffectiveTime", effectiveOn));
                cmd.Parameters.Add(new SqlParameter("@IsFuture", isEffectiveInFuture));
                cmd.Parameters.Add(new SqlParameter("@IsDefault", false));


            });
        }
        public IEnumerable<SaleZoneRoutingProduct> GetEffectiveZoneRoutingProducts(SalePriceListOwnerType ownerType, int ownerId, DateTime effectiveOn)
        {
            return GetItemsSP("TOneWhS_BE.sp_SaleEntityRoutingProduct_GetEffectiveZoneRoutingProducts", SaleZoneRoutingProductMapper, ownerType, ownerId, effectiveOn);
        }
        public bool AreSaleEntityRoutingProductUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("TOneWhS_BE.SaleEntityRoutingProduct", ref updateHandle);
        }
        public IEnumerable<DefaultRoutingProduct> GetDefaultRoutingProductsEffectiveAfter(SalePriceListOwnerType ownerType, int ownerId, DateTime minimumDate)
        {
            return GetItemsSP("TOneWhS_BE.sp_SaleEntityRoutingProduct_GetDefaultRoutingProductsEffectiveAfter", DefaultRoutingProductMapper, ownerType, ownerId, minimumDate);
        }
        public IEnumerable<SaleZoneRoutingProduct> GetSaleZoneRoutingProductsEffectiveAfter(SalePriceListOwnerType ownerType, int ownerId, DateTime minimumDate)
        {
            return GetItemsSP("TOneWhS_BE.sp_SaleEntityRoutingProduct_GetSaleZoneRoutingProductsEffectiveAfter", SaleZoneRoutingProductMapper, ownerType, ownerId, minimumDate);
        }
        public IEnumerable<DefaultRoutingProduct> GetAllDefaultRoutingProductsByOwner(SalePriceListOwnerType ownerType, int ownerId)
        {
            return GetItemsSP("TOneWhS_BE.sp_SaleEntityRoutingProduct_GetAllDefaultRPsByOwner", DefaultRoutingProductMapper, ownerType, ownerId);
        }
        public IEnumerable<SaleZoneRoutingProduct> GetAllZoneRoutingProductsByOwner(SalePriceListOwnerType ownerType, int ownerId, IEnumerable<long> saleZoneIds)
        {
            if (saleZoneIds == null || saleZoneIds.Count() == 0)
                throw new Vanrise.Entities.MissingArgumentValidationException("saleZoneIds were not passed");
            string saleZoneIdsAsString = string.Join(",", saleZoneIds);
            return GetItemsSP("TOneWhS_BE.sp_SaleEntityRoutingProduct_GetAllZoneRPsByOwner", SaleZoneRoutingProductMapper, ownerType, ownerId, saleZoneIdsAsString);
        }
        public IEnumerable<DefaultRoutingProduct> GetAllDefaultRoutingProductsBySellingProductsAndCustomer(IEnumerable<int> sellingProductIds, int customerId)
        {
            if (sellingProductIds == null || sellingProductIds.Count() == 0)
                throw new Vanrise.Entities.MissingArgumentValidationException("sellingProductIds were not passed");
            string sellingProductIdsAsString = string.Join(",", sellingProductIds);
            return GetItemsSP("TOneWhS_BE.sp_SaleEntityRoutingProduct_GetAllDefaultRPsBySellingProductsAndCustomer", DefaultRoutingProductMapper, sellingProductIdsAsString, customerId);
        }
        public IEnumerable<SaleZoneRoutingProduct> GetAllZoneRoutingProductsBySellingProductsAndCustomer(IEnumerable<int> sellingProductIds, int customerId, IEnumerable<long> saleZoneIds)
        {
            if (sellingProductIds == null || sellingProductIds.Count() == 0)
                throw new Vanrise.Entities.MissingArgumentValidationException("sellingProductIds were not passed");
            if (saleZoneIds == null || saleZoneIds.Count() == 0)
                throw new Vanrise.Entities.MissingArgumentValidationException("saleZoneIds were not passed");

            string sellingProductIdsAsString = string.Join(",", sellingProductIds);
            string saleZoneIdsAsString = string.Join(",", saleZoneIds);
            return GetItemsSP("TOneWhS_BE.sp_SaleEntityRoutingProduct_GetAllZoneRPsBySellingProductsAndCustomer", SaleZoneRoutingProductMapper, sellingProductIdsAsString, customerId, saleZoneIdsAsString);
        }
        public IEnumerable<DefaultRoutingProduct> GetAllDefaultRoutingProducts(int sellingProductId, int customerId, IEnumerable<long> zoneIds)
        {
            return GetItemsSP("TOneWhS_BE.sp_SaleEntityRoutingProduct_GetAllDefaultRPsBySellingProductAndCustomer", DefaultRoutingProductMapper, sellingProductId, customerId, string.Join(",", zoneIds));
        }
        public IEnumerable<SaleZoneRoutingProduct> GetAllSaleZoneRoutingProducts(int sellingProductId, int customerId, IEnumerable<long> zoneIds)
        {
            return GetItemsSP("TOneWhS_BE.sp_SaleEntityRoutingProduct_GetAllZoneRPsBySellingProductAndCustomer", SaleZoneRoutingProductMapper, sellingProductId, customerId, string.Join(",", zoneIds));
        }
        #endregion

        #region Private Methods
        #endregion

        #region Mappers

        private DefaultRoutingProduct DefaultRoutingProductMapper(IDataReader reader)
        {
            return new DefaultRoutingProduct()
            {
                SaleEntityRoutingProductId = (long)reader["ID"],
                RoutingProductId = GetReaderValue<Int32>(reader, "RoutingProductID"),
                OwnerType = GetReaderValue<SalePriceListOwnerType>(reader, "OwnerType"),
                OwnerId = (int)reader["OwnerID"],
                BED = GetReaderValue<DateTime>(reader, "BED"),
                EED = GetReaderValue<DateTime?>(reader, "EED")
            };
        }

        private SaleZoneRoutingProduct SaleZoneRoutingProductMapper(IDataReader reader)
        {
            return new SaleZoneRoutingProduct()
            {
                SaleEntityRoutingProductId = (long)reader["ID"],
                RoutingProductId = GetReaderValue<Int32>(reader, "RoutingProductID"),
                OwnerType = GetReaderValue<SalePriceListOwnerType>(reader, "OwnerType"),
                OwnerId = (int)reader["OwnerID"],
                BED = GetReaderValue<DateTime>(reader, "BED"),
                EED = GetReaderValue<DateTime?>(reader, "EED"),
                SaleZoneId = GetReaderValue<long>(reader, "ZoneID")
            };
        }

        #endregion

        #region State Backup Methods

        public string BackupAllSaleEntityRoutingProductDataBySellingNumberPlanId(long stateBackupId, string backupDatabase, int sellingNumberPlanId)
        {
            return String.Format(@"INSERT INTO [{0}].[TOneWhS_BE_Bkup].[SaleEntityRoutingProduct] WITH (TABLOCK)
                                            SELECT erp.[ID], erp.[OwnerType], erp.[OwnerID], erp.[ZoneID], erp.[RoutingProductID], erp.[BED], erp.[EED], {1} AS StateBackupID  FROM [TOneWhS_BE].[SaleEntityRoutingProduct]
                                            erp WITH (NOLOCK) Inner Join [TOneWhS_BE].SaleZone sz WITH (NOLOCK) on erp.ZoneID = sz.ID
                                            Where sz.SellingNumberPlanID = {2}", backupDatabase, stateBackupId, sellingNumberPlanId);
        }


        public string BackupAllSaleEntityRoutingProductDataByByOwner(long stateBackupId, string backupDatabase, int ownerId, int ownerType)
        {
            return String.Format(@"INSERT INTO [{0}].[TOneWhS_BE_Bkup].[SaleEntityRoutingProduct] WITH (TABLOCK)
                                           SELECT [ID], [OwnerType], [OwnerID], [ZoneID], [RoutingProductID], [BED], [EED], {1} AS StateBackupID  FROM [TOneWhS_BE].[SaleEntityRoutingProduct] WITH (NOLOCK)
                                           Where OwnerId = {2} and OwnerType = {3}", backupDatabase, stateBackupId, ownerId, ownerType);
        }

        public string GetRestoreCommands(long stateBackupId, string backupDatabase)
        {
            return String.Format(@"INSERT INTO [TOneWhS_BE].[SaleEntityRoutingProduct] ([ID], [OwnerType], [OwnerID], [ZoneID], [RoutingProductID], [BED], [EED])
                                           SELECT [ID], [OwnerType], [OwnerID], [ZoneID], [RoutingProductID], [BED], [EED] FROM [{0}].[TOneWhS_BE_Bkup].[SaleEntityRoutingProduct]
                                            WITH (NOLOCK) Where StateBackupID = {1} ", backupDatabase, stateBackupId);
        }

        public string GetDeleteCommandsBySellingNumberPlanId(long sellingNumberPlanId)
        {
            return String.Format(@"DELETE erp FROM [TOneWhS_BE].[SaleEntityRoutingProduct]
                                            erp Inner Join [TOneWhS_BE].SaleZone sz on erp.ZoneID = sz.ID
                                            Where sz.SellingNumberPlanID = {0}", sellingNumberPlanId);
        }


        public string GetDeleteCommandsByOwner(int ownerId, int ownerType)
        {
            return String.Format(@"DELETE FROM [TOneWhS_BE].[SaleEntityRoutingProduct] 
                                           Where OwnerId = {0} and OwnerType = {1}", ownerId, ownerType);
        }

        #endregion
    }
}
