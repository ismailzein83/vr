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
    public class CustomerToDDataManager : ToDConsiderationDataManager ,ICustomerTODDataManager
    {
       
        public Vanrise.Entities.BigResult<CustomerTODConsiderationInfo> GetCustomerToDConsiderationByCriteria(Vanrise.Entities.DataRetrievalInput<TODQuery> input )
        {


            return RetrieveData(input, (tempTableName) =>
            {
                string zoneIds = null;
                if (input.Query.ZoneIds != null && input.Query.ZoneIds.Count() > 0)
                    zoneIds = string.Join<int>(",", input.Query.ZoneIds);
                ExecuteNonQuerySP("BEntity.sp_CustomersToDConsideration_CreateTempForFiltered", tempTableName, zoneIds, input.Query.CustomerId, input.Query.EffectiveOn);

            }, (reader) => CustomerTODConsiderationInfoMapper(reader, input.Query.EffectiveOn));
        }


        private CustomerTODConsiderationInfo CustomerTODConsiderationInfoMapper(IDataReader reader, DateTime effectiveOn)
        {
            CustomerTODConsiderationInfo tod = base.ToDConsiderationInfoMapper<CustomerTODConsiderationInfo>(reader, effectiveOn);
            tod.CustomerNameSuffix = reader["CustomerNameSuffix"] as string;
            return tod;
        }

        public List<CustomerTODConsiderationInfo> GetCustomersToDConsideration( DateTime when)
        {
            return GetItemsSP("BEntity.sp_ToDConsideration_GetSale", ToDConsiderationMapper, when);
        }
        private CustomerTODConsiderationInfo ToDConsiderationMapper(IDataReader reader)
        {
            CustomerTODConsiderationInfo toDConsideration = new CustomerTODConsiderationInfo();
                toDConsideration.ToDConsiderationID = (int)(long)reader["ID"];
                toDConsideration.ZoneID = GetReaderValue<int>(reader, "ZoneID");
                toDConsideration.SupplierID = reader["SupplierID"] as string;
                toDConsideration.CustomerID = reader["CustomerID"] as string;
                toDConsideration.BeginTime = reader["BeginTime"] as string;
                toDConsideration.EndTime = reader["EndTime"] as string;
                toDConsideration.WeekDay = reader["TODWeekDay"] != DBNull.Value ? (DayOfWeek)Enum.Parse(typeof(DayOfWeek), reader["TODWeekDay"].ToString()) : 0; 
                toDConsideration.HolidayDate = GetReaderValue<DateTime?>(reader, "HolidayDate");
                toDConsideration.HolidayName = reader["HolidayName"] as string;
                toDConsideration.RateType = reader["RateType"] != DBNull.Value ? (ToDRateType)Enum.Parse(typeof(ToDRateType), reader["RateType"].ToString()) : ToDRateType.Normal;
                toDConsideration.BeginEffectiveDate = GetReaderValue<DateTime?>(reader, "BeginEffectiveDate");
                toDConsideration.EndEffectiveDate = GetReaderValue<DateTime?>(reader, "EndEffectiveDate");
                return toDConsideration;

        }

    }
}
