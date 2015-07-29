using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Vanrise.Data.SQL;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data.SQL
{
    public class DashboardManager : BaseSQLDataManager, IDashboardManager
    {

        public DashboardManager()
            : base("CDRDBConnectionString")
        {

        }

        public List<CasesSummary> GetCasesSummary(DateTime fromDate, DateTime toDate)
        {
            return GetItemsSP("FraudAnalysis.sp_Dashboard_GetCasesSummary", CasesSummaryMapper, fromDate, toDate);
        }


        public List<StrategyCases> GetStrategyCases(DateTime fromDate, DateTime toDate)
        {
            return GetItemsSP("FraudAnalysis.sp_Dashboard_GetFraudCasesPerStrategy", StrategyCasesMapper, fromDate, toDate);
        }


        public List<BTSCases> GetBTSCases(DateTime fromDate, DateTime toDate)
        {
            return GetItemsSP("FraudAnalysis.sp_Dashboard_GetTopTenBTS", BTSCasesMapper, fromDate, toDate);
        }


        public List<CellCases> GetCellCases(DateTime fromDate, DateTime toDate)
        {
            return GetItemsSP("FraudAnalysis.sp_Dashboard_GetTopTenCell", CellCasesMapper, fromDate, toDate);
        }




        #region Private Methods


        private CasesSummary CasesSummaryMapper(IDataReader reader)
        {
            var casesSummary = new CasesSummary();
            casesSummary.CountCase = (int) reader["CountCase"];
            casesSummary.StatusName = reader["StatusName"] as string;
            return casesSummary;
        }


        private StrategyCases StrategyCasesMapper(IDataReader reader)
        {
            var strategyCases = new StrategyCases();
            strategyCases.CountCase = (int)reader["CountCase"];
            strategyCases.StrategyName = reader["StrategyName"] as string;
            return strategyCases;
        }

        private BTSCases BTSCasesMapper(IDataReader reader)
        {
            var bTSCases = new BTSCases();
            bTSCases.CountCase = (int)reader["CountCase"];
            bTSCases.BTS_Id = reader["BTS_Id"] as string;
            return bTSCases;
        }

        private CellCases CellCasesMapper(IDataReader reader)
        {
            var cellCases = new CellCases();
            cellCases.CountCase = (int)reader["CountCase"];
            cellCases.Cell_Id = reader["Cell_Id"] as string;
            return cellCases;
        }

          


        #endregion



    }
}
