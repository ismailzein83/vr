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
        public SaleEntityRoutingProductDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }

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

        public bool InsertOrUpdateDefaultRoutingProduct(SalePriceListOwnerType ownerType, int ownerId, NewDefaultRoutingProduct newDefaultRoutingProduct)
        {
            int affectedRows = ExecuteNonQuerySP("TOneWhS_BE.sp_SaleEntityRoutingProduct_InsertOrUpdateDefault", ownerType, ownerId, newDefaultRoutingProduct.DefaultRoutingProductId, newDefaultRoutingProduct.BED, newDefaultRoutingProduct.EED);

            return affectedRows > 0;
        }

        public bool UpdateDefaultRoutingProduct(SalePriceListOwnerType ownerType, int ownerId, DefaultRoutingProductChange defaultRoutingProductChange)
        {
            int affectedRows = ExecuteNonQuerySP("TOneWhS_BE.sp_SaleEntityRoutingProduct_UpdateDefault", ownerType, ownerId, defaultRoutingProductChange.EED);
            return affectedRows > 0;
        }

        public bool InsertZoneRoutingProducts(SalePriceListOwnerType ownerType, int ownerId, IEnumerable<NewZoneRoutingProduct> newZoneRoutingProducts)
        {
            DataTable newZoneRoutingProductsTable = BuildNewZoneRoutingProductsTable(newZoneRoutingProducts);

            int affectedRows = ExecuteNonQuerySPCmd("TOneWhs_BE.sp_SaleEntityRoutingProduct_InsertZoneRoutingProducts", (cmd) =>
            {
                cmd.Parameters.Add(new SqlParameter("@OwnerType", ownerType));
                cmd.Parameters.Add(new SqlParameter("@OwnerID", ownerId));
                
                var tableParameter = new SqlParameter("@NewZoneRoutingProducts", SqlDbType.Structured);
                tableParameter.Value = newZoneRoutingProductsTable;
                cmd.Parameters.Add(tableParameter);
            });

            return affectedRows == newZoneRoutingProducts.Count();
        }

        private DataTable BuildNewZoneRoutingProductsTable(IEnumerable<NewZoneRoutingProduct> newZoneRoutingProducts)
        {
            DataTable table = new DataTable();

            table.Columns.Add("ZoneID", typeof(long));
            table.Columns.Add("RoutingProductID", typeof(int));
            table.Columns.Add("BED", typeof(DateTime));
            table.Columns.Add("EED", typeof(DateTime));

            table.BeginLoadData();

            foreach (var product in newZoneRoutingProducts)
            {
                DataRow row = table.NewRow();

                row["ZoneID"] = product.ZoneId;
                row["RoutingProductID"] = product.ZoneRoutingProductId;
                row["BED"] = product.BED;

                if (product.EED != null)
                    row["EED"] = product.EED;

                table.Rows.Add(row);
            }

            table.EndLoadData();

            return table;
        }

        public bool UpdateZoneRoutingProducts(SalePriceListOwnerType ownerType, int ownerId, IEnumerable<ZoneRoutingProductChange> zoneRoutingProductChanges)
        {
            DataTable zoneRoutingProductChangesTable = BuildZoneRoutingProductChangesTable(zoneRoutingProductChanges);

            int affectedRows = ExecuteNonQuerySPCmd("TOneWhS_BE.sp_SaleEntityRoutingProduct_UpdateZoneRoutingProducts", (cmd) => {
                cmd.Parameters.Add(new SqlParameter("@OwnerType", ownerType));
                cmd.Parameters.Add(new SqlParameter("@OwnerID", ownerId));
                
                var tableParameter = new SqlParameter("@ZoneRoutingProductChanges", SqlDbType.Structured);
                tableParameter.Value = zoneRoutingProductChangesTable;
                cmd.Parameters.Add(tableParameter);
            });

            return affectedRows == zoneRoutingProductChanges.Count();
        }

        private DataTable BuildZoneRoutingProductChangesTable(IEnumerable<ZoneRoutingProductChange> zoneRoutingProductChanges)
        {
            DataTable table = new DataTable();

            table.Columns.Add("ZoneID", typeof(long));
            table.Columns.Add("RoutingProductID", typeof(int));
            table.Columns.Add("EED", typeof(DateTime));

            table.BeginLoadData();

            foreach (var change in zoneRoutingProductChanges)
            {
                DataRow row = table.NewRow();

                row["ZoneID"] = change.ZoneId;
                row["RoutingProductID"] = change.ZoneRoutingProductId;
                
                if (change.EED != null)
                    row["EED"] = change.EED;

                table.Rows.Add(row);
            }

            table.EndLoadData();

            return table;
        }

        public bool AreSaleEntityRoutingProductUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("TOneWhS_BE.SaleEntityRoutingProduct", ref updateHandle);
        }

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
