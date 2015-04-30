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
            //try
            //{
            Commission commission = new Commission();
            commission.ID = (int)reader["ID"];
            commission.SupplierId = reader["supplierId"] as string;
            commission.CustomerId = reader["customerId"] as string;
            commission.ZoneId = GetReaderValue<int>(reader, "zoneId");
            commission.Amount = GetReaderValue<decimal?>(reader, "amount");
            decimal? fromRate = GetReaderValue<decimal?>(reader, "fromRate");
            if (fromRate.HasValue)
                commission.FromRate = (float)fromRate.Value;
            decimal? toRate = GetReaderValue<decimal?>(reader, "toRate");
            if (toRate.HasValue)
                commission.ToRate = (float)toRate.Value;
            commission.IsExtraCharge = reader["isExtraCharge"] as string == "Y";
            decimal? percentage = GetReaderValue<decimal?>(reader, "percentage");
            if (percentage.HasValue)
                commission.Percentage = (float)percentage.Value;
            commission.BeginEffectiveDate = GetReaderValue<DateTime>(reader, "BED");
            commission.EndEffectiveDate = GetReaderValue<DateTime>(reader, "EED");
            commission.IsEffective = reader["IsEffective"] as string == "Y";
            commission.UserId = (int)reader["userId"];
            return commission;
            //}
            //catch (Exception ex)
            //{
            //    string s = ex.Message;
            //}
            //return new Commission();
            //return new Commission
            //{
            //    ID = (int)reader["ID"],
            //    SupplierId = reader["supplierId"] as string,
            //    CustomerId = reader["customerId"] as string,
            //    ZoneId = GetReaderValue<int>(reader, "zoneId"),
            //    Amount = GetReaderValue<decimal?>(reader, "amount"),
            //    FromRate = (float)GetReaderValue<decimal?>(reader, "fromRate"),
            //    ToRate = (float)GetReaderValue<decimal?>(reader, "toRate"),
            //    IsExtraCharge = reader["isExtraCharge"] as string == "Y",
            //    Percentage = (float)GetReaderValue<decimal?>(reader, "percentage"),
            //    BeginEffectiveDate = GetReaderValue<DateTime>(reader, "BED"),
            //    EndEffectiveDate = GetReaderValue<DateTime>(reader, "EED"),
            //    IsEffective = reader["IsEffective"] as string == "Y",
            //    UserId = (int)reader["userId"],
            //};
        }

        public List<Commission> GetCommission(string customerId, int zoneId, DateTime when)
        {
            return GetItemsSP("BEntity.sp_Commission_GetCommissions", CommissionMapper, zoneId, customerId, when);
        }
    }
}
