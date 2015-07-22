using System;
using System.Collections.Generic;
using Vanrise.Fzero.FraudAnalysis.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Business
{
    public class CaseManagmentManager
    {

        public Vanrise.Fzero.FraudAnalysis.Entities.UpdateOperationOutput<SubscriberCase> UpdateStrategy(SubscriberCase subscriberCaseObject)
        {
            ICaseManagementDataManager manager = FraudDataManagerFactory.GetDataManager<ICaseManagementDataManager>();
            bool updateActionSucc = manager.SaveSubscriberCase(subscriberCaseObject);
            Vanrise.Fzero.FraudAnalysis.Entities.UpdateOperationOutput<SubscriberCase> updateOperationOutput = new Vanrise.Fzero.FraudAnalysis.Entities.UpdateOperationOutput<SubscriberCase>();

            if (updateActionSucc)
            {
                updateOperationOutput.Result = Vanrise.Fzero.FraudAnalysis.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = subscriberCaseObject;
            }
            else
                updateOperationOutput.Result = Vanrise.Fzero.FraudAnalysis.Entities.UpdateOperationResult.Failed;
            return updateOperationOutput;
        }

    }
}
