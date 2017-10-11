using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.DBSync.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class SourceTodDataManager : BaseSQLDataManager
    {
        readonly bool _getEffectiveOnly;
        public SourceTodDataManager(string connectionString, bool getEffectiveOnly)
            : base(connectionString, false)
        {
            _getEffectiveOnly = getEffectiveOnly;
        }

        public IEnumerable<SourceTod> GetSourceTods()
        {
            return GetItemsText(query_getTods, SourceTodMapper, (cmd) =>
            {
                cmd.Parameters.Add(new SqlParameter("@GetEffectiveOnly", _getEffectiveOnly));
            });
        }


        SourceTod SourceTodMapper(IDataReader reader)
        {
            string beginTime = reader["BeginTime"] as string;
            string endTime = reader["EndTime"] as string;
            return new SourceTod
            {
                TodId = (long)reader["ToDConsiderationID"],
                SupplierId = reader["SupplierId"] as string,
                CustomerId = reader["CustomerId"] as string,
                ZoneId = (int)reader["ZoneId"],
                BeginTime = string.IsNullOrEmpty(beginTime) ? (TimeSpan?)null : TimeSpan.Parse(beginTime),
                EndTime = string.IsNullOrEmpty(endTime) ? (TimeSpan?)null : TimeSpan.Parse(endTime),
                RateType = (ToDRateType)GetReaderValue<byte>(reader, "RateType"),
                DayOfWeek = (DayOfWeek?)GetReaderValue<byte>(reader, "WeekDay"),
                HolidayDateTime = GetReaderValue<DateTime?>(reader, "HolidayDate"),
                HolidayName = reader["HolidayName"] as string,
                BED = GetReaderValue<DateTime>(reader, "BeginEffectiveDate"),
                EED = GetReaderValue<DateTime?>(reader, "EndEffectiveDate")
            };
        }

        const string query_getTods = @"SELECT [ToDConsiderationID]
                                              ,[ZoneID]
                                              ,[SupplierID]
                                              ,[CustomerID]
                                              ,[BeginTime]
                                              ,[EndTime]
                                              ,[WeekDay]
                                              ,[HolidayDate]
                                              ,[HolidayName]
                                              ,[RateType]
                                              ,[BeginEffectiveDate]
                                              ,[EndEffectiveDate]
                                          FROM [ToDConsideration]
                                          WHERE  ((@GetEffectiveOnly = 0 and BeginEffectiveDate <= getdate()) or (@GetEffectiveOnly = 1 and IsEffective = 'Y'))
                                          order by CustomerId";
    }
}
