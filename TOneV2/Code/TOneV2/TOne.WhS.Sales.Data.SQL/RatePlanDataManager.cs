using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.Sales.Data.SQL
{
    public class RatePlanDataManager : BaseSQLDataManager, IRatePlanDataManager
    {
        public RatePlanDataManager() : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString")) { }

        public bool InsertSalePriceList(SalePriceList salePriceList, out int salePriceListId)
        {
            object insertedId;

            int recordsAffected = ExecuteNonQuerySP("TOneWhS_Sales.sp_SalePriceList_Insert", out insertedId, salePriceList.OwnerType, salePriceList.OwnerId, salePriceList.CurrencyId);

            salePriceListId = (int)insertedId;

            return recordsAffected > 0;
        }

        public bool CloseThenInsertSaleRates(int customerId, List<SaleRate> newSaleRates)
        {
            DataTable newSaleRatesTable = BuildNewSaleRatesTable(newSaleRates);

            int recordsAffected = ExecuteNonQuerySPCmd("TOneWhS_Sales.sp_SaleRate_CloseAndInsert", (cmd) =>
            {
                cmd.Parameters.Add(new SqlParameter("@CustomerId", customerId));

                var tableParameter = new SqlParameter("@NewSaleRatesTable", SqlDbType.Structured);
                tableParameter.Value = newSaleRatesTable;
                cmd.Parameters.Add(tableParameter);
            });

            return recordsAffected >= newSaleRates.Count;
        }

        private DataTable BuildNewSaleRatesTable(List<SaleRate> newSaleRates)
        {
            DataTable table = new DataTable();

            table.Columns.Add("ID", typeof(long));
            table.Columns["ID"].AllowDBNull = true;

            table.Columns.Add("PriceListID", typeof(int));
            table.Columns.Add("ZoneID", typeof(long));
            table.Columns.Add("Rate", typeof(decimal));
            table.Columns.Add("BED", typeof(DateTime));
            table.Columns.Add("EED", typeof(DateTime));

            table.BeginLoadData();

            foreach (var saleRate in newSaleRates)
            {
                DataRow row = table.NewRow();

                if (saleRate.SaleRateId != 0)
                    row["ID"] = saleRate.SaleRateId;

                row["PriceListID"] = saleRate.PriceListId;
                row["ZoneID"] = saleRate.ZoneId;
                row["Rate"] = saleRate.NormalRate;
                row["BED"] = saleRate.BeginEffectiveDate;
                
                if (saleRate.EndEffectiveDate != null)
                    row["EED"] = saleRate.EndEffectiveDate;

                table.Rows.Add(row);
            }

            table.EndLoadData();

            return table;
        }
    }
}
