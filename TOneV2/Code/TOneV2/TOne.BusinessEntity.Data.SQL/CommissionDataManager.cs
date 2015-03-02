using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Entities;
using TOne.Data.SQL;

namespace TOne.BusinessEntity.Data.SQL
{
    public class CommissionDataManager : BaseTOneDataManager, ICommissionDataManager
    {

        private Commission CommissionMapper(IDataReader reader)
        {
            return new Commission
            {
                ID = (int)reader["ID"],
                SupplierId = reader["supplierId"] as string,
                CustomerId = reader["customerId"] as string,
                ZoneId = GetReaderValue<int>(reader, "zoneId"),
                Amount = GetReaderValue<decimal?>(reader, "amount"),
                FromRate = GetReaderValue<float?>(reader, "fromRate"),
                ToRate = GetReaderValue<float?>(reader, "toRate"),
                IsExtraCharge = reader["isExtraCharge"] as string == "Y",
                Percentage = GetReaderValue<float?>(reader, "percentage"),
                BeginEffectiveDate = GetReaderValue<DateTime>(reader, "BED"),
                EndEffectiveDate = GetReaderValue<DateTime>(reader, "EED"),
                IsEffective = reader["IsEffective"] as string == "Y",
                UserId = (int)reader["userId"],
            };
        }

        public List<Commission> GetCommission(string customerId, int zoneId, DateTime when)
        {
            return GetItemsSP("BEntity.sp_Commission_GetCommissions", CommissionMapper, zoneId, customerId, when);
        }
    }
}
