using Microsoft.AnalysisServices.AdomdClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BI.Entities;

namespace TOne.BI.Data.SQL
{
    public class BaseDataManager
    {
        string _biConnectionString;
        string _cubeName;

        protected string CubeName
        {
            get
            {
                return _cubeName;
            }
        }
        public BaseDataManager()
        {
            _biConnectionString = ConfigurationManager.ConnectionStrings["BIConnectionString"].ConnectionString;
            _cubeName = ConfigurationManager.AppSettings["BICubeName"];
        }

        private CellSet GetCellSet(string mdxQuery)
        {
            using (var Connection = new AdomdConnection(_biConnectionString))
            {
                Connection.Open();

                AdomdCommand command = Connection.CreateCommand();
                command.CommandText = mdxQuery;

                CellSet cs = command.ExecuteCellSet();

               
                Connection.Close();

                return cs;
            }
        }

        private DataTable ConvertCellSetToDataSet(CellSet csResult)
        {
            if (csResult == null)
            {
                return null;
            }

            DataTable dtResult = new DataTable();
            DataColumn dcResult = null;
            DataRow drResult = null;
            int numberOfDims = 0;
            if (csResult.Axes.Count > 1)
            {
                if (csResult.Axes[1].Positions.Count > 0)
                {
                    Position py = csResult.Axes[1].Positions[0];

                    int index = 1;
                    numberOfDims = py.Members.Count;

                    foreach (Member m in py.Members)
                    {
                        dcResult = new DataColumn();
                        dcResult.ColumnName = m.LevelName;
                        dtResult.Columns.Add(dcResult);
                        index++;
                    }
                }
            }

            String name = null;
            foreach (Position pos in csResult.Axes[0].Positions)
            {
                dcResult = new DataColumn();
                name = string.Empty;
                foreach (Member m in pos.Members)
                {
                    name += m.Caption + ";";
                }
                dcResult.ColumnName = name;
                dtResult.Columns.Add(dcResult);
            }

            int y = 0;
            int colIndex = 0;
            int x = 0;
            if (csResult.Axes.Count > 1)
            {
                foreach (Position py in csResult.Axes[1].Positions)
                {
                    drResult = dtResult.NewRow();
                    colIndex = 0;
                    foreach (Member m in py.Members)
                    {
                        drResult[colIndex] = m.Caption;
                        colIndex++;
                    }

                    for (x = 0; x < csResult.Axes[0].Positions.Count; x++)
                    {
                        drResult[colIndex] = csResult[x, y].FormattedValue;
                        colIndex++;
                    }
                    dtResult.Rows.Add(drResult);
                    y++;
                }
            }
            return dtResult;
        }

        public DataTable GetData(string mdxQuery)
        {
            CellSet cs = GetCellSet(mdxQuery);
            return ConvertCellSetToDataSet(cs);
            //using (var Connection = new AdomdConnection(_biConnectionString))
            //{
            //    Connection.Open();

            //    AdomdCommand command = Connection.CreateCommand();
            //    command.CommandTimeout = 100000;
            //    command.CommandText = mdxQuery;

            //    var reader = command.ExecuteReader();
            //    while(reader.Read())
            //    {

            //    }

            //    Connection.Close();

            //    return null;
            //}
        }

        protected void ExecuteReaderMDX(string mdxQuery, Action<IDataReader> onReaderReady)
        {
            using (var connection = new AdomdConnection(_biConnectionString))
            {
                connection.Open();

                using (AdomdCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = mdxQuery;

                    using (var reader = cmd.ExecuteReader())
                    {
                        onReaderReady(reader);
                        reader.Close();
                    }
                }

                connection.Close();
            }
        }

        protected string GetDateColumns(TimeDimensionType timeDimensionType)
        {
            switch(timeDimensionType)
            {
                case TimeDimensionType.Daily:
                    return String.Format("{0} * {1} * {2}", GetDateColumn(DateColumns.Year), GetDateColumn(DateColumns.MonthOfYear), GetDateColumn(DateColumns.DayOfMonth));
                case TimeDimensionType.Weekly:
                    return String.Format("{0} * {1} * {2}", GetDateColumn(DateColumns.Year), GetDateColumn(DateColumns.MonthOfYear), GetDateColumn(DateColumns.WeekOfMonth));
                case TimeDimensionType.Monthly:
                    return String.Format("{0} * {1}", GetDateColumn(DateColumns.Year), GetDateColumn(DateColumns.MonthOfYear));
                case TimeDimensionType.Yearly:
                    return GetDateColumn(DateColumns.Year);
            }
            return null;
        }
       

        enum DateColumns
        {
            Year,
            MonthOfYear,
            WeekOfMonth,
            DayOfMonth
        }

        string GetDateColumn(DateColumns dateColumn)
        {
            string fieldName = GetDateSmallColumnName(dateColumn);
            
            return string.Format("[Date].[{0}].CHILDREN", fieldName);
        }

        private string GetDateSmallColumnName(DateColumns dateColumn)
        {
            switch(dateColumn)
            {
                case DateColumns.Year: return "Year";
                case DateColumns.MonthOfYear: return "Month Of Year";
                case DateColumns.WeekOfMonth: return "Week Of Month";
                case DateColumns.DayOfMonth: return "Day Of Month";
            }
            return null;
        }

        string ReadDateColumnCaption(DateColumns dateColumn, IDataReader reader)
        {            
            string columnName = string.Format("[Date].[{0}].[{0}].[MEMBER_CAPTION]", GetDateSmallColumnName(dateColumn));
            return reader[columnName] as string;
        }

        string GetMonthDescription(int monthNumber)
        {
            return string.Format("{0: MMM}", new DateTime(2000, monthNumber, 1));
        }

        protected void FillTimeCaptions(BaseTimeDimensionRecord record, IDataReader reader, TimeDimensionType timeDimensionType)
        {
            int year = int.Parse(ReadDateColumnCaption(DateColumns.Year, reader));
            int month = 1;
            int day = 1;
            switch (timeDimensionType)
            {
                case TimeDimensionType.Daily:
                    month = int.Parse(ReadDateColumnCaption(DateColumns.MonthOfYear, reader));
                    day = int.Parse(ReadDateColumnCaption(DateColumns.DayOfMonth, reader));
                    record.TimeGroupName = String.Format("{0}-{1}", GetMonthDescription(month), year);
                    record.TimeValue = String.Format("{0}", day);
                    break;
                case TimeDimensionType.Weekly:
                    month = int.Parse(ReadDateColumnCaption(DateColumns.MonthOfYear, reader));
                    record.TimeGroupName = String.Format("{0}-{1}", GetMonthDescription(month), year);
                    record.TimeValue = String.Format("Week {0}", ReadDateColumnCaption(DateColumns.WeekOfMonth, reader));
                    break;
                case TimeDimensionType.Monthly:
                    month = int.Parse(ReadDateColumnCaption(DateColumns.MonthOfYear, reader));
                    record.TimeGroupName = String.Format("{0}", year);
                    record.TimeValue = String.Format("{0}", GetMonthDescription(month));
                    break;
                case TimeDimensionType.Yearly:
                    record.TimeValue = String.Format("{0}", year);
                    break;
            }
            record.Time = new DateTime(year, month, day);
        }
    }
}
