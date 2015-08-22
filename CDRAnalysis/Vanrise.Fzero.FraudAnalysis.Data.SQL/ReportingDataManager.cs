using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Vanrise.Data.SQL;
using Vanrise.Entities;
using Vanrise.Fzero.FraudAnalysis.Entities;
using System.Linq;

namespace Vanrise.Fzero.FraudAnalysis.Data.SQL
{
    public class ReportingDataManager : BaseSQLDataManager, IReportingDataManager
    {

        public ReportingDataManager()
            : base("CDRDBConnectionString")
        {

        }


        public BigResult<CaseProductivity> GetFilteredCasesProductivity(Vanrise.Entities.DataRetrievalInput<CaseProductivityResultQuery> input)
        {

            Action<string> createTempTableAction = (tempTableName) =>
            {

                string Query = " IF NOT OBJECT_ID('"+tempTableName+"', N'U') IS NOT NULL" 
                             + " Begin "
                             + " SELECT s.Name StrategyName"
                             + " , Sum(Case when ac.StatusId = 1 then 1 else 0 end) as GeneratedCases"
                             + " , Sum(Case when ac.StatusId = 3 then 1 when ac.StatusId = 4 then 1 else 0 end) as ClosedCases "
                             + " , Sum(Case when ac.StatusId = 3 then 1 else 0 end) as FraudCases"
                             + (!input.Query.GroupDaily ? " , null as DateDay" : " , CAST(ac.LogDate AS DATE)    as DateDay")
                             + " into " + tempTableName 
                             + " FROM [FraudAnalysis].[AccountCase]	ac with(nolock, index=IX_AccountCase_LogDate) inner join [FraudAnalysis].[Strategy] s"
                             + " on ac.StrategyId = s.Id"
                             + " Where ac.LogDate BETWEEN @FromDate and  @ToDate"
                             + (input.Query.StrategiesList != "" ? " and ac.StrategyId IN (" + input.Query.StrategiesList + ")" : "")
                             + " Group by s.Name"
                             + (!input.Query.GroupDaily ? "" : " , CAST(ac.LogDate AS DATE) ")
                             + " End";



                ExecuteNonQueryText(Query, (cmd) =>
                {
                    cmd.Parameters.Add(new SqlParameter("@FromDate", input.Query.FromDate));
                    cmd.Parameters.Add(new SqlParameter("@ToDate", input.Query.ToDate));
                });
            };


            return RetrieveData(input, createTempTableAction, CaseProductivityMapper);
        }


        public BigResult<BlockedLines> GetFilteredBlockedLines(DataRetrievalInput<BlockedLinesResultQuery> input)
        {

            Action<string> createTempTableAction = (tempTableName) =>
            {

                string Query = " IF NOT OBJECT_ID('" + tempTableName + "', N'U') IS NOT NULL"
                             + " Begin "
                             + " SELECT s.Name StrategyName"
                             + " , Count(distinct ac.AccountNumber) as BlockedLinesCount"
                             + (!input.Query.GroupDaily ? " , null as DateDay" : " , CAST(ac.LogDate AS DATE)    as DateDay")

                             + (!input.Query.GroupDaily ? " , '' as AccountNumbers" : " , STUFF(( SELECT ', ' + [AccountNumber]  FROM [FraudAnalysis].[AccountCase] WHERE StatusId=3 and  (CAST(LogDate AS DATE) =  CAST(ac.LogDate AS DATE) and   ac.StrategyId= StrategyId )   FOR XML PATH(''),TYPE).value('(./text())[1]','VARCHAR(MAX)')  ,1,2,''    ) AS AccountNumbers  ")
                             + " into " + tempTableName
                             + " FROM [FraudAnalysis].[AccountCase]	ac with(nolock, index=IX_AccountCase_LogDate) inner join [FraudAnalysis].[Strategy] s"
                             + " on ac.StrategyId = s.Id"
                             + " Where ac.LogDate BETWEEN @FromDate and  @ToDate and ac.StatusId=3"
                             + (input.Query.StrategiesList != "" ? " and ac.StrategyId IN (" + input.Query.StrategiesList + ")" : "")
                             + " Group by s.Name, ac.StrategyId"
                             + (!input.Query.GroupDaily ? "" : " , CAST(ac.LogDate AS DATE) ")
                             + " End";



                ExecuteNonQueryText(Query, (cmd) =>
                {
                    cmd.Parameters.Add(new SqlParameter("@FromDate", input.Query.FromDate));
                    cmd.Parameters.Add(new SqlParameter("@ToDate", input.Query.ToDate));
                });
            };


            return RetrieveData(input, createTempTableAction, BlockedLinesMapper);
        }


