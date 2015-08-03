using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Vanrise.Data.SQL;
using Vanrise.Entities;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data.SQL
{
    public class DashboardManager : BaseSQLDataManager, IDashboardManager
    {

        public DashboardManager()
            : base("CDRDBConnectionString")
        {

        }
               

        public List<StrategyCases> GetStrategyCases(DateTime fromDate, DateTime toDate)
        {
            return GetItemsSP("FraudAnalysis.sp_Dashboard_GetFraudCasesPerStrategy", StrategyCasesMapper, fromDate, toDate);
        }


        public BigResult<CasesSummary> GetCasesSummary(Vanrise.Entities.DataRetrievalInput<DashboardResultQuery> input)
        {
            Action<string> createTempTableAction = (tempTableName) =>
            {
                ExecuteNonQuerySP("FraudAnalysis.sp_Dashboard_CreateTempForCasesSummary", tempTableName, input.Query.FromDate, input.Query.ToDate);
            };
            return RetrieveData(input, createTempTableAction, CasesSummaryMapper);
            //return GetItemsSP("FraudAnalysis.sp_Dashboard_GetCasesSummary", CasesSummaryMapper, fromDate, toDate);
        }

               


        public BigResult<BTSCases> GetBTSCases(Vanrise.Entities.DataRetrievalInput<DashboardResultQuery> input)
        {
            Action<string> createTempTableAction = (tempTableName) =>
            {
                ExecuteNonQuerySP("FraudAnalysis.sp_Dashboard_CreateTempForTopTenBTS", tempTableName, input.Query.FromDate, input.Query.ToDate);
            };
            return RetrieveData(input, createTempTableAction, BTSCasesMapper);
            //return GetItemsSP("FraudAnalysis.sp_Dashboard_GetTopTenBTS", BTSCasesMapper, fromDate, toDate);
        }


        public BigResult<CellCases> GetCellCases(Vanrise.Entities.DataRetrievalInput<DashboardResultQuery> input)
        {
            Action<string> createTempTableAction = (tempTableName) =>
            {
                ExecuteNonQuerySP("FraudAnalysis.sp_Dashboard_CreateTempForTopTenCell", tempTableName, input.Query.FromDate, input.Query.ToDate);
            };
            return RetrieveData(input, createTempTableAction, CellCasesMapper);
            //return GetItemsSP("FraudAnalysis.sp_Dashboard_GetTopTenCell", CellCasesMapper, fromDate, toDate);
        }




        #region Private Methods


        private CasesSummary CasesSummaryMapper(IDataReader reader)
        {
            var casesSummary = new CasesSummary();
            casesSummary.CountCases = (int)reader["CountCases"];
            casesSummary.StatusName = reader["StatusName"] as string;
            return casesSummary;
        }


        private StrategyCases StrategyCasesMapper(IDataReader reader)
        {
            var strategyCases = new StrategyCases();
            strategyCases.CountCases = (int)reader["CountCases"];
            strategyCases.StrategyName = reader["StrategyName"] as string;
            return strategyCases;
        }

        private BTSCases BTSCasesMapper(IDataReader reader)
        {
            var bTSCases = new BTSCases();
            bTSCases.CountCases = (int)reader["CountCases"];
            bTSCases.BTS_Id =  GetReaderValue<int?>(reader,"BTS_Id");
            return bTSCases;
        }

        private CellCases CellCasesMapper(IDataReader reader)
        {
            var cellCases = new CellCases();
            cellCases.CountCases = (int)reader["CountCases"];
            cellCases.Cell_Id = reader["Cell_Id"] as string;
            return cellCases;
        }

          


        #endregion



    }
}
