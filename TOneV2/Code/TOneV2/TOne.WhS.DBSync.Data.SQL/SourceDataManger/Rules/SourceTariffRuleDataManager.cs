using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.DBSync.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.DBSync.Data.SQL.SourceDataManger
{
    public class SourceTariffRuleDataManager : BaseSQLDataManager
    {
        readonly bool _getEffectiveOnly;
        DateTime? _effectiveAfter;
        public SourceTariffRuleDataManager(string connectionString, DateTime? effectiveAfter, bool getEffectiveOnly)
            : base(connectionString, false)
        {
            _effectiveAfter = effectiveAfter;
            _getEffectiveOnly = getEffectiveOnly;
        }

        public IEnumerable<SourceTariffRule> GetTariffRules()
        {
            return GetItemsText(string.Format(query_getSourceTariffRules, MigrationUtils.GetEffectiveQuery("t", _getEffectiveOnly, _effectiveAfter)), SourceTariffRuleMapper, null);
        }

        private SourceTariffRule SourceTariffRuleMapper(IDataReader reader)
        {
            return new SourceTariffRule
            {
                SourceId = reader["TariffID"].ToString(),
                BED = (DateTime)reader["BeginEffectiveDate"],
                EED = GetReaderValue<DateTime?>(reader, "EndEffectiveDate"),
                CustomerId = reader["CustomerID"] as string,
                SupplierId = reader["SupplierID"] as string,
                ZoneId = (int)reader["ZoneID"],
                RepeatFirstPeriod = (reader["RepeatFirstPeriod"] as string).Equals("Y"),
                CallFee = GetReaderValue<decimal>(reader, "CallFee"),
                FirstPeriod = GetReaderValue<byte>(reader, "FirstPeriod"),
                FirstPeriodRate = GetReaderValue<decimal>(reader, "FirstPeriodRate"),
                FractionUnit = GetReaderValue<byte>(reader, "FractionUnit"),
                SupplierProfileID = reader["ProfileID"].ToString()
            };
        }

        const string query_getSourceTariffRules = @"SELECT [TariffID]
                                                          ,[ZoneID]
                                                          ,[CustomerID]
                                                          ,[SupplierID]
                                                          ,[CallFee]
                                                          ,[FirstPeriodRate]
                                                          ,[FirstPeriod]
                                                          ,[RepeatFirstPeriod]
                                                          ,[FractionUnit]
                                                          ,[BeginEffectiveDate]
                                                          ,[EndEffectiveDate]
														  ,ca.ProfileID
                                                      FROM      [dbo].[Tariff] t
													  join CarrierAccount ca on ca.CarrierAccountID = SupplierID
                                                      WHERE  1=1 {0}";
    }
}