        public BigResult<LinesDetected> GetFilteredLinesDetected(DataRetrievalInput<LinesDetectedResultQuery> input)
        {

            Action<string> createTempTableAction = (tempTableName) =>
            {

                string Query = " IF NOT OBJECT_ID('" + tempTableName + "', N'U') IS NOT NULL"
                             + " Begin "
                             + " SELECT ac.AccountNumber, SUM(cdr.DurationInSeconds) AS Volume, COUNT(ac.ID) AS GeneratedCases, 'Fraud' AS ReasonofBlocking, 0 AS ActiveDays "
                             + " , Count(distinct ac.AccountNumber) as BlockedLinesCount"
                             + " into " + tempTableName
                             + " FROM   FraudAnalysis.AccountCase AS ac WITH (nolock, INDEX = IX_AccountCase_AccountNumber) INNER JOIN"
                             + " FraudAnalysis.NormalCDR AS cdr WITH (nolock, INDEX = IX_NormalCDR_MSISDN) ON ac.AccountNumber = cdr.MSISDN"
                             + " Where ac.StatusID = 3 and cdr.Call_Type = 1 and cdr.ConnectDateTime BETWEEN @FromDate and  @ToDate"
                             + " GROUP BY ac.AccountNumber "
                             + " End";



                ExecuteNonQueryText(Query, (cmd) =>
                {
                    cmd.Parameters.Add(new SqlParameter("@FromDate", input.Query.FromDate));
                    cmd.Parameters.Add(new SqlParameter("@ToDate", input.Query.ToDate));
                });
            };


            return RetrieveData(input, createTempTableAction, LinesDetectedMapper);
        }


        #region Private Methods

        private CaseProductivity CaseProductivityMapper(IDataReader reader)
        {
            CaseProductivity caseProductivity = new CaseProductivity();
            caseProductivity.ClosedCases = (int)reader["ClosedCases"];
            caseProductivity.GeneratedCases = (int)reader["GeneratedCases"];
            caseProductivity.ClosedoverGenerated = Convert.ToDecimal(caseProductivity.ClosedCases)/Convert.ToDecimal(caseProductivity.GeneratedCases);
            caseProductivity.FraudCases = (int)reader["FraudCases"];
            caseProductivity.StrategyName = reader["StrategyName"] as string;
            caseProductivity.DateDay = GetReaderValue<DateTime?>(reader , "DateDay");
            return caseProductivity;
        }



        private BlockedLines BlockedLinesMapper(IDataReader reader)
        {
            BlockedLines blockedLines = new BlockedLines();
            blockedLines.BlockedLinesCount = (int)reader["BlockedLinesCount"];
            blockedLines.StrategyName = reader["StrategyName"] as string;
            blockedLines.DateDay = GetReaderValue<DateTime?>(reader, "DateDay");
            blockedLines.AccountNumbers = GetReaderValue<string>(reader, "AccountNumbers").Split(',').ToList();
            return blockedLines;
        }

        private LinesDetected LinesDetectedMapper(IDataReader reader)
        {
            LinesDetected linesDetected = new LinesDetected();
            linesDetected.AccountNumber = reader["AccountNumber"] as string;
            linesDetected.ReasonofBlocking = reader["ReasonofBlocking"] as string;
            linesDetected.ActiveDays = (int) reader["ActiveDays"] ;
            linesDetected.GeneratedCases = (int)reader["GeneratedCases"];
            linesDetected.Volume = (decimal)reader["Volume"];
            return linesDetected;
        }

        #endregion
       
    }
}
