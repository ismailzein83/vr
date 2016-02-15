﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Vanrise.Data.SQL;
using Vanrise.Entities;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data.SQL
{
    public class ReportingDataManager : BaseSQLDataManager, IReportingDataManager
    {
        public ReportingDataManager()
            : base("CDRDBConnectionString")
        {

        }

        #region Cases Productivity

        public BigResult<CaseProductivity> GetFilteredCasesProductivity(DataRetrievalInput<CaseProductivityQuery> input)
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
                    SELECT 0 ClosedoverGenerated,
                        s.Name AS StrategyName,
                        SUM(CASE WHEN ac.Status = 1 THEN 1 ELSE 0 END) AS GeneratedCases,
                        SUM(CASE WHEN ac.Status = 3 OR ac.Status = 4 THEN 1 ELSE 0 END) AS ClosedCases,
                        SUM(CASE WHEN ac.Status = 3 THEN 1 ELSE 0 END) AS FraudCases
                        #SELECT_CLAUSE#
                        
		            INTO #TEMP_TABLE_NAME#
		            
		            FROM [FraudAnalysis].[AccountCase] ac WITH(NOLOCK)
                    INNER JOIN FraudAnalysis.StrategyExecutionItem sed WITH(NOLOCK) ON sed.CaseID = ac.ID
                    INNER JOIN FraudAnalysis.StrategyExecution se WITH(NOLOCK) ON se.ID = sed.StrategyExecutionID
                    INNER JOIN [FraudAnalysis].[Strategy] s WITH(NOLOCK) ON se.StrategyID = s.ID
		            
                    #WHERE_CLAUSE#
		            
		            #GROUP_BY_CLAUSE#
                END
            ");

            query.Replace("#TEMP_TABLE_NAME#", tempTableName);
            query.Replace("#SELECT_CLAUSE#", GetSelectClause(groupDaily));
            query.Replace("#WHERE_CLAUSE#", GetWhereClause(fromDate, toDate, groupDaily, strategyIDs));
            query.Replace("#GROUP_BY_CLAUSE#", GetGroupByClause(groupDaily));

            return query.ToString();
        }

        private string GetSelectClause(bool GroupDaily)
        {
            StringBuilder selectClause = new StringBuilder();

            if (GroupDaily)
                selectClause.Append(", CAST(se.ExecutionDate AS DATE) AS DateDay");
            else
                selectClause.Append(", NULL AS DateDay");

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

            if (strategyIDs != null && strategyIDs.Count > 0)
                whereClause.Append(" and se.StrategyID IN (" + string.Join(",", strategyIDs) + ")");

            return whereClause.ToString();
        }

        #endregion

        #region Blocked Lines

        public BigResult<BlockedLines> GetFilteredBlockedLines(DataRetrievalInput<BlockedLinesQuery> input)
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
                        SELECT distinct ', ' + ac1.[AccountNumber]
                        
                        FROM [FraudAnalysis].[AccountCase] ac1
                        INNER JOIN FraudAnalysis.StrategyExecutionItem sed1 ON sed1.CaseID = ac1.ID
                        INNER JOIN FraudAnalysis.StrategyExecution se1 ON se1.ID = sed1.StrategyExecutionID
                        INNER JOIN [FraudAnalysis].[Strategy] s1 ON se1.StrategyID = s1.ID
                        
                        #INNER_WHERE_CLAUSE#
                        
                        FOR XML PATH(''),TYPE
                    ).value('(./text())[1]','VARCHAR(MAX)'),1,2,'') AS AccountNumbers

                    INTO #TEMP_TABLE_NAME#

                    FROM [FraudAnalysis].[AccountCase] ac WITH(NOLOCK)
                    INNER JOIN FraudAnalysis.StrategyExecutionItem sed ON sed.CaseID=ac.ID
                    INNER JOIN FraudAnalysis.StrategyExecution se ON se.ID = sed.StrategyExecutionID
                    INNER JOIN [FraudAnalysis].[Strategy] s ON se.StrategyID = s.ID
                    
                    #OUTER_WHERE_CLAUSE#
                    
                    #GROUP_BY_CLAUSE#
                END
            ");

            query.Replace("#TEMP_TABLE_NAME#", tempTableName);
            query.Replace("#DATE_DAY#", (!groupDaily) ? "NULL AS DateDay," : "CAST(ac.StatusUpdatedTime AS DATE) AS DateDay,");
            query.Replace("#INNER_WHERE_CLAUSE#", GetInnerWhereClauseFilteredBlockedLines(groupDaily));
            query.Replace("#OUTER_WHERE_CLAUSE#", GetOuterWhereClauseFilteredBlockedLines(strategyIDs));
            query.Replace("#GROUP_BY_CLAUSE#", GetGroupByClauseFilteredBlockedLines(groupDaily));

            return query.ToString();
        }

        private string GetInnerWhereClauseFilteredBlockedLines(bool groupDaily)
        {
            StringBuilder whereClause = new StringBuilder();

            whereClause.Append("WHERE Status = 3");
            whereClause.Append(" AND se1.StrategyID = StrategyID");
            whereClause.Append((!groupDaily) ? null : " AND CAST(ac1.StatusUpdatedTime AS DATE) = CAST(ac.StatusUpdatedTime AS DATE)");

            return whereClause.ToString();
        }

        private string GetOuterWhereClauseFilteredBlockedLines(List<int> strategyIDs)
        {
            StringBuilder whereClause = new StringBuilder();

            whereClause.Append("WHERE ac.StatusUpdatedTime >= @FromDate");
            whereClause.Append(" AND ac.StatusUpdatedTime <= @ToDate");
            whereClause.Append(" AND ac.Status = 3");
            
            if (strategyIDs != null && strategyIDs.Count > 0)
                whereClause.Append(" AND se.StrategyID IN (" + string.Join(",", strategyIDs) + ")");

            return whereClause.ToString();
        }

        private string GetGroupByClauseFilteredBlockedLines(bool groupDaily)
        {
            StringBuilder groupByClause = new StringBuilder();

            groupByClause.Append("GROUP BY s.Name, se.StrategyID");
            groupByClause.Append((!groupDaily) ? null : ", CAST(ac.StatusUpdatedTime AS DATE)");

            return groupByClause.ToString();
        }

        #endregion

        #region Get Filtered Lines Detected
        
        public BigResult<LinesDetected> GetFilteredLinesDetected(DataRetrievalInput<LinesDetectedQuery> input)
        {
            Action<string> createTempTableAction = (tempTableName) =>
            {
                ExecuteNonQuerySP("FraudAnalysis.sp_AccountCase_GetBlockedLines", tempTableName, input.Query.FromDate, input.Query.ToDate);
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
            linesDetected.Occurrences = (int)reader["Occurrences"];
            linesDetected.Volume = (decimal)reader["Volume"];
            return linesDetected;
        }

        #endregion
    }
}
