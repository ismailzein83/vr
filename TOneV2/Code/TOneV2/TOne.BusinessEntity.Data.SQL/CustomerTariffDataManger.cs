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
    public class CustomerTariffDataManger : BaseTOneDataManager, ICustomerTariffDataManager
    {
        public Vanrise.Entities.BigResult<CustomerTariff> GetFilteredCustomerTariffs(Vanrise.Entities.DataRetrievalInput<CustomerTariffQuery> input)
        {
            Dictionary<string, string> mapper = new Dictionary<string, string>();

            mapper.Add("CustomerName", "CustomerID");
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
                ExecuteNonQueryText(CreateTempTableIfNotExists(tempTableName, input.Query.selectedCustomerID, input.Query.selectedZoneIDs), (cmd) =>
                {
                    cmd.Parameters.Add(new SqlParameter("@EffectiveOn", input.Query.effectiveOn));
                });
            };

            return RetrieveData(input, createTempTableAction, CustomerTariffMapper, mapper);
        }

        private string CreateTempTableIfNotExists(string tempTableName, string selectedCustomerID, List<int> selectedZoneIDs)
        {
            StringBuilder query = new StringBuilder(@""); ;
            return query.ToString();
        }

        private CustomerTariff CustomerTariffMapper(IDataReader reader)
        {
            CustomerTariff customerTariff = new CustomerTariff
            {
                TariffID = (long)reader["TariffID"],
                CustomerID = reader["CustomerID"] as string,
                ZoneID = GetReaderValue<int>(reader, "ZoneID"),
                Currency = GetReaderValue<string>(reader, "Currency"),
                CallFee = (decimal)reader["CallFee"],
                FirstPeriod = GetReaderValue<byte>(reader, "FirstPeriod"),
                FirstPeriodRate = GetReaderValue<decimal>(reader, "FirstPeriodRate"),
                FractionUnit = GetReaderValue<byte>(reader, "FractionUnit"),
                BED = GetReaderValue<DateTime>(reader, "BeginEffectiveDate"),
                EED = GetReaderValue<DateTime>(reader, "EndEffectiveDate"),
                IsEffective = (string)reader["IsEffective"]
            };

            return customerTariff;
        }

        protected IList<TABS.Tariff> GetTariffs(TABS.CarrierAccount supplier, TABS.CarrierAccount customer, TABS.Zone zone, DateTime from)
        {
            string hql = string.Format(@"FROM  Tariff t
                                                     WHERE (t.EndEffectiveDate > :when OR t.EndEffectiveDate IS NULL)
                                                        AND (t.EndEffectiveDate IS NULL OR t.EndEffectiveDate != t.BeginEffectiveDate)
                                                        {0}
                                                        {1}
                                                        {2}
                                                     ORDER BY t.BeginEffectiveDate"
                                                , (zone != null) ? " AND t.Zone = :zone" : ""
                                                , (customer != null) ? " AND t.Customer = :customer" : ""
                                                , (supplier != null) ? " AND t.Supplier = :supplier" : "");
            NHibernate.IQuery query = CurrentSession.CreateQuery(hql).SetParameter("when", from);
            if (zone != null) query.SetParameter("zone", zone);
            if (customer != null) query.SetParameter("customer", customer);
            if (supplier != null) query.SetParameter("supplier", supplier);
            IList<TABS.Tariff> listTariffs = query.List<TABS.Tariff>();
            return listTariffs;
        }
    }
}
