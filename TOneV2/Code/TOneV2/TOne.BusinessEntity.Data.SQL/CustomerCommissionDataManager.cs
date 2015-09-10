using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Entities;
using TOne.Data.SQL;
using Vanrise.Entities;

namespace TOne.BusinessEntity.Data.SQL
{
    public class CustomerCommissionDataManager : BaseTOneDataManager, ICustomerCommissionDataManager
    {
        public Vanrise.Entities.BigResult<CustomerCommission> GetCustomerCommissions(Vanrise.Entities.DataRetrievalInput<CustomerCommissionQuery> input)
        {
            string ZoneIDs = null;
            if (input.Query.ZoneIds != null && input.Query.ZoneIds.Count() > 0)
                ZoneIDs = string.Join<int>(",", input.Query.ZoneIds);

            Action<string> createTempTableAction = (tempTableName) =>
            {
                ExecuteNonQuerySP("BEntity.sp_Commision_CreateTempByCustomer", tempTableName, input.Query.CustomerId, ZoneIDs, input.Query.EffectiveFrom);
            };
            return RetrieveData(input, createTempTableAction, CommissionTempTableMapper);
        }

        public List<CustomerCommission> GetCustomerCommissions( DateTime when)
        {
            return GetItemsSP("BEntity.sp_Commission_GetSale", CommissionMapper, when);
        }

        private CustomerCommission CommissionMapper(IDataReader reader)
        {
            CustomerCommission commission = new CustomerCommission();
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
          
        }
        private CustomerCommission CommissionTempTableMapper(IDataReader reader)
        {
            CustomerCommission commission = new CustomerCommission();
            commission.ID = (int)reader["CommissionID"];
            commission.Amount = GetReaderValue<decimal?>(reader, "Amount");
            commission.BeginEffectiveDate = GetReaderValue<DateTime>(reader, "BeginEffectiveDate");
            commission.CustomerId = reader["CustomerID"] as string;
            commission.EndEffectiveDate = GetReaderValue<DateTime?>(reader, "EndEffectiveDate");
            decimal? fromRate = GetReaderValue<decimal?>(reader, "FromRate");
            if (fromRate.HasValue)
                commission.FromRate = (float)fromRate.Value;
            decimal? toRate = GetReaderValue<decimal?>(reader, "ToRate");
            if (toRate.HasValue)
                commission.ToRate = (float)toRate.Value;
            commission.IsEffective = reader["IsEffective"] as string == "Y";
            commission.IsExtraCharge = reader["IsExtraCharge"] as string == "Y";
            decimal? percentage = GetReaderValue<decimal?>(reader, "Percentage");
            if (percentage.HasValue)
                commission.Percentage = (float)percentage.Value;
            commission.SupplierId = reader["SupplierID"] as string;
            commission.UserId = (int)reader["UserID"];
            commission.ZoneId = GetReaderValue<int>(reader, "ZoneID");
            commission.ZoneName = reader["zoneName"] as string;
            commission.CustomerName = reader["CustomerName"] as string;
            commission.Currency = reader["Currency"] as string;
            return commission;

        }


    }
}
