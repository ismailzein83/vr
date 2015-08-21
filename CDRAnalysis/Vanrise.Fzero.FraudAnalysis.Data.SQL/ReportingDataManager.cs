using System;
using System.Data;
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
                ExecuteNonQuerySP("FraudAnalysis.sp_Strategy_CreateTempForFilteredCasesProductivity", tempTableName, input.Query.FromDate, input.Query.ToDate, input.Query.StrategiesList);
            };


            return RetrieveData(input, createTempTableAction, CaseProductivityMapper);
        }

        #region Private Methods

        private CaseProductivity CaseProductivityMapper(IDataReader reader)
        {
            CaseProductivity caseProductivity = new CaseProductivity();
            caseProductivity.ClosedCases = (int)reader["ClosedCases"];
            caseProductivity.ClosedoverGenerated = (float)reader["ClosedoverGenerated"];
            caseProductivity.FraudCases = (int)reader["FraudCases"];
            caseProductivity.GeneratedCases = (int)reader["GeneratedCases"];
            caseProductivity.StrategyName = reader["StrategyName"] as string;
            return caseProductivity;
        }

        #endregion

        
    }
}
