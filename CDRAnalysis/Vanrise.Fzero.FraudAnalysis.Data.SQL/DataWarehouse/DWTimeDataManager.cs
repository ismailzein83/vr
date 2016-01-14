using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Vanrise.Data;
using Vanrise.Data.SQL;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data.SQL
{
    public class DWTimeDataManager : BaseSQLDataManager, IDWTimeDataManager
    {

        public DWTimeDataManager()
            : base("DWSDBConnString")
        {

        }


        static string[] s_Columns = new string[] {
            "DateInstance",
            "Year",
            "Month",
            "Week",
            "Day",
            "Hour",
            "MonthName",
            "DayName"
        };


        #region Private Methods

        private DWTime TimeMapper(IDataReader reader)
        {
            var time = new DWTime();
            time.DateInstance = ((DateTime)reader["DateInstance"]);
            time.Year = GetReaderValue<int?>(reader, "Year");
            time.Month = GetReaderValue<int?>(reader, "Month");
            time.Week = GetReaderValue<int?>(reader, "Week");
            time.Day = GetReaderValue<int?>(reader, "Day");
            time.Hour = GetReaderValue<int?>(reader, "Hour");
            time.MonthName = reader["MonthName"] as string;
            time.DayName = reader["DayName"] as string;
            return time;
        }

        #endregion



        public List<DWTime> GetTimes(DateTime from, DateTime to)
        {
            string query = string.Format("SELECT [DateInstance] ,[Year] ,[Month] ,[Week],[Day] ,[Hour] ,[MonthName] ,[DayName]FROM [dbo].[Dim_Time] where DateInstance between  @FromDate and @ToDate");
            return GetItemsText(query, TimeMapper, (cmd) =>
            {
                cmd.Parameters.Add(new SqlParameter("@FromDate", from));
                cmd.Parameters.Add(new SqlParameter("@ToDate", to));
            });
        }

        public void ApplyDWTimesToDB(object preparedDWTimes)
        {
            InsertBulkToTable(preparedDWTimes as BaseBulkInsertInfo);
        }

        public void SaveDWTimesToDB(List<DWTime> dwTimes)
        {
            object dbApplyStream = InitialiazeStreamForDBApply();

            foreach (DWTime dim in dwTimes)
            {
                WriteRecordToStream(dim, dbApplyStream);
            }

            ApplyDWTimesToDB(FinishDBApplyStream(dbApplyStream));
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                TableName = "Dim_Time",
                Stream = streamForBulkInsert,
                ColumnNames=s_Columns,
                TabLock = false,
                KeepIdentity = false,
                FieldSeparator = '^'
            };
        }

        public object InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        public void WriteRecordToStream(DWTime record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}", record.DateInstance, record.Year, record.Month, record.Week, record.Day, record.Hour, record.MonthName, record.DayName);
        }
    }
}
