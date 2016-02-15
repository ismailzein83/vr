using Microsoft.AnalysisServices.AdomdClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BI.Entities;

namespace Vanrise.BI.Data.SQL
{
    public class BaseDataManager
    {
        string _biConnectionString;
        string _cubeName;
        protected List<BIConfiguration<BIConfigurationMeasure>> _measureDefinitions;
        protected List<BIConfiguration<BIConfigurationEntity>> _entityDefinitions;
        public List<BIConfiguration<BIConfigurationMeasure>> MeasureDefinitions
        {
            set { _measureDefinitions = value; }
        }
        public List<BIConfiguration<BIConfigurationEntity>> EntityDefinitions
        {
            set { _entityDefinitions = value; }
        }
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

        #region Removed

        //private CellSet GetCellSet(string mdxQuery)
        //{
        //    using (var Connection = new AdomdConnection(_biConnectionString))
        //    {
        //        Connection.Open();

        //        AdomdCommand command = Connection.CreateCommand();
        //        command.CommandText = mdxQuery;

        //        CellSet cs = command.ExecuteCellSet();


        //        Connection.Close();

        //        return cs;
        //    }
        //}

        //private DataTable ConvertCellSetToDataSet(CellSet csResult)
        //{
        //    if (csResult == null)
        //    {
        //        return null;
        //    }

        //    DataTable dtResult = new DataTable();
        //    DataColumn dcResult = null;
        //    DataRow drResult = null;
        //    int numberOfDims = 0;
        //    if (csResult.Axes.Count > 1)
        //    {
        //        if (csResult.Axes[1].Positions.Count > 0)
        //        {
        //            Position py = csResult.Axes[1].Positions[0];

        //            int index = 1;
        //            numberOfDims = py.Members.Count;

        //            foreach (Member m in py.Members)
        //            {
        //                dcResult = new DataColumn();
        //                dcResult.ColumnName = m.LevelName;
        //                dtResult.Columns.Add(dcResult);
        //                index++;
        //            }
        //        }
        //    }

        //    String name = null;
        //    foreach (Position pos in csResult.Axes[0].Positions)
        //    {
        //        dcResult = new DataColumn();
        //        name = string.Empty;
        //        foreach (Member m in pos.Members)
        //        {
        //            name += m.Caption + ";";
        //        }
        //        dcResult.ColumnName = name;
        //        dtResult.Columns.Add(dcResult);
        //    }

        //    int y = 0;
        //    int colIndex = 0;
        //    int x = 0;
        //    if (csResult.Axes.Count > 1)
        //    {
        //        foreach (Position py in csResult.Axes[1].Positions)
        //        {
        //            drResult = dtResult.NewRow();
        //            colIndex = 0;
        //            foreach (Member m in py.Members)
        //            {
        //                drResult[colIndex] = m.Caption;
        //                colIndex++;
        //            }

        //            for (x = 0; x < csResult.Axes[0].Positions.Count; x++)
        //            {
        //                drResult[colIndex] = csResult[x, y].FormattedValue;
        //                colIndex++;
        //            }
        //            dtResult.Rows.Add(drResult);
        //            y++;
        //        }
        //    }
        //    return dtResult;
        //}

        //public DataTable GetData(string mdxQuery)
        //{
        //    CellSet cs = GetCellSet(mdxQuery);
        //    return ConvertCellSetToDataSet(cs);
        //    //using (var Connection = new AdomdConnection(_biConnectionString))
        //    //{
        //    //    Connection.Open();

        //    //    AdomdCommand command = Connection.CreateCommand();
        //    //    command.CommandTimeout = 100000;
        //    //    command.CommandText = mdxQuery;

        //    //    var reader = command.ExecuteReader();
        //    //    while(reader.Read())
        //    //    {

        //    //    }

        //    //    Connection.Close();

        //    //    return null;
        //    //}
        //}

        #endregion

        #region ExecuteReader

        protected void ExecuteReaderMDX(string mdxQuery, Action<IDataReader> onReaderReady,params MDXParameter[] parameters)
        {
            using (var connection = new AdomdConnection(_biConnectionString))
            {
                connection.Open();

                using (AdomdCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = mdxQuery;
                    foreach (MDXParameter param in parameters)
                        cmd.Parameters.Add(param.Name,param.Value);
                    using (var reader = cmd.ExecuteReader())
                    {
                        onReaderReady(reader);
                        reader.Close();
                    }
                }

                connection.Close();
            }
        }

        #endregion

        #region Query Builders

