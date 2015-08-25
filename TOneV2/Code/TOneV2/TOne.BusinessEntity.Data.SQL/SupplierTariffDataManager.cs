using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TOne.BusinessEntity.Entities;
using TOne.Data.SQL;

namespace TOne.BusinessEntity.Data.SQL
{
    public class SupplierTariffDataManager : BaseTOneDataManager, ISupplierTariffDataManager
    {
        public Vanrise.Entities.BigResult<SupplierTariff> GetFilteredSupplierTariffs(Vanrise.Entities.DataRetrievalInput<SupplierTariffQuery> input)
        {
            Dictionary<string, string> mapper = new Dictionary<string, string>();

            mapper.Add("SupplierName", "SupplierID");
            mapper.Add("ZoneName", "ZoneID");
            mapper.Add("Currency", "Currency");
            mapper.Add("CallFee", "CallFee");
            mapper.Add("FirstPeriod", "FirstPeriod");
            mapper.Add("FirstPeriodRate", "FirstPeriodRate");
            mapper.Add("FractionUnit", "FractionUnit");
            mapper.Add("BED", "BeginEffectiveDate");
            mapper.Add("EED", "EndEffectiveDate");
            mapper.Add("IsEffective", "IsEffective");

            Action<string> createTempTableAction = (tempTableName) =>
            {
                ExecuteNonQueryText(CreateTempTableIfNotExists(tempTableName, input.Query.selectedSupplierID, input.Query.selectedZoneIDs), (cmd) =>
                {
                    cmd.Parameters.Add(new SqlParameter("@EffectiveOn", input.Query.effectiveOn));
                });
            };
            return RetrieveData(input, createTempTableAction, SupplierTariffMapper, mapper);
        }

        private string CreateTempTableIfNotExists(string tempTableName, string selectedSupplierID, List<int> selectedZoneIDs)
        {
            StringBuilder query = new StringBuilder(@"
                IF NOT OBJECT_ID('#TEMP_TABLE_NAME#', N'U') IS NOT NULL
                BEGIN
                
                WITH CurrencyCTE AS
                (
	                SELECT ca.CarrierAccountID, cp.CurrencyID AS Currency
	                FROM CarrierAccount ca INNER JOIN CarrierProfile cp ON cp.ProfileID = ca.ProfileID
	                WHERE ca.AccountType IN (1, 2)
                )

                SELECT
	                t.TariffID,
	                t.SupplierID,
	                t.ZoneID,
	                cte.Currency,
	                t.CallFee,
	                t.FirstPeriod,
	                t.FirstPeriodRate,
	                t.RepeatFirstPeriod,
	                t.FractionUnit,
	                t.BeginEffectiveDate,
	                t.EndEffectiveDate,
	                t.IsEffective

                INTO #TEMP_TABLE_NAME#

                FROM Tariff t
                INNER JOIN CurrencyCTE cte ON cte.CarrierAccountID = t.SupplierID
                INNER JOIN CarrierAccount ca ON ca.CarrierAccountID = t.SupplierID
                
                #MAIN_WHERE_CLAUSE#

                ORDER BY t.BeginEffectiveDate
                
	            END
            ");

            query.Replace("#TEMP_TABLE_NAME#", tempTableName);
            query.Replace("#MAIN_WHERE_CLAUSE#", GetMainWhereClause(selectedSupplierID, selectedZoneIDs));

            return query.ToString();
        }

        private SupplierTariff SupplierTariffMapper(IDataReader reader)
        {
            SupplierTariff supplierTariff = new SupplierTariff
            {
                TariffID = (long)reader["TariffID"],
                SupplierID = reader["SupplierID"] as string,
                ZoneID = GetReaderValue<int>(reader, "ZoneID"),
                Currency = GetReaderValue<string>(reader, "Currency"),
                CallFee = (decimal)reader["CallFee"],
                FirstPeriod = GetReaderValue<byte>(reader, "FirstPeriod"),
                FirstPeriodRate = GetReaderValue<decimal>(reader, "FirstPeriodRate"),
                FractionUnit = GetReaderValue<byte>(reader, "FractionUnit"),
                BED = GetReaderValue<DateTime>(reader, "BeginEffectiveDate"),
                EED = GetReaderValue<DateTime>(reader, "EndEffectiveDate"),
                EndEffectiveDateDescription = (GetReaderValue<DateTime>(reader, "EndEffectiveDate") == Convert.ToDateTime(null)) ? "" : Convert.ToString(Convert.ToDateTime(reader["EndEffectiveDate"]).Date.ToString()),
                IsEffective = (string)reader["IsEffective"]
            };

            return supplierTariff;
        }

        private string GetMainWhereClause(string selectedSupplierID, List<int> selectedZoneIDs)
        {
            string whereClause = "WHERE";

            whereClause += " t.CustomerID = 'SYS'";
            whereClause += " AND t.BeginEffectiveDate <= DATEADD(HH, ca.GMTTime, @EffectiveOn)";
            whereClause += " AND (t.EndEffectiveDate IS NULL OR DATEADD(HH, ca.GMTTime, @EffectiveOn) < t.EndEffectiveDate)";
            
            if (selectedSupplierID != null)
                whereClause += " AND SupplierID = '" + selectedSupplierID + "'";

            if (selectedZoneIDs.Count > 0)
                whereClause += " AND ZoneID IN (" + GetCommaSeparatedList(selectedZoneIDs) + ")";
            //else if (selectedSupplierID != null)
            //{
            //    ZoneManager manager = new ZoneManager();
            //    List<TOne.BusinessEntity.Entities.ZoneInfo> supplierZones = manager.GetZonesBySupplierID(selectedSupplierID);

            //    whereClause += " AND ZoneID IN (" + GetCommaSeparatedList(GetZoneIDs(supplierZones)) + ")";
            //}

            return whereClause;
        }

        private List<int> GetZoneIDs(List<TOne.BusinessEntity.Entities.ZoneInfo> zones)
        {
            List<int> ids = new List<int>();

            foreach (TOne.BusinessEntity.Entities.ZoneInfo zone in zones)
            {
                ids.Add(zone.ZoneId);
            }

            return ids;
        }

        private string GetCommaSeparatedList(List<int> items)
        {
            string list = "";

            for (int i = 0; i < items.Count; i++)
            {
                list += (i > 0) ? (", " + items[i].ToString()) : items[i].ToString();
            }

            return list;
        }
    }
}
