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
        public SourceTariffRuleDataManager(string connectionString, bool getEffectiveOnly)
            : base(connectionString, false)
        {
            _getEffectiveOnly = getEffectiveOnly;
        }

        public IEnumerable<SourceTariffRule> GetTariffRules()
        {
            return GetItemsText(query_getSourceTariffRules, SourceTariffRuleMapper, (cmd) =>
            {
                cmd.Parameters.Add(new SqlParameter("@GetEffectiveOnly", _getEffectiveOnly));
            });
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
                FractionUnit = GetReaderValue<byte>(reader, "FractionUnit")
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
                                                      FROM      [dbo].[Tariff]
                                                      WHERE  ((@GetEffectiveOnly = 0 and BeginEffectiveDate <= getdate()) 
                                                                or (@GetEffectiveOnly = 1 and IsEffective = 'Y'))";
    }
}
