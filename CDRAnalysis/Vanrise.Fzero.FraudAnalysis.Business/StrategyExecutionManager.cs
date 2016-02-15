using System.Collections.Generic;
using System.Linq;
using Vanrise.Entities;
using Vanrise.Fzero.FraudAnalysis.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Security.Business;
using Vanrise.Security.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Business
{
    public class StrategyExecutionManager
    {
        public InsertOperationOutput<StrategyExecution> ExecuteStrategy(StrategyExecution strategyExecutionObject)
        {
            InsertOperationOutput<StrategyExecution> insertOperationOutput = new InsertOperationOutput<StrategyExecution>();

            int strategyExecutionId = -1;

            IStrategyExecutionDataManager manager = FraudDataManagerFactory.GetDataManager<IStrategyExecutionDataManager>();
            manager.AddStrategyExecution(strategyExecutionObject, out strategyExecutionId);

            strategyExecutionObject.ID = strategyExecutionId;
            insertOperationOutput.InsertedObject = strategyExecutionObject;

            return insertOperationOutput;
        }

        public BigResult<StrategyExecutionDetail> GetFilteredStrategyExecutions(Vanrise.Entities.DataRetrievalInput<StrategyExecutionQuery> input)
        {
            IStrategyExecutionDataManager strategyExecutionDataManager = FraudDataManagerFactory.GetDataManager<IStrategyExecutionDataManager>();
            StrategyManager strategyManager = new StrategyManager();
            Strategy strategy = new Strategy();

            UserManager userManager = new UserManager();
            User executedByName = new User();
            User cancelledByName = new User();

            BigResult<StrategyExecution> bigResultItems = strategyExecutionDataManager.GetFilteredStrategyExecutions(input);
            List<StrategyExecution> executions = bigResultItems.Data.ToList();

            BigResult<StrategyExecutionDetail> detailedBigResultItems = new BigResult<StrategyExecutionDetail>();
            List<StrategyExecutionDetail> details = new List<StrategyExecutionDetail>();

            foreach (var i in executions)
            {
                StrategyExecutionDetail item = new StrategyExecutionDetail();
                item.Entity = i;

                strategy = strategyManager.GetStrategy(i.StrategyID);
                if (strategy != null)
                    item.StrategyName = strategy.Name;

                executedByName = userManager.GetUserbyId(i.ExecutedBy);
                if (executedByName != null)
                    item.ExecutedByName = executedByName.Name;

                if (i.CancelledBy.HasValue)
                {
                    cancelledByName = userManager.GetUserbyId(i.CancelledBy.Value);
                    if (cancelledByName != null)
                        item.CancelledByName = cancelledByName.Name;
                }

                item.PeriodName = Vanrise.Common.Utilities.GetEnumDescription<PeriodEnum>((PeriodEnum)i.PeriodID);
                item.StatusDescription = Vanrise.Common.Utilities.GetEnumDescription<SuspicionOccuranceStatus>((SuspicionOccuranceStatus)i.Status);

                details.Add(item);
            }

            detailedBigResultItems.Data = (IEnumerable<StrategyExecutionDetail>)details;
            detailedBigResultItems.ResultKey = bigResultItems.ResultKey;
            detailedBigResultItems.TotalCount = bigResultItems.TotalCount;
            return detailedBigResultItems;
        }

        public StrategyExecution GetStrategyExecution(long strategyExecutionId)
        {
            IStrategyExecutionDataManager strategyExecutionDataManager = FraudDataManagerFactory.GetDataManager<IStrategyExecutionDataManager>();
            return strategyExecutionDataManager.GetStrategyExecution(strategyExecutionId);
        }


        public bool CloseStrategyExecution(long strategyExecutionId, long numberofSubscribers, long numberofCDRs,long numberofSuspicions,  long executionDuration)
        {
            IStrategyExecutionDataManager strategyExecutionDataManager = FraudDataManagerFactory.GetDataManager<IStrategyExecutionDataManager>();
           
            return strategyExecutionDataManager.CloseStrategyExecution(strategyExecutionId, numberofSubscribers, numberofCDRs,numberofSuspicions,  executionDuration);
        }

    }
}
