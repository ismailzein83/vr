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

                string Query = " IF NOT OBJECT_ID('" + tempTableName + "', N'U') IS NOT NULL"
                            + " Begin "
                            + " SELECT  "
                            + " temp.StrategyName, sum(temp.GeneratedCases) GeneratedCases, sum(temp.ClosedCases) ClosedCases"
                            + " ,sum(temp.FraudCases) FraudCases, temp.ExecutionDate DateDay   "
                            + " into " + tempTableName 
                            + " from ("
                            + " select s.Name StrategyName, count(sed.Id) as GeneratedCases ,  0 ClosedCases, 0 FraudCases"
                            + (!input.Query.GroupDaily ? " , null as ExecutionDate" : " , CAST(se.ExecutionDate AS DATE)    as ExecutionDate")
                            + " from FraudAnalysis.StrategyExecutionDetails sed with(nolock) "
                            + " inner join FraudAnalysis.StrategyExecution se with(nolock)  on se.ID = sed.StrategyExecutionID"
                            + " inner join [FraudAnalysis].[Strategy] s with(nolock)  on se.StrategyId = s.Id"
                            + " where sed.caseId is null and se.ExecutionDate >= @FromDate and se.ExecutionDate <=  @ToDate "
                            + (input.Query.StrategiesList != "" ? " and se.StrategyId IN (" + input.Query.StrategiesList + ")" : "")
                            + " group by s.Name"
                            + (!input.Query.GroupDaily ? "" : " , CAST(se.ExecutionDate AS DATE) ")
                            + " union"
                            + " SELECT  "
                            + " s.Name StrategyName , 0 as GeneratedCases, Sum(Case when ac.Status = 3 then 1 when ac.Status = 4 then 1 else 0 end) as ClosedCases"
                            + " ,Sum(Case when ac.Status = 3 then 1 else 0 end) as FraudCases"
                            + (!input.Query.GroupDaily ? " , null as ExecutionDate" : " , CAST(se.ExecutionDate AS DATE)    as ExecutionDate")
                            + " FROM [FraudAnalysis].[AccountCase]	ac with(nolock) "
                            + " inner join FraudAnalysis.StrategyExecutionDetails sed with(nolock)  on sed.CaseID = ac.ID"
                            + " inner join FraudAnalysis.StrategyExecution se with(nolock)  on se.ID = sed.StrategyExecutionID"
                            + " inner join [FraudAnalysis].[Strategy] s with(nolock)  on se.StrategyId = s.Id"
                            + " where se.ExecutionDate >= @FromDate and se.ExecutionDate <=  @ToDate "
                            + (input.Query.StrategiesList != "" ? " and se.StrategyId IN (" + input.Query.StrategiesList + ")" : "")
                            + " Group by s.Name "
                            + (!input.Query.GroupDaily ? "" : " , CAST(se.ExecutionDate AS DATE) ")
                            + " ) "
                            + " as temp"
                            + " group by temp.StrategyName"
                            + " ,temp.ExecutionDate"
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


                string Query =    " IF NOT OBJECT_ID('" + tempTableName + "', N'U') IS NOT NULL"
                                + " Begin "
                                + " SELECT s.Name StrategyName , Count(distinct ac.AccountNumber) as BlockedLinesCount"
                                + (!input.Query.GroupDaily ? ",null DateDay" : " , CAST(se.ExecutionDate AS DATE) DateDay ")
                                + " , STUFF(( "
                                + " SELECT ', ' + ac1.[AccountNumber]  "
                                + " FROM [FraudAnalysis].[AccountCase]  ac1"
                                + " inner join FraudAnalysis.StrategyExecutionDetails sed1 on sed1.CaseID=ac1.ID"
                                + " inner join FraudAnalysis.StrategyExecution se1 on se1.ID = sed1.StrategyExecutionID"
                                + " inner join [FraudAnalysis].[Strategy] s1 on se1.StrategyId = s1.Id "
                                + " WHERE Status=3  "
                                + (!input.Query.GroupDaily ? "" : " and  CAST(se1.ExecutionDate AS DATE) =  CAST(se.ExecutionDate AS DATE) ")
                                + " and   se1.StrategyId= StrategyId  "
                                + " FOR XML PATH(''),TYPE).value('(./text())[1]','VARCHAR(MAX)')  ,1,2,''    ) AS AccountNumbers  "
                                + " into " + tempTableName
                                + " FROM [FraudAnalysis].[AccountCase]	ac "
                                + " with(nolock) "
                                + " inner join FraudAnalysis.StrategyExecutionDetails sed on sed.CaseID=ac.ID"
                                + " inner join FraudAnalysis.StrategyExecution se on se.ID = sed.StrategyExecutionID"
                                + " inner join [FraudAnalysis].[Strategy] s on se.StrategyId = s.Id"
                                + " Where se.ExecutionDate >= @FromDate and  se.ExecutionDate <= @ToDate and ac.Status=3"
                                + (input.Query.StrategiesList != "" ? " and se.StrategyId IN (" + input.Query.StrategiesList + ")" : "")
                                + " Group by s.Name, se.StrategyId"
                                + (!input.Query.GroupDaily ? "" : " , CAST(se.ExecutionDate AS DATE) ")
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
                             + " SELECT ac.AccountNumber, SUM(cdr.DurationInSeconds)/60 AS Volume, COUNT(distinct ac.ID) AS GeneratedCases, 'Fraud' AS ReasonofBlocking, count(distinct cast( cdr.connectdatetime as date)) AS ActiveDays "
                             + " , Count(distinct ac.AccountNumber) as BlockedLinesCount"
                             + " into " + tempTableName
                             + " FROM   FraudAnalysis.AccountCase AS ac WITH (nolock) INNER JOIN"
                             + " FraudAnalysis.NormalCDR AS cdr WITH (nolock, INDEX = IX_NormalCDR_MSISDN) ON ac.AccountNumber = cdr.MSISDN"
                             + " Where ac.Status = 3 and cdr.Call_Type = 1 and ac.CreatedTime BETWEEN @FromDate and  @ToDate"
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
            
            caseProductivity.ClosedoverGenerated = ((decimal)caseProductivity.GeneratedCases != (decimal)0) ? 
                Convert.ToDecimal(caseProductivity.ClosedCases)/Convert.ToDecimal(caseProductivity.GeneratedCases) : 0;
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
