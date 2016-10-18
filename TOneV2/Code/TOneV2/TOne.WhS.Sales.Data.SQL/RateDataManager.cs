using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Data.SQL;
using TOne.WhS.Sales.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.Sales.Data.SQL
{
    public class RateDataManager : BaseTOneDataManager, IRateDataManager
    {

        #region Ctor/Local Variables

        public RateDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }

        #endregion

        #region Public Methods

        public bool CloseRates(IEnumerable<Entities.DraftRateToClose> rateChanges)
        {
            DataTable rateChangesTable = BuildRateChangesTable(rateChanges);

            int affectedRows = ExecuteNonQuerySPCmd("TOneWhS_BE.sp_SaleRate_CloseRates", (cmd) =>
            {
                SqlParameter tableParameter = new SqlParameter("@RateChanges", SqlDbType.Structured);
                tableParameter.Value = rateChangesTable;
                cmd.Parameters.Add(tableParameter);
            });

            return affectedRows > 0;
        }

        public bool InsertRates(IEnumerable<Entities.DraftRateToChange> newRates, int priceListId)
        {
            DataTable newRatesTable = BuildNewRatesTable(newRates, priceListId);

            int affectedRows = ExecuteNonQuerySPCmd("TOneWhS_BE.sp_SaleRate_InsertRates", (cmd) =>
            {
                SqlParameter tableParameter = new SqlParameter("@NewRates", SqlDbType.Structured);
                tableParameter.Value = newRatesTable;
                cmd.Parameters.Add(tableParameter);
            });

            return affectedRows > 0;
        }

        #endregion

        #region Private Methods

        DataTable BuildNewRatesTable(IEnumerable<DraftRateToChange> newRates, int priceListId)
        {
            DataTable table = new DataTable();

            table.Columns.Add("ZoneID", typeof(long));
            table.Columns.Add("PriceListID", typeof(int));
            table.Columns.Add("CurrencyID", typeof(int));
            table.Columns.Add("NormalRate", typeof(decimal));
            table.Columns.Add("BED", typeof(DateTime));
            table.Columns.Add("EED", typeof(DateTime));

            table.BeginLoadData();

            foreach (DraftRateToChange newRate in newRates)
            {
                DataRow row = table.NewRow();
                row["ZoneID"] = newRate.ZoneId;
                row["PriceListID"] = priceListId;
                if (newRate.CurrencyId != null)
                    row["CurrencyID"] = newRate.CurrencyId;
                row["NormalRate"] = newRate.Rate;
                row["BED"] = newRate.BED;
                if (newRate.EED != null)
                    row["EED"] = newRate.EED;
                table.Rows.Add(row);
            }

            table.EndLoadData();

            return table;
        }

        DataTable BuildRateChangesTable(IEnumerable<DraftRateToClose> rateChanges)
        {
            DataTable table = new DataTable();

            table.Columns.Add("RateID", typeof(long));
            table.Columns.Add("EED", typeof(DateTime));

            table.BeginLoadData();

            foreach (DraftRateToClose rateChange in rateChanges)
            {
                DataRow row = table.NewRow();
                row["RateID"] = rateChange.RateId;
                if (rateChange.EED != null)
                    row["EED"] = rateChange.EED;
                table.Rows.Add(row);
            }

            table.EndLoadData();

            return table;
        }
        
        #endregion
    }
}
