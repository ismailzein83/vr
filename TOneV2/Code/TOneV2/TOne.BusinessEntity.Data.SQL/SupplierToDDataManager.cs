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
    public class SupplierToDDataManager : ToDConsiderationDataManager , ISupplierTODDataManager
    {
       
        public Vanrise.Entities.BigResult<SupplierTODConsiderationInfo> GetSupplierToDConsiderationByCriteria(Vanrise.Entities.DataRetrievalInput<TODQuery> input , List<string> suppliersAMUids )
        {


            return RetrieveData(input, (tempTableName) =>
            {
                string zoneIds = null;
                string suppliersIds = null;
                if (input.Query.ZoneIds.Count() > 0)
                    zoneIds = string.Join<int>(",", input.Query.ZoneIds);
                if (suppliersAMUids.Count() > 0)
                    suppliersIds = string.Join<string>(",", suppliersAMUids);
                ExecuteNonQuerySP("BEntity.sp_SupplierToDConsideration_CreateTempForFiltered", tempTableName, zoneIds, suppliersIds, input.Query.SupplierId, input.Query.EffectiveOn);

            }, (reader) => SupplierToDConsiderationInfoMapper(reader, input.Query.EffectiveOn));
        }

        private SupplierTODConsiderationInfo SupplierToDConsiderationInfoMapper(IDataReader reader, DateTime effectiveOn)
        {
            SupplierTODConsiderationInfo tod = base.ToDConsiderationInfoMapper<SupplierTODConsiderationInfo>(reader, effectiveOn);
            tod.UserName = reader["UserName"] as string;
            tod.WeekDayName = reader["WeekDayName"] as string;
            tod.CustomerName = reader["CustomerName"] as string;
            return tod;
        }
        public List<SupplierTODConsiderationInfo> GetSuppliersToDConsideration(DateTime when)
        {
            return GetItemsSP("BEntity.sp_ToDConsideration_GetCost", ToDConsiderationMapper, when);
        }
        private SupplierTODConsiderationInfo ToDConsiderationMapper(IDataReader reader)
        {
            SupplierTODConsiderationInfo toDConsideration = new SupplierTODConsiderationInfo();
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
