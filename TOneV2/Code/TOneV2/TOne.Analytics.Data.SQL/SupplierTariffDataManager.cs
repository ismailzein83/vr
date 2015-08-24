using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Analytics.Entities;
using TOne.Data.SQL;

namespace TOne.Analytics.Data.SQL
{
    public class SupplierTariffDataManager : BaseTOneDataManager, ISupplierTariffDataManager
    {
        public Vanrise.Entities.BigResult<SupplierTariff> GetFilteredSupplierTariffs(Vanrise.Entities.DataRetrievalInput<SupplierTariffQuery> input)
        {
            Dictionary<string, string> mapper = new Dictionary<string, string>();

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

                FROM Tariff t INNER JOIN CurrencyCTE cte ON cte.CarrierAccountID = t.SupplierID

                WHERE
	                t.CustomerID = 'SYS'
	                AND (t.EndEffectiveDate IS NULL OR t.EndEffectiveDate > @EffectiveOn)
	                AND t.BeginEffectiveDate <= @EffectiveOn
            ");
            return query.ToString();
        }

        private SupplierTariff SupplierTariffMapper(IDataReader reader)
        {
            SupplierTariff supplierTariff = new SupplierTariff
            {
                
            };

            return supplierTariff;
        }
    }
}
