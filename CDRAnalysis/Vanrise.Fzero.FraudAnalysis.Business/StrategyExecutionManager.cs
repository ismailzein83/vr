﻿using System;
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

        public void OverrideStrategyExecution(List<int> strategyIDs, DateTime from, DateTime to)
        {
            IStrategyExecutionDataManager strategyExecutionDataManager = FraudDataManagerFactory.GetDataManager<IStrategyExecutionDataManager>();
            ICaseManagementDataManager caseManagementDataManager = FraudDataManagerFactory.GetDataManager<ICaseManagementDataManager>();

            foreach (var strategyID in strategyIDs)
            {
                strategyExecutionDataManager.OverrideStrategyExecution(strategyID, from, to);
            }


            List<int> CaseIDs = strategyExecutionDataManager.GetCasesIDsofStrategyExecutionDetails(null, from, to, strategyIDs);

            if (CaseIDs != null && CaseIDs.Count > 0)
            {
                strategyExecutionDataManager.DeleteStrategyExecutionDetails_ByFilters(null, from, to, strategyIDs);

                caseManagementDataManager.DeleteAccountCases_ByCaseIDs(CaseIDs);
            }
        }


        public BigResult<StrategyExecutionItem> GetFilteredStrategyExecutions(Vanrise.Entities.DataRetrievalInput<StrategyExecutionQuery> input)
        {
            IStrategyExecutionDataManager strategyExecutionDataManager = FraudDataManagerFactory.GetDataManager<IStrategyExecutionDataManager>();
            StrategyManager strategyManager = new StrategyManager();
            Strategy strategy = new Strategy();
            BigResult<StrategyExecution> bigResultItems = strategyExecutionDataManager.GetFilteredStrategyExecutions(input);
            List<StrategyExecution> executions = bigResultItems.Data.ToList();

            BigResult<StrategyExecutionItem> rsltResultItems = new BigResult<StrategyExecutionItem>();
            List<StrategyExecutionItem> items = new List<StrategyExecutionItem>();
            List<long> pcocessIds = new List<long>();

            foreach (var i in executions)
                pcocessIds.Add(i.ProcessID);


            BPClient bpClient = new BPClient();
            Dictionary<long, BPInstanceStatus> processInstances = bpClient.GetProcessesStatuses(pcocessIds);

            foreach (var i in executions)
            {
                StrategyExecutionItem item = new StrategyExecutionItem();
                item.Entity = i;
                BPInstanceStatus status;
                if (processInstances.TryGetValue(i.ProcessID, out status))
                    item.Status = status;

                strategy = strategyManager.GetStrategyById(i.StrategyID);
                if (strategy != null)
                    item.StrategyName = strategy.Name;

                item.PeriodName = Vanrise.Common.Utilities.GetEnumDescription<PeriodEnum>((PeriodEnum)i.PeriodID);

                items.Add(item);
            }

            rsltResultItems.Data = (IEnumerable<StrategyExecutionItem>)items;
            rsltResultItems.ResultKey = bigResultItems.ResultKey;
            rsltResultItems.TotalCount = bigResultItems.TotalCount;
            return rsltResultItems;
        }

    }
}
