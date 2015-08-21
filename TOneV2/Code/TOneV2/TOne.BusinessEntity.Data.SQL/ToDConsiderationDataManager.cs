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
            try
            {
                ToDConsideration toDConsideration = new ToDConsideration();


                toDConsideration.ToDConsiderationID = (int)(long)reader["ID"];//(int)reader["ID"]
                toDConsideration.ZoneId = GetReaderValue<int>(reader, "ZoneID");
                toDConsideration.SupplierId = reader["SupplierID"] as string;
                toDConsideration.CustomerId = reader["CustomerID"] as string;
                toDConsideration.BeginTime = reader["BeginTime"] as string;
                toDConsideration.EndTime = reader["EndTime"] as string;
                toDConsideration.WeekDay = reader["TODWeekDay"] != DBNull.Value ? (DayOfWeek)Enum.Parse(typeof(DayOfWeek), reader["TODWeekDay"].ToString()) : 0; //String.IsNullOrEmpty(reader["TODWeekDay"] as string) ? (DayOfWeek)Enum.Parse(typeof(DayOfWeek), reader["TODWeekDay"]as string) : 0;
                toDConsideration.HolidayDate = GetReaderValue<DateTime?>(reader, "HolidayDate");
                toDConsideration.HolidayName = reader["HolidayName"] as string;
                toDConsideration.RateType = reader["RateType"] != DBNull.Value ? (ToDRateType)Enum.Parse(typeof(ToDRateType), reader["RateType"].ToString()) : ToDRateType.Normal;// String.IsNullOrEmpty(reader["RateType"] as string) ? (ToDRateType)Enum.Parse(typeof(ToDRateType), reader["RateType"] as string) : ToDRateType.Normal;
                toDConsideration.BeginEffectiveDate = GetReaderValue<DateTime?>(reader, "BeginEffectiveDate");
                toDConsideration.EndEffectiveDate = GetReaderValue<DateTime?>(reader, "EndEffectiveDate");
                return toDConsideration;

            }
            catch (Exception ex)
            {
                string s = ex.Message;
            }
            return new ToDConsideration();
            //return new ToDConsideration
            //{
            //    ToDConsiderationID = GetReaderValue<int>(reader, "ID"),
            //    ZoneId = GetReaderValue<int>(reader, "ZoneID"),
            //    SupplierId = reader["SupplierID"] as string,
            //    CustomerId = reader["CustomerID"] as string,
            //    BeginTime = reader["BeginTime"] as string,
            //    EndTime = reader["EndTime"] as string,
            //    WeekDay = String.IsNullOrEmpty(reader["TODWeekDay"] as string) ? (DayOfWeek)Enum.Parse(typeof(DayOfWeek), reader["TODWeekDay"] as string) : 0,
            //    HolidayDate = GetReaderValue<DateTime?>(reader, "HolidayDate"),
            //    HolidayName = reader["HolidayName"] as string,
            //    RateType = String.IsNullOrEmpty(reader["RateType"] as string) ? (ToDRateType)Enum.Parse(typeof(ToDRateType), reader["RateType"] as string) : ToDRateType.Normal,
            //    BeginEffectiveDate = GetReaderValue<DateTime?>(reader, "BeginEffectiveDate"),
            //    EndEffectiveDate = GetReaderValue<DateTime?>(reader, "EndEffectiveDate"),
            //};
        }

        private TODConsiderationInfo ToDConsiderationInfoMapper(IDataReader reader)
        {
            TODConsiderationInfo tod = new TODConsiderationInfo();
            return tod;
         
        }

        public List<ToDConsideration> GetToDConsideration(string customerId, int zoneId, DateTime when)
        {
            return GetItemsSP("BEntity.sp_ToDConsideration_GetToDConsiderations", ToDConsiderationMapper, ToDBNullIfDefault(zoneId), customerId, when);
        }

        public Vanrise.Entities.BigResult<TODConsiderationInfo> GetToDConsiderationByCriteria(Vanrise.Entities.DataRetrievalInput<TODCustomerQuery> input)
        {
            return RetrieveData(input, (tempTableName) =>
            {
                ExecuteNonQuerySP("BEntity.sp_CustomersToDConsideration_CreateTempForFiltered", tempTableName, input.Query.ZoneId, input.Query.CustomerId, input.Query.EffectiveOn);

            }, ToDConsiderationInfoMapper);
        }
    }
}
