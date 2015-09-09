using System;
using System.Collections.Generic;
using System.Data;
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
        }

        public BigResult<BTSCases> GetBTSCases(Vanrise.Entities.DataRetrievalInput<DashboardResultQuery> input)
        {
            Action<string> createTempTableAction = (tempTableName) =>
            {
                ExecuteNonQuerySP("FraudAnalysis.sp_Dashboard_CreateTempForTopTenBTS", tempTableName, input.Query.FromDate, input.Query.ToDate);
            };
            return RetrieveData(input, createTempTableAction, BTSCasesMapper);
        }


        public BigResult<BTSHighValueCases> GetTop10BTSHighValue(Vanrise.Entities.DataRetrievalInput<DashboardResultQuery> input)
        {
            Action<string> createTempTableAction = (tempTableName) =>
            {
                ExecuteNonQuerySP("FraudAnalysis.sp_Dashboard_CreateTempForTopTenHighValueBTS", tempTableName, input.Query.FromDate, input.Query.ToDate);
            };
            return RetrieveData(input, createTempTableAction, BTSHighValueCasesMapper);
        }

        public BigResult<DailyVolumeLoose> GetDailyVolumeLooses(DataRetrievalInput<DashboardResultQuery> input)
        {
            Action<string> createTempTableAction = (tempTableName) =>
            {
                ExecuteNonQuerySP("FraudAnalysis.sp_Dashboard_CreateTempForDailyVolumeLooses", tempTableName, input.Query.FromDate, input.Query.ToDate);
            };
            return RetrieveData(input, createTempTableAction, DailyVolumeLoosesMapper);
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

        private BTSHighValueCases BTSHighValueCasesMapper(IDataReader reader)
        {
            var bTSHighValueCases = new BTSHighValueCases();
            bTSHighValueCases.Volume = (decimal)reader["Volume"];
            bTSHighValueCases.BTS_Id = GetReaderValue<int?>(reader, "BTS_Id");
            return bTSHighValueCases;
        }


        private DailyVolumeLoose DailyVolumeLoosesMapper(IDataReader reader)
        {
            var dailyVolumeLoose = new DailyVolumeLoose();
            dailyVolumeLoose.DateDay = (DateTime)reader["DateDay"];
            dailyVolumeLoose.Volume = GetReaderValue<decimal>(reader, "Volume");
            return dailyVolumeLoose;
        }

        #endregion

    }
}
