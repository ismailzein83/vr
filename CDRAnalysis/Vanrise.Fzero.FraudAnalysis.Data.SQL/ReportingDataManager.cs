using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Vanrise.Data.SQL;
using Vanrise.Entities;
using Vanrise.Fzero.FraudAnalysis.Entities;
using System.Linq;
using System.Collections.Generic;

namespace Vanrise.Fzero.FraudAnalysis.Data.SQL
{
    public class ReportingDataManager : BaseSQLDataManager, IReportingDataManager
    {
        public ReportingDataManager()
            : base("CDRDBConnectionString")
        {

        }

        #region Get Filtered Cases Productivity

        public BigResult<CaseProductivity> GetFilteredCasesProductivity(DataRetrievalInput<CaseProductivityResultQuery> input)
        {

            Action<string> createTempTableAction = (tempTableName) =>
            {
                ExecuteNonQueryText(CreateTempTableIfNotExists(tempTableName, input.Query.FromDate, input.Query.ToDate, input.Query.GroupDaily, input.Query.StrategyIDs), (cmd) => { });
            };

            return RetrieveData(input, createTempTableAction, CaseProductivityMapper);
        }

        private string CreateTempTableIfNotExists(string tempTableName, DateTime? fromDate, DateTime? toDate, bool groupDaily, List<int> strategyIDs)
        {
            StringBuilder query = new StringBuilder(@"
                IF NOT OBJECT_ID('#TEMP_TABLE_NAME#', N'U') IS NOT NULL
                BEGIN
                
                SELECT  0 ClosedoverGenerated,
                        s.Name StrategyName , Count(ac.ID)  as GeneratedCases, Sum(Case when ac.Status = 3 then 1 when ac.Status = 4 then 1 else 0 end) as ClosedCases
                        ,Sum(Case when ac.Status = 3 then 1 else 0 end) as FraudCases  #Select_CLAUSE#
                        		
		                INTO #TEMP_TABLE_NAME#
		
		                FROM [FraudAnalysis].[AccountCase]	ac with(nolock) 
                        inner join FraudAnalysis.StrategyExecutionDetails sed with(nolock)  on sed.CaseID = ac.ID
                        inner join FraudAnalysis.StrategyExecution se with(nolock)  on se.ID = sed.StrategyExecutionID
                        inner join [FraudAnalysis].[Strategy] s with(nolock)  on se.StrategyId = s.Id
		
                        #WHERE_CLAUSE#
		
		                #GroupBy_CLAUSE#

                        END
            ");

            query.Replace("#GroupBy_CLAUSE#", GetGroupByClause(groupDaily));
            query.Replace("#Select_CLAUSE#", GetSelectClause(groupDaily));
            query.Replace("#TEMP_TABLE_NAME#", tempTableName);
            query.Replace("#WHERE_CLAUSE#", GetWhereClause(fromDate, toDate, groupDaily, strategyIDs));

            return query.ToString();
        }

        private string GetSelectClause(bool GroupDaily)
        {
            StringBuilder selectClause = new StringBuilder();

            if (GroupDaily)
                selectClause.Append(" , CAST(se.ExecutionDate AS DATE)    as DateDay");
            else
                selectClause.Append(" , null as DateDay");

            return selectClause.ToString();
        }

        private string GetGroupByClause(bool GroupDaily)
        {
            StringBuilder groupByClause = new StringBuilder();

            groupByClause.AppendFormat("Group by s.Name");

            if (GroupDaily)
                groupByClause.Append(" , CAST(se.ExecutionDate AS DATE)");

            return groupByClause.ToString();
        }

        private string GetWhereClause(DateTime? fromDate, DateTime? toDate, bool groupDaily, List<int> strategyIDs)
        {
            StringBuilder whereClause = new StringBuilder();

            whereClause.Append("where 1=1 ");

            if (fromDate != null)
                whereClause.Append(" AND se.ExecutionDate >= '" + fromDate + "'");

            if (toDate != null)
                whereClause.Append(" AND se.ExecutionDate <= '" + toDate + "'");

            if (strategyIDs != null)
                whereClause.Append(" and se.StrategyId IN (" + String.Join(",", strategyIDs) + ")");

            return whereClause.ToString();
        }

        #endregion

        #region Get Filtered Blocked Lines

        public BigResult<BlockedLines> GetFilteredBlockedLines(DataRetrievalInput<BlockedLinesResultQuery> input)
        {
            Action<string> createTempTableAction = (tempTableName) =>
            {
                ExecuteNonQueryText(CreateTempTableIfNotExistsForFilteredBlockedLines(tempTableName, input.Query.GroupDaily, input.Query.StrategyIDs), (cmd) =>
                {
                    cmd.Parameters.Add(new SqlParameter("@FromDate", input.Query.FromDate));
                    cmd.Parameters.Add(new SqlParameter("@ToDate", input.Query.ToDate));
                });
            };

            return RetrieveData(input, createTempTableAction, BlockedLinesMapper);
        }

        private string CreateTempTableIfNotExistsForFilteredBlockedLines(string tempTableName, bool groupDaily, List<int> strategyIDs)
        {
            StringBuilder query = new StringBuilder(@"
                IF NOT OBJECT_ID('#TEMP_TABLE_NAME#', N'U') IS NOT NULL
                BEGIN
                    SELECT s.Name AS StrategyName,
                    COUNT(DISTINCT ac.AccountNumber) AS BlockedLinesCount,
                    #DATE_DAY#
                    STUFF((
                        SELECT ', ' + ac1.[AccountNumber]
                        
                        FROM [FraudAnalysis].[AccountCase] ac1
                        INNER JOIN FraudAnalysis.StrategyExecutionDetails sed1 ON sed1.CaseID = ac1.ID
                        INNER JOIN FraudAnalysis.StrategyExecution se1 ON se1.ID = sed1.StrategyExecutionID
                        INNER JOIN [FraudAnalysis].[Strategy] s1 ON se1.StrategyId = s1.Id
                        
                        #INNER_WHERE_CLAUSE#
                        
                        FOR XML PATH(''),TYPE
                    ).value('(./text())[1]','VARCHAR(MAX)'),1,2,'') AS AccountNumbers

                    INTO #TEMP_TABLE_NAME#

                    FROM [FraudAnalysis].[AccountCase] ac WITH(NOLOCK)
                    INNER JOIN FraudAnalysis.StrategyExecutionDetails sed ON sed.CaseID=ac.ID
                    INNER JOIN FraudAnalysis.StrategyExecution se ON se.ID = sed.StrategyExecutionID
                    INNER JOIN [FraudAnalysis].[Strategy] s ON se.StrategyId = s.Id
                    
                    #OUTER_WHERE_CLAUSE#
                    
                    #GROUP_BY_CLAUSE#
                END
            ");

            query.Replace("#TEMP_TABLE_NAME#", tempTableName);
            query.Replace("#DATE_DAY#", (!groupDaily) ? "NULL AS DateDay," : "CAST(se.ExecutionDate AS DATE) AS DateDay,");
            query.Replace("#INNER_WHERE_CLAUSE#", GetInnerWhereClauseFilteredBlockedLines(groupDaily));
            query.Replace("#OUTER_WHERE_CLAUSE#", GetOuterWhereClauseFilteredBlockedLines(strategyIDs));
            query.Replace("#GROUP_BY_CLAUSE#", GetGroupByClauseFilteredBlockedLines(groupDaily));

            return query.ToString();
        }

        private string GetInnerWhereClauseFilteredBlockedLines(bool groupDaily)
        {
            StringBuilder whereClause = new StringBuilder();

            whereClause.Append("WHERE Status = 3");
            whereClause.Append(" AND se1.StrategyId = StrategyId");
            whereClause.Append((!groupDaily) ? null : " AND CAST(se1.ExecutionDate AS DATE) = CAST(se.ExecutionDate AS DATE)");

            return whereClause.ToString();
        }

        private string GetOuterWhereClauseFilteredBlockedLines(List<int> strategyIDs)
        {
            StringBuilder whereClause = new StringBuilder();

            whereClause.Append("WHERE se.ExecutionDate >= @FromDate");
            whereClause.Append(" AND se.ExecutionDate <= @ToDate");
            whereClause.Append(" AND ac.Status = 3");
            
            if (strategyIDs != null && strategyIDs.Count > 0)
                whereClause.Append(" AND se.StrategyId IN (" + string.Join(",", strategyIDs) + ")");

            return whereClause.ToString();
        }

        private string GetGroupByClauseFilteredBlockedLines(bool groupDaily)
        {
            StringBuilder groupByClause = new StringBuilder();

            groupByClause.Append("GROUP BY s.Name, se.StrategyId");
            groupByClause.Append((!groupDaily) ? null : ", CAST(se.ExecutionDate AS DATE)");

            return groupByClause.ToString();
        }

        #endregion

        #region Get Filtered Lines Detected

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

        #endregion

        #region Private Methods

        private CaseProductivity CaseProductivityMapper(IDataReader reader)
        {
            CaseProductivity caseProductivity = new CaseProductivity();
            caseProductivity.ClosedCases = (int)reader["ClosedCases"];
            caseProductivity.GeneratedCases = (int)reader["GeneratedCases"];

            caseProductivity.ClosedoverGenerated = ((decimal)caseProductivity.GeneratedCases != (decimal)0) ?
                Convert.ToDecimal(caseProductivity.ClosedCases) / Convert.ToDecimal(caseProductivity.GeneratedCases) : 0;
            caseProductivity.FraudCases = (int)reader["FraudCases"];
            caseProductivity.StrategyName = reader["StrategyName"] as string;
            caseProductivity.DateDay = GetReaderValue<DateTime?>(reader, "DateDay");
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
            linesDetected.ActiveDays = (int)reader["ActiveDays"];
            linesDetected.GeneratedCases = (int)reader["GeneratedCases"];
            linesDetected.Volume = (decimal)reader["Volume"];
            return linesDetected;
        }

        #endregion
    }
}
