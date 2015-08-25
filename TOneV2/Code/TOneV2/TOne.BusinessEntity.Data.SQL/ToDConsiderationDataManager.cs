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

        private TODConsiderationInfo ToDConsiderationInfoMapper(IDataReader reader ,DateTime when)
        {
            TODConsiderationInfo toDConsiderationInfo = new TODConsiderationInfo
            {
                ToDConsiderationID = GetReaderValue<long>(reader, "ToDConsiderationID"),
                ZoneID = GetReaderValue<int>(reader, "ZoneID"),
                SupplierID = reader["SupplierID"] as string,
                CustomerID = reader["CustomerID"] as string,
                BeginTime = reader["BeginTime"] as string,
                EndTime = reader["EndTime"] as string,
                WeekDay = reader["WeekDay"] != DBNull.Value ? (DayOfWeek)Enum.Parse(typeof(DayOfWeek), reader["WeekDay"].ToString()) : 0,
                HolidayDate = GetReaderValue<DateTime>(reader, "HolidayDate"),
                HolidayName = reader["HolidayName"] as string,
                RateType = reader["RateType"] != DBNull.Value ? (ToDRateType)Enum.Parse(typeof(ToDRateType), reader["RateType"].ToString()) : ToDRateType.Normal,
                BeginEffectiveDate = GetReaderValue<DateTime>(reader, "BeginEffectiveDate"),
                EndEffectiveDate = (reader["EndEffectiveDate"] == DBNull.Value) ? null : (DateTime?)GetReaderValue<DateTime>(reader, "EndEffectiveDate"),
                UserID = GetReaderValue<int>(reader, "UserID"),
                ZoneName = reader["ZoneName"] as string,
                CarrierName = reader["CarrierName"] as string,
                DefinitionDisplayS = reader["DefinitionDisplayS"] as string,
            };
            toDConsiderationInfo.IsActive = WasActive(when, toDConsiderationInfo);

            return toDConsiderationInfo;
         
        }

        private bool WasActive(DateTime when , TODConsiderationInfo tod )
        {
            if (!GetIsEffective(tod.BeginEffectiveDate, tod.EndEffectiveDate, when)) return false;

            string time = when.ToString("HH:mm:ss.fff");

            // Assume effective
            bool isActive = true;

            // In any 
            if (tod.BeginTime != null && (time.CompareTo(tod.BeginTime) < 0 || time.CompareTo(tod.EndTime) > 0)) isActive = false;
            if (tod.WeekDay != null && when.DayOfWeek != tod.WeekDay.Value) isActive = false;
            if (tod.HolidayDate != null && (tod.HolidayDate.Value.Month != when.Month || tod.HolidayDate.Value.Day != when.Day)) isActive = false;

            return isActive;
        }



        private bool GetIsEffective(DateTime? BeginEffectiveDate, DateTime? EndEffectiveDate, DateTime when)
        {
            bool isEffective = BeginEffectiveDate.HasValue ? BeginEffectiveDate.Value <= when : true;
            if (isEffective)
                isEffective = EndEffectiveDate.HasValue ? EndEffectiveDate.Value >= when : true;
            return isEffective;
        }

        public List<ToDConsideration> GetToDConsideration(string customerId, int zoneId, DateTime when)
        {
            return GetItemsSP("BEntity.sp_ToDConsideration_GetToDConsiderations", ToDConsiderationMapper, ToDBNullIfDefault(zoneId), customerId, when);
        }

        public Vanrise.Entities.BigResult<TODConsiderationInfo> GetToDConsiderationByCriteria(Vanrise.Entities.DataRetrievalInput<TODCustomerQuery> input)
        {
            return RetrieveData(input, (tempTableName) =>
            {
                ExecuteNonQuerySP("BEntity.sp_CustomersToDConsideration_CreateTempForFiltered", tempTableName, (input.Query.ZoneIds.Count()>0 && input.Query.ZoneIds!=null )? string.Join(",", input.Query.ZoneIds.Select(x => x.ToString()).ToArray())  : null, input.Query.CustomerId , input.Query.EffectiveOn);

            }, (reader) => ToDConsiderationInfoMapper(reader, input.Query.EffectiveOn));
        }
    }
}
