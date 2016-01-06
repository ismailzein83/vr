using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Vanrise.Data.SQL;
using Vanrise.Fzero.CDRImport.Entities;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data.SQL
{
    public class DWTimeDataManager : BaseSQLDataManager, IDWTimeDataManager
    {

        public DWTimeDataManager()
            : base("DWSDBConnString")
        {

        }

        public List<DWTime> GetTimes(DateTime from, DateTime to)
        {
            string query = string.Format("SELECT [DateInstance] ,[Year] ,[Month] ,[Week],[Day] ,[Hour] ,[MonthName] ,[DayName]FROM [dbo].[Dim_Time] where DateInstance between  @FromDate and @ToDate");
            return GetItemsText(query, TimeMapper, (cmd) =>
            {
                cmd.Parameters.Add(new SqlParameter("@FromDate", from));
                cmd.Parameters.Add(new SqlParameter("@ToDate", to));
            });
        }


        #region Private Methods

        private DWTime TimeMapper(IDataReader reader)
        {
            var time = new DWTime();
            DateTime date = ((DateTime)reader["DateInstance"]);
            time.DateInstance = new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second, date.Kind);
            time.Year = GetReaderValue<int?>(reader,"Year");
            time.Month = GetReaderValue<int?>(reader,"Month");
            time.Week= GetReaderValue<int?>(reader,"Week");
            time.Day = GetReaderValue<int?>(reader,"Day");
            time.Hour = GetReaderValue<int?>(reader,"Hour");
            time.MonthName= reader["MonthName"] as string;
            time.DayName= reader["DayName"] as string;
            return time;
        }

        #endregion



    }
}
