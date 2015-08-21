using System;
using System.Data;
using System.Data.SqlClient;
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

        #endregion

        
    }
}