        protected string BuildQuery(string columnsPart, string rowsPartValue, string filtersPart, string expressionsPart = null)
        {
            string rowsPart = null;
            if (!String.IsNullOrEmpty(rowsPartValue))
                rowsPart=string.Format(@",{0} ON ROWS", rowsPartValue);
            string query = string.Format(@"select {{{0}}} ON COLUMNS                               
                                    {1} 
                                    FROM (SELECT {3} ON COLUMNS  FROM [{2}])
                                    ", columnsPart, rowsPart, CubeName, filtersPart);

            if (!String.IsNullOrEmpty(expressionsPart))
                return String.Format(@"WITH {0}
                                        {1}", expressionsPart, query);
            else
                return query;
        }

        protected string BuildQueryColumnsPart(params string[] columnNames)
        {
            StringBuilder queryBuilder = null;
            foreach (var columnName in columnNames)
            {
                if (queryBuilder == null)
                    queryBuilder = new StringBuilder();
                else
                    queryBuilder.Append(", ");
                queryBuilder.Append(columnName);
            }
            return queryBuilder.ToString();
        }

        protected string BuildQueryRowsPart(params string[] columnNames)
        {
            StringBuilder queryBuilder = null;
            foreach (var columnName in columnNames)
            {
                if (queryBuilder == null)
                    queryBuilder = new StringBuilder();
                else
                    queryBuilder.Append(" * ");
                queryBuilder.AppendFormat("{0}.CHILDREN", columnName);
            }
            return queryBuilder.ToString();
        }

        protected string BuildQueryFiltersPart(params string[] filters)
        {
            StringBuilder queryBuilder = null;
            foreach (var f in filters)
            {
                if (queryBuilder == null)
                    queryBuilder = new StringBuilder();
                else
                    queryBuilder.Append(", ");
                queryBuilder.Append(f);
            }
            return string.Format("({0})", queryBuilder);
        }

        protected string BuildQueryColumnFilter(List<string> columnName,string columnValue)
        {
            return String.Format("{0}.&[{1}]", columnName, columnValue);
        }

        protected string BuildQueryTopRowsPart(string columnBy, int count, params string[] columnsNames)
        {
            return String.Format(@"TopCount({0}, {1}, {2})", BuildQueryRowsPart(columnsNames), count, columnBy);
        }

        protected string GetRowColumnToRead(string columnName)
        {
            string[] columnParts = columnName.Split('.');
            string smallColumnName = columnParts[columnParts.Length - 1];
            return string.Format("{0}.{1}.[MEMBER_CAPTION]", columnName, smallColumnName);
        }

        #endregion

        #region DateTime dimension
        protected string BuildQueryDateRowColumns(TimeDimensionType timeDimensionType, BIConfigurationTimeEntity configurationTimeEntity)
        {
            switch (timeDimensionType)
            {
                case TimeDimensionType.Yearly:
                    return BuildQueryRowsPart(configurationTimeEntity.Year);
                case TimeDimensionType.Monthly:
                    return BuildQueryRowsPart(configurationTimeEntity.Year, configurationTimeEntity.MonthOfYear);
                case TimeDimensionType.Weekly:
                    return BuildQueryRowsPart(configurationTimeEntity.Year, configurationTimeEntity.MonthOfYear, configurationTimeEntity.WeekOfMonth);
                case TimeDimensionType.Daily:
                    return BuildQueryRowsPart(configurationTimeEntity.Year, configurationTimeEntity.MonthOfYear, configurationTimeEntity.DayOfMonth);
                case TimeDimensionType.Hourly:
                    return BuildQueryRowsPart(configurationTimeEntity.Year, configurationTimeEntity.MonthOfYear, configurationTimeEntity.DayOfMonth, configurationTimeEntity.Hour);
            }
            return null;
        }

        string GetMonthDescription(int monthNumber)
        {
            return string.Format("{0: MMM}", new DateTime(2000, monthNumber, 1));
        }

        protected void FillTimeCaptions(BaseTimeDimensionRecord record, IDataReader reader, TimeDimensionType timeDimensionType, BIConfigurationTimeEntity configurationTimeEntity)
        {
            int year;
            if (!int.TryParse(reader[GetRowColumnToRead(configurationTimeEntity.Year)] as string, out year))
                return;
            int month = 1;
            int day = 1;
            int hour = 0;
            switch (timeDimensionType)
            {
                case TimeDimensionType.Yearly:
                    break;
                case TimeDimensionType.Monthly:
                    if (!int.TryParse(reader[GetRowColumnToRead(configurationTimeEntity.MonthOfYear)] as string, out month))
                        return;
                    break;
                case TimeDimensionType.Weekly:
                    if (!int.TryParse(reader[GetRowColumnToRead(configurationTimeEntity.MonthOfYear)] as string, out month))
                        return;
                    int week;
                    if (!int.TryParse(reader[GetRowColumnToRead(configurationTimeEntity.WeekOfMonth)] as string, out week))
                        return;
                    record.WeekNumber = week;
                    day = (week - 1) * 7 + 1;
                    break;
                case TimeDimensionType.Daily:
                    if (!int.TryParse(reader[GetRowColumnToRead(configurationTimeEntity.MonthOfYear)] as string, out month))
                        return;
                    if (!int.TryParse(reader[GetRowColumnToRead(configurationTimeEntity.DayOfMonth)] as string, out day))
                        return;
                    break;
                case TimeDimensionType.Hourly:
                    if (!int.TryParse(reader[GetRowColumnToRead(configurationTimeEntity.MonthOfYear)] as string, out month))
                        return;
                    if (!int.TryParse(reader[GetRowColumnToRead(configurationTimeEntity.DayOfMonth)] as string, out day))
                        return;
                    if (!int.TryParse(reader[GetRowColumnToRead(configurationTimeEntity.Hour)] as string, out hour))
                        return;
                    break;
            }
            record.Time = new DateTime(year, month, day, hour, 0, 0);
        }

        protected string GetDateFilter(DateTime fromDate, DateTime toDate, BIConfigurationTimeEntity defaultConfigurationTimeEntity)
        {
            return String.Format("Filter({0}.AllMembers, ({0}.CurrentMember.member_caption>='{1:yyyy-MM-dd HH:mm:ss}' And {0}.CurrentMember.member_caption<='{2:yyyy-MM-dd HH:mm:ss} '))", defaultConfigurationTimeEntity.Date, fromDate, toDate);
        }

        #endregion

        
    }
   
}