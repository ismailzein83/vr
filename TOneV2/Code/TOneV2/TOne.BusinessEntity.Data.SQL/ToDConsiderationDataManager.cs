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
    public class ToDConsiderationDataManager : BaseTOneDataManager, IToDConsiderationDataManager
    {
        private ToDConsideration ToDConsiderationMapper(IDataReader reader)
        {
            return new ToDConsideration
            {
                ToDConsiderationID = (int)reader["ID"],
                ZoneId = GetReaderValue<int>(reader, "ZoneID"),
                SupplierId = reader["SupplierID"] as string,
                CustomerId = reader["CustomerID"] as string,
                BeginTime = reader["BeginTime"] as string,
                EndTime = reader["EndTime"] as string,
                WeekDay = String.IsNullOrEmpty(reader["TODWeekDay"] as string) ? (DayOfWeek)Enum.Parse(typeof(DayOfWeek), reader["TODWeekDay"] as string) : 0,
                HolidayDate = GetReaderValue<DateTime?>(reader, "HolidayDate"),
                HolidayName = reader["HolidayName"] as string,
                RateType =  String.IsNullOrEmpty(reader["RateType"] as string) ?  (ToDRateType)Enum.Parse(typeof(ToDRateType), reader["RateType"] as string) : ToDRateType.Normal,
                BeginEffectiveDate = GetReaderValue<DateTime?>(reader, "BeginEffectiveDate"),
                EndEffectiveDate = GetReaderValue<DateTime?>(reader, "EndEffectiveDate"),
            };
        }

        public List<ToDConsideration> GetToDConsideration(string customerId, int zoneId, DateTime when)
        {
            return GetItemsSP("BEntity.sp_ToDConsideration_GetToDConsiderations", ToDConsiderationMapper, zoneId, customerId, when);
        }

    }
}
