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
        public IEnumerable<DefaultRoutingProduct> GetDefaultRoutingProducts(IEnumerable<Entities.RoutingCustomerInfo> customerInfos, DateTime? effectiveOn, bool isEffectiveInFuture)
        {
            DataTable dtActiveCustomers = CarrierAccountDataManager.BuildRoutingCustomerInfoTable(customerInfos);
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
        public IEnumerable<SaleZoneRoutingProduct> GetSaleZoneRoutingProducts(IEnumerable<Entities.RoutingCustomerInfo> customerInfos, DateTime? effectiveOn, bool isEffectiveInFuture)
        {
            DataTable dtActiveCustomers = CarrierAccountDataManager.BuildRoutingCustomerInfoTable(customerInfos);
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
