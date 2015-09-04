using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Vanrise.Data.SQL;
using Vanrise.Entities;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data.SQL
{
    public class StrategyExecutionDataManager : BaseSQLDataManager, IStrategyExecutionDataManager
    {

        public StrategyExecutionDataManager()
            : base("CDRDBConnectionString")
        {

        }

        public bool ExecuteStrategy(StrategyExecution strategyExecutionObject, out int insertedId)
        {
            object id;
            int recordesEffected = ExecuteNonQuerySP("FraudAnalysis.sp_StrategyExecution_Insert", out id,
                strategyExecutionObject.ProcessID,
                strategyExecutionObject.StrategyID,
                strategyExecutionObject.FromDate,
                strategyExecutionObject.ToDate,
                strategyExecutionObject.PeriodID,
                DateTime.Now
            );

            if (recordesEffected > 0)
            {
                insertedId = (int)id;
                return true;
            }
            else
            {
                insertedId = 0;
                return false;
            }


        }


        public object FinishDBApplyStream(object dbApplyStream)
        {
            throw new NotImplementedException();
        }

        public object InitialiazeStreamForDBApply()
        {
            throw new NotImplementedException();
        }

        public void WriteRecordToStream(StrategyExecutionDetail record, object dbApplyStream)
        {
            throw new NotImplementedException();
        }
    }
}
