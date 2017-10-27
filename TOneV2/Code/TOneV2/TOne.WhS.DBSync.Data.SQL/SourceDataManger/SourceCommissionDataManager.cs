using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.DBSync.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class SourceCommissionDataManager : BaseSQLDataManager
    {
        DateTime? _effectiveFrom;
        bool _onlyEffective;
        public SourceCommissionDataManager(string connectionString, DateTime? effectiveFrom, bool onlyEffective)
            : base(connectionString, false)
        {
            _effectiveFrom = effectiveFrom;
            _onlyEffective = onlyEffective;
        }

        public IEnumerable<SourceCommission> GetSourceCommissions()
        {
            return GetItemsText(query_getCommissions + MigrationUtils.GetEffectiveQuery("c", _onlyEffective, _effectiveFrom), SourceCommissionMapper, null);
        }


        SourceCommission SourceCommissionMapper(IDataReader reader)
        {
            return new SourceCommission
            {
                CommissionId = (int)reader["CommissionID"],
                SupplierId = reader["SupplierId"] as string,
                CustomerId = reader["CustomerId"] as string,
                ZoneId = (int)reader["ZoneId"],
                FromRate = GetReaderValue<decimal>(reader, "FromRate"),
                ToRate = GetReaderValue<decimal>(reader, "ToRate"),
                Percentage = GetReaderValue<decimal?>(reader, "Percentage"),
                Amount = GetReaderValue<decimal?>(reader, "Amount"),
                BED = GetReaderValue<DateTime>(reader, "BeginEffectiveDate"),
                EED = GetReaderValue<DateTime?>(reader, "EndEffectiveDate"),
                IsExtraCharge = "Y".Equals(reader["IsExtraCharge"] as string)
            };
        }

        const string query_getCommissions = @"SELECT [CommissionID]
                                                      ,[SupplierID]
                                                      ,[CustomerID]
                                                      ,[ZoneID]
                                                      ,[FromRate]
                                                      ,[ToRate]
                                                      ,[Percentage]
                                                      ,[Amount]
                                                      ,[BeginEffectiveDate]
                                                      ,[EndEffectiveDate]
                                                      ,[IsExtraCharge]      
                                                  FROM [Commission] c where 1=1 ";
    }
}
