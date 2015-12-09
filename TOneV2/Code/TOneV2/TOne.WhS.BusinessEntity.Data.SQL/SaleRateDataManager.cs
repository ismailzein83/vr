using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.BusinessEntity.Data.SQL
{
    public class SaleRateDataManager : BaseSQLDataManager, ISaleRateDataManager
    {
        public SaleRateDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }

        public Vanrise.Entities.BigResult<Entities.SaleRate> GetSaleRateFilteredFromTemp(Vanrise.Entities.DataRetrievalInput<Entities.SaleRateQuery> input)
        {
            Action<string> createTempTableAction = (tempTableName) =>
            {
                string zonesids = null;
                if (input.Query.ZonesIds!=null &&  input.Query.ZonesIds.Count() > 0)
                    zonesids = string.Join<int>(",", input.Query.ZonesIds);



                ExecuteNonQuerySP("TOneWhS_BE.sp_SaleRate_CreateTempByFiltered", tempTableName, input.Query.EffectiveOn, input.Query.SellingNumberPlanId, zonesids, input.Query.OwnerType , input.Query.OwnerId);
            };

            return RetrieveData(input, createTempTableAction, SaleRateMapper);

        }
        public List<SaleRate> GetEffectiveSaleRates(SalePriceListOwnerType ownerType, int ownerId, DateTime effectiveOn)
        {
            return GetItemsSP("TOneWhS_BE.sp_SaleRate_GetByOwnerAndEffective", SaleRateMapper, ownerType, ownerId, effectiveOn);
        }
        public List<SaleRate> GetEffectiveSaleRateByCustomers(IEnumerable<RoutingCustomerInfo> customerInfos, DateTime? effectiveOn, bool isEffectiveInFuture)
        {
            DataTable dtActiveCustomers = CarrierAccountDataManager.BuildRoutingCustomerInfoTable(customerInfos);
            return GetItemsSPCmd("[TOneWhS_BE].[sp_SaleRate_GetEffectiveByCustomers]", SaleRateMapper, (cmd) =>
            {
                var dtPrm = new SqlParameter("@ActiveCustomersInfo", SqlDbType.Structured);
                dtPrm.Value = dtActiveCustomers;
                cmd.Parameters.Add(dtPrm);
                cmd.Parameters.Add(new SqlParameter("@CustomerOwnerType", SalePriceListOwnerType.Customer));
                cmd.Parameters.Add(new SqlParameter("@EffectiveTime", effectiveOn));
                cmd.Parameters.Add(new SqlParameter("@IsFuture", isEffectiveInFuture));
            });
        }
        public bool AreSaleRatesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("TOneWhS_BE.SaleRate", ref updateHandle);
        }
        public bool CloseRates(IEnumerable<RateChange> rateChanges)
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
        DataTable BuildRateChangesTable(IEnumerable<RateChange> rateChanges)
        {
            DataTable table = new DataTable();

            table.Columns.Add("RateID", typeof(long));
            table.Columns.Add("EED", typeof(DateTime));

            table.BeginLoadData();

            foreach (RateChange rateChange in rateChanges)
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
        public bool InsertRates(IEnumerable<NewRate> newRates, int priceListId)
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
        DataTable BuildNewRatesTable(IEnumerable<NewRate> newRates, int priceListId)
        {
            DataTable table = new DataTable();

            table.Columns.Add("ZoneID", typeof(long));
            table.Columns.Add("PriceListID", typeof(int));
            table.Columns.Add("CurrencyID", typeof(int));
            table.Columns.Add("NormalRate", typeof(decimal));
            table.Columns.Add("OtherRates", typeof(string));
            table.Columns.Add("BED", typeof(DateTime));
            table.Columns.Add("EED", typeof(DateTime));

            table.BeginLoadData();

            foreach (NewRate newRate in newRates)
            {
                DataRow row = table.NewRow();
                row["ZoneID"] = newRate.ZoneId;
                row["PriceListID"] = priceListId;
                if (newRate.CurrencyId != null)
                    row["CurrencyID"] = newRate.CurrencyId;
                row["NormalRate"] = newRate.NormalRate;
                if (newRate.OtherRates != null)
                    row["OtherRates"] = Vanrise.Common.Serializer.Serialize(newRate.OtherRates);
                row["BED"] = newRate.BED;
                if (newRate.EED != null)
                    row["EED"] = newRate.EED;
                table.Rows.Add(row);
            }

            table.EndLoadData();

            return table;
        }
        public IEnumerable<SaleRate> GetExistingRatesByZoneIds(SalePriceListOwnerType ownerType, int ownerId, IEnumerable<long> zoneIds, DateTime minEED)
        {
            string zoneIdsParameter = string.Join(",", zoneIds);
            return GetItemsSP("TOneWhS_BE.sp_SaleRate_GetExistingByZoneIDs", SaleRateMapper, ownerType, ownerId, zoneIdsParameter, minEED);
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

            saleRate.BED = (DateTime)reader["BED"];
            saleRate.EED = GetReaderValue<DateTime?>(reader, "EED");

            return saleRate;
        }

        #endregion
    }
}
