using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Data.SQL;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.Data.SQL
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

        public bool InsertDefaultRoutingProduct(BusinessEntity.Entities.SalePriceListOwnerType ownerType, int ownerId, Entities.DraftNewDefaultRoutingProduct newDefaultRoutingProduct)
        {
            int affectedRows = ExecuteNonQuerySP("TOneWhS_BE.sp_SaleEntityRoutingProduct_InsertOrUpdateDefault", ownerType, ownerId, newDefaultRoutingProduct.DefaultRoutingProductId, newDefaultRoutingProduct.BED, newDefaultRoutingProduct.EED);
            return affectedRows > 0;
        }

        public bool UpdateDefaultRoutingProduct(BusinessEntity.Entities.SalePriceListOwnerType ownerType, int ownerId, Entities.DraftChangedDefaultRoutingProduct defaultRoutingProductChange)
        {
            int affectedRows = ExecuteNonQuerySP("TOneWhS_BE.sp_SaleEntityRoutingProduct_UpdateDefault", ownerType, ownerId, defaultRoutingProductChange.EED);
            return affectedRows > 0;
        }

        public bool InsertZoneRoutingProducts(BusinessEntity.Entities.SalePriceListOwnerType ownerType, int ownerId, IEnumerable<Entities.DraftNewZoneRoutingProduct> newZoneRoutingProducts)
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

        public bool UpdateZoneRoutingProducts(BusinessEntity.Entities.SalePriceListOwnerType ownerType, int ownerId, IEnumerable<Entities.DraftChangedZoneRoutingProduct> zoneRoutingProductChanges)
        {
            DataTable zoneRoutingProductChangesTable = BuildZoneRoutingProductChangesTable(zoneRoutingProductChanges);

            int affectedRows = ExecuteNonQuerySPCmd("TOneWhS_BE.sp_SaleEntityRoutingProduct_UpdateZoneRoutingProducts", (cmd) =>
            {
                cmd.Parameters.Add(new SqlParameter("@OwnerType", ownerType));
                cmd.Parameters.Add(new SqlParameter("@OwnerID", ownerId));

                var tableParameter = new SqlParameter("@ZoneRoutingProductChanges", SqlDbType.Structured);
                tableParameter.Value = zoneRoutingProductChangesTable;
                cmd.Parameters.Add(tableParameter);
            });

            return affectedRows == zoneRoutingProductChanges.Count();
        }

        #endregion

        #region Private Methods

        DataTable BuildNewZoneRoutingProductsTable(IEnumerable<DraftNewZoneRoutingProduct> newZoneRoutingProducts)
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

        DataTable BuildZoneRoutingProductChangesTable(IEnumerable<DraftChangedZoneRoutingProduct> zoneRoutingProductChanges)
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

        #endregion
    }
}
