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
            mapper.Add("Currency", "CurrencyID");
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
                
                WITH Carriers AS
                (
	                SELECT ca.CarrierAccountID, cp.Name AS SupplierName, ca.NameSuffix, ca.GMTTime, cp.CurrencyID
	                FROM CarrierAccount ca INNER JOIN CarrierProfile cp ON cp.ProfileID = ca.ProfileID
	                --WHERE ca.ActivationStatus = 2
                ),

                Zones AS (SELECT ZoneID, Name AS ZoneName FROM Zone WHERE IsEffective = 'Y')

                SELECT
	                t.TariffID,
	                t.ZoneID,
	                z.ZoneName,
	                t.SupplierID,
	                c.SupplierName,
	                c.NameSuffix,
	                c.CurrencyID,
	                t.CallFee,
	                t.FirstPeriod,
	                t.FirstPeriodRate,
	                t.FractionUnit,
	                t.BeginEffectiveDate,
	                t.EndEffectiveDate,
	                t.IsEffective

                INTO #TEMP_TABLE_NAME#

                FROM Tariff t
                INNER JOIN Carriers c ON c.CarrierAccountID = t.SupplierID
                INNER JOIN Zones z ON z.ZoneID = t.ZoneID
                
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
                SupplierName = reader["SupplierName"] as string,
                ZoneID = GetReaderValue<int>(reader, "ZoneID"),
                ZoneName = reader["ZoneName"] as string,
                CurrencyID = GetReaderValue<string>(reader, "CurrencyID"),
                CallFee = (decimal)reader["CallFee"],
                FirstPeriod = GetReaderValue<byte>(reader, "FirstPeriod"),
                FirstPeriodRate = GetReaderValue<decimal>(reader, "FirstPeriodRate"),
                FractionUnit = GetReaderValue<byte>(reader, "FractionUnit"),
                BED = GetReaderValue<DateTime>(reader, "BeginEffectiveDate"),
                EED = GetReaderValue<DateTime>(reader, "EndEffectiveDate"),
                EEDDescription = GetReaderValue<string>(reader, "EndEffectiveDate"),
                IsEffective = (string)reader["IsEffective"]
            };

            return supplierTariff;
        }

        private string GetMainWhereClause(string selectedSupplierID, List<int> selectedZoneIDs)
        {
            string whereClause = "WHERE";

            whereClause += " t.CustomerID = 'SYS'";
            
            if (selectedSupplierID != null)
                whereClause += " AND SupplierID = '" + selectedSupplierID + "'";

            if (selectedZoneIDs.Count > 0)
                whereClause += " AND t.ZoneID IN (" + GetCommaSeparatedList(selectedZoneIDs) + ")";

            whereClause += " AND t.BeginEffectiveDate <= DATEADD(HH, c.GMTTime, @EffectiveOn)";
            whereClause += " AND (t.EndEffectiveDate IS NULL OR t.EndEffectiveDate > DATEADD(HH, c.GMTTime, @EffectiveOn))";

            return whereClause;
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
