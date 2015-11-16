using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Data.SQL;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data.SQL
{
    public class SaleEntityRoutingProductDataManager : BaseTOneDataManager, ISaleEntityRoutingProductDataManager
    {
        public IEnumerable<DefaultRoutingProduct> GetDefaultRoutingProducts(DateTime? effectiveOn, bool isEffectiveInFuture, IEnumerable<Entities.RoutingCustomerInfo> customerInfos)
        {
            DataTable dtActiveCustomers = CarrierAccountDataManager.BuildRoutingCustomerInfoTable(customerInfos);
            return GetItemsSPCmd("[TOneWhS_BE].[sp_SaleEntityRoutingProduct_GetDefaultRoutingProducts]", DefaultRoutingProductMapper, (cmd) =>
            {
                var dtPrm = new SqlParameter("@ActiveCustomersInfo", SqlDbType.Structured);
                dtPrm.Value = dtActiveCustomers;
                cmd.Parameters.Add(dtPrm);

                cmd.Parameters.Add(new SqlParameter("@EffectiveTime", effectiveOn));
                cmd.Parameters.Add(new SqlParameter("@IsFuture", isEffectiveInFuture));
            });
        }

        public IEnumerable<SaleZoneRoutingProduct> GetSaleZoneRoutingProducts(DateTime? effectiveOn, bool isEffectiveInFuture, IEnumerable<Entities.RoutingCustomerInfo> customerInfos)
        {
            DataTable dtActiveCustomers = CarrierAccountDataManager.BuildRoutingCustomerInfoTable(customerInfos);
            return GetItemsSPCmd("[TOneWhS_BE].[sp_SaleEntityRoutingProduct_GetSaleZonesRoutingProducts]", SaleZoneRoutingProductMapper, (cmd) =>
            {
                var dtPrm = new SqlParameter("@ActiveCustomersInfo", SqlDbType.Structured);
                dtPrm.Value = dtActiveCustomers;
                cmd.Parameters.Add(dtPrm);

                cmd.Parameters.Add(new SqlParameter("@EffectiveTime", effectiveOn));
                cmd.Parameters.Add(new SqlParameter("@IsFuture", isEffectiveInFuture));
            });
        }

        DefaultRoutingProduct DefaultRoutingProductMapper(IDataReader reader)
        {
            return new DefaultRoutingProduct()
            {
                RoutingProductId = GetReaderValue<Int32>(reader, "RoutingProductID"),
                OwnerType = (SalePriceListOwnerType)GetReaderValue<int>(reader, "OwnerType"),
                OwnerId = (int)reader["OwnerID"],
                BED = GetReaderValue<DateTime>(reader, "BED"),
                EED = GetReaderValue<DateTime?>(reader, "EED")
            };
        }

        SaleZoneRoutingProduct SaleZoneRoutingProductMapper(IDataReader reader)
        {
            return new SaleZoneRoutingProduct()
            {
                RoutingProductId = GetReaderValue<Int32>(reader, "RoutingProductID"),
                OwnerType = (SalePriceListOwnerType)GetReaderValue<int>(reader, "OwnerType"),
                OwnerId = (int)reader["OwnerID"],
                BED = GetReaderValue<DateTime>(reader, "BED"),
                EED = GetReaderValue<DateTime?>(reader, "EED"),
                SaleZoneId = (int)reader["ZoneId"]
            };

        }
    }
}
