using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities.RatePlanning;
using Vanrise.Data.SQL;

namespace TOne.WhS.BusinessEntity.Data.SQL
{
    public class SaleRateDataManager : BaseSQLDataManager, ISaleRateDataManager
    {
        public SaleRateDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }

        public List<SaleRate> GetEffectiveSaleRates(SalePriceListOwnerType ownerType, int ownerId, DateTime effectiveOn)
        {
            return GetItemsSP("TOneWhS_BE.sp_SaleRate_GetByOwnerAndEffective", SaleRateMapper, ownerType, ownerId, effectiveOn);
        }

        public bool AreSaleRatesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("TOneWhS_BE.SaleRate", ref updateHandle);
        }

        public bool CloseRates(IEnumerable<RateChange> rateChanges)
        {
            DataTable rateChangesTable = BuildRateChangesTable(rateChanges);

            int affectedRows = ExecuteNonQuerySPCmd("TOneWhS_BE.sp_SaleRate_Close", (cmd) => {
                SqlParameter tableParameter = new SqlParameter("@RateChanges", SqlDbType.Structured);
                tableParameter.Value = rateChangesTable;
                cmd.Parameters.Add(tableParameter);
            });

            return affectedRows > 0;
        }

        private DataTable BuildRateChangesTable(IEnumerable<RateChange> rateChanges)
        {
            DataTable table = new DataTable();

            table.Columns.Add("RateID", typeof(long));
            table.Columns.Add("EED", typeof(DateTime));
            table.Columns["EED"].AllowDBNull = true;

            table.BeginLoadData();

            foreach (RateChange rateChange in rateChanges)
            {
                DataRow row = table.NewRow();
                row["RateID"] = rateChange.RateId;
                row["EED"] = rateChange.EED;
                table.Rows.Add(row);
            }

            table.EndLoadData();
            
            return table;
        }

        public bool InsertRates(IEnumerable<SaleRate> newRates)
        {
            DataTable newRatesTable = BuildNewRatesTable(newRates);

            int affectedRows = ExecuteNonQuerySPCmd("TOneWhS_BE.sp_SaleRate_InsertRates", (cmd) =>
            {
                SqlParameter tableParameter = new SqlParameter("@NewRates", SqlDbType.Structured);
                tableParameter.Value = newRatesTable;
                cmd.Parameters.Add(tableParameter);
            });

            return affectedRows > 0;
        }

        private DataTable BuildNewRatesTable(IEnumerable<SaleRate> newRates)
        {
            DataTable table = new DataTable();

            table.Columns.Add("ZoneId", typeof(long));
            table.Columns.Add("PriceListId", typeof(int));

            table.Columns.Add("CurrencyId", typeof(int));
            table.Columns["CurrencyId"].AllowDBNull = true;

            table.Columns.Add("NormalRate", typeof(decimal));
            table.Columns.Add("OtherRates", typeof(Dictionary<int, decimal>));
            table.Columns.Add("BeginEffectiveDate", typeof(DateTime));

            table.Columns.Add("EndEffectiveDate", typeof(DateTime));
            table.Columns["EndEffectiveDate"].AllowDBNull = true;

            table.BeginLoadData();

            foreach (SaleRate newRate in newRates)
            {
                DataRow row = table.NewRow();
                row["ZoneId"] = newRate.ZoneId;
                row["PriceListId"] = newRate.PriceListId;
                row["CurrencyId"] = newRate.CurrencyId;
                row["NormalRate"] = newRate.NormalRate;
                row["OtherRates"] = newRate.OtherRates;
                row["BeginEffectiveDate"] = newRate.BeginEffectiveDate;
                row["EndEffectiveDate"] = newRate.EndEffectiveDate;
                table.Rows.Add(row);
            }

            table.EndLoadData();

            return table;
        }

        #region Mappers

        private SaleRate SaleRateMapper(IDataReader reader)
        {
            SaleRate saleRate = new SaleRate();

            saleRate.SaleRateId = (long)reader["ID"];
            saleRate.ZoneId = (long)reader["ZoneID"];
            saleRate.PriceListId = (int)reader["PriceListID"];

            saleRate.NormalRate = (decimal)reader["Rate"];
            saleRate.OtherRates = reader["OtherRates"] as string != null ? Vanrise.Common.Serializer.Deserialize<Dictionary<int, decimal>>(reader["OtherRates"] as string) : null;

            saleRate.BeginEffectiveDate = (DateTime)reader["BED"];
            saleRate.EndEffectiveDate = GetReaderValue<DateTime?>(reader, "EED");

            return saleRate;
        }


        #endregion
    }
}
