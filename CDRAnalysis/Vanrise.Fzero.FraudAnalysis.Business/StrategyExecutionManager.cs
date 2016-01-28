using System;
using System.Collections.Generic;
using Vanrise.Entities;
using Vanrise.Fzero.FraudAnalysis.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.BusinessProcess.Client;
using Vanrise.BusinessProcess.Entities;
using System.Linq;
using System.Collections;

namespace Vanrise.Fzero.FraudAnalysis.Business
{
    public class StrategyExecutionManager
    {
        public InsertOperationOutput<StrategyExecution> ExecuteStrategy(StrategyExecution strategyExecutionObject)
        {
            InsertOperationOutput<StrategyExecution> insertOperationOutput = new InsertOperationOutput<StrategyExecution>();

            int strategyExecutionId = -1;

            IStrategyExecutionDataManager manager = FraudDataManagerFactory.GetDataManager<IStrategyExecutionDataManager>();
            manager.ExecuteStrategy(strategyExecutionObject, out strategyExecutionId);

            strategyExecutionObject.ID = strategyExecutionId;
            insertOperationOutput.InsertedObject = strategyExecutionObject;

            return insertOperationOutput;
        }

        public BigResult<StrategyExecutionDetail> GetFilteredStrategyExecutions(Vanrise.Entities.DataRetrievalInput<StrategyExecutionQuery> input)
        {
            IStrategyExecutionDataManager strategyExecutionDataManager = FraudDataManagerFactory.GetDataManager<IStrategyExecutionDataManager>();
            StrategyManager strategyManager = new StrategyManager();
            Strategy strategy = new Strategy();
            BigResult<StrategyExecution> bigResultItems = strategyExecutionDataManager.GetFilteredStrategyExecutions(input);
            List<StrategyExecution> executions = bigResultItems.Data.ToList();

            BigResult<StrategyExecutionDetail> rsltResultItems = new BigResult<StrategyExecutionDetail>();
            List<StrategyExecutionDetail> items = new List<StrategyExecutionDetail>();
            List<long> pcocessIds = new List<long>();

            foreach (var i in executions)
                pcocessIds.Add(i.ProcessID);


            BPClient bpClient = new BPClient();
            Dictionary<long, BPInstanceStatus> processInstances = bpClient.GetProcessesStatuses(pcocessIds);

            foreach (var i in executions)
            {
                StrategyExecutionDetail item = new StrategyExecutionDetail();
                item.Entity = i;
                BPInstanceStatus status;
                if (processInstances.TryGetValue(i.ProcessID, out status))
                    item.Status = status;

                strategy = strategyManager.GetStrategy(i.StrategyID);
                if (strategy != null)
                    item.StrategyName = strategy.Name;

                item.PeriodName = Vanrise.Common.Utilities.GetEnumDescription<PeriodEnum>((PeriodEnum)i.PeriodID);

                items.Add(item);
            }

            rsltResultItems.Data = (IEnumerable<StrategyExecutionDetail>)items;
            rsltResultItems.ResultKey = bigResultItems.ResultKey;
            rsltResultItems.TotalCount = bigResultItems.TotalCount;
            return rsltResultItems;
        }

    }
}
