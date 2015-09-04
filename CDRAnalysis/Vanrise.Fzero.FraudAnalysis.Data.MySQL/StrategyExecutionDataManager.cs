using System;
using System.Collections.Generic;
using Vanrise.Data.MySQL;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data.MySQL
{
    public class StrategyExecutionDataManager : BaseMySQLDataManager, IStrategyExecutionDataManager
    {
        public StrategyExecutionDataManager()
            : base("CDRDBConnectionStringMySQL")
        {

        }


        public bool ExecuteStrategy(StrategyExecution strategyExecutionObject, out int insertedId)
        {
            throw new NotImplementedException();
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


        public void ApplyStrategyExecutionDetailsToDB(object preparedStrategyExecutionDetails)
        {
            throw new NotImplementedException();
        }
      
    }
}
