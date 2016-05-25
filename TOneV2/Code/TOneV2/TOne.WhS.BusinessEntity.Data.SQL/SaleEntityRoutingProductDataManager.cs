using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Data.SQL;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.BusinessEntity.Data.SQL
{
    public class SaleEntityRoutingProductDataManager : BaseTOneDataManager, ISaleEntityRoutingProductDataManager
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

        #endregion

        #region Private Methods
        #endregion
     
        #region Mappers

        private DefaultRoutingProduct DefaultRoutingProductMapper(IDataReader reader)
        {
            return new DefaultRoutingProduct()
            {
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
                RoutingProductId = GetReaderValue<Int32>(reader, "RoutingProductID"),
                OwnerType = GetReaderValue<SalePriceListOwnerType>(reader, "OwnerType"),
                OwnerId = (int)reader["OwnerID"],
                BED = GetReaderValue<DateTime>(reader, "BED"),
                EED = GetReaderValue<DateTime?>(reader, "EED"),
                SaleZoneId = GetReaderValue<long>(reader, "ZoneID")
            };

        }

        #endregion
    }
}
