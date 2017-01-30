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
    public class SourceTodDataManager : BaseSQLDataManager
    {
        public SourceTodDataManager(string connectionString)
            : base(connectionString, false)
        {

        }

        public IEnumerable<SourceTod> GetSourceTods()
        {
            return GetItemsText(query_getTods, SourceTodMapper, null);
        }


        SourceTod SourceTodMapper(IDataReader reader)
        {
            return new SourceTod
            {
                TodId = (int)reader["ToDConsiderationID"],
                SupplierId = reader["SupplierId"] as string,
                CustomerId = reader["CustomerId"] as string,
                ZoneId = (int)reader["ZoneId"],
                BeginTime = reader["BeginTime"] as string,
                EndTime = reader["EndTime"] as string,
                RateType = GetReaderValue<short>(reader, "RateType"),
                WeekDay = GetReaderValue<short>(reader, "Amount"),
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
                                          order by CustomerId";
    }
}
