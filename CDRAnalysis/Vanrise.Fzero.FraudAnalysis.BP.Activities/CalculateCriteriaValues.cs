using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess;
using Vanrise.Fzero.FraudAnalysis.Business;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Queueing;
using System.Globalization;
using Vanrise.Fzero.FraudAnalysis.Data;

namespace Vanrise.Fzero.FraudAnalysis.BP.Activities
{
    #region Arguments Classes

    public class CalculateCriteriaValuesInput
    {
        public BaseQueue<NumberProfileBatch> InputQueue { get; set; }

        public BaseQueue<NumberCriteriaBatch> OutputQueue { get; set; }
    }

    #endregion
    public class CalculateCriteriaValues : DependentAsyncActivity<CalculateCriteriaValuesInput>
    {
        #region Arguments

        [RequiredArgument]
        public InArgument<BaseQueue<NumberProfileBatch>> InputQueue { get; set; }

        public InOutArgument<BaseQueue<NumberCriteriaBatch>> OutputQueue { get; set; }

        #endregion

        protected override void DoWork(CalculateCriteriaValuesInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            int BatchSize = int.Parse( System.Configuration.ConfigurationManager.AppSettings["NumberProfileBatchSize"].ToString());
            handle.SharedInstanceData.WriteTrackingMessage(BusinessProcess.Entities.BPTrackingSeverity.Information, "Start SaveCDRsToDB.DoWork.Start {0}", DateTime.Now);
            INumberProfileDataManager dataManager = FraudDataManagerFactory.GetDataManager<INumberProfileDataManager>();

            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.InputQueue.TryDequeue(
                        (item) =>
                        {
                            foreach (NumberProfile number in item.numberProfiles)
                            {
                                inputArgument.OutputQueue.Enqueue(new NumberCriteriaBatch()
                                {
                                    criteriaValues  = new CriteriaManager().GetCriteriaValues(number),
                                    number = number.subscriberNumber
                                });
                            }
                                
                            handle.SharedInstanceData.WriteTrackingMessage(BusinessProcess.Entities.BPTrackingSeverity.Information, "End SaveCDRsToDB.DoWork.SavedtoDB {0}", DateTime.Now);
                        });
                }
                while (!ShouldStop(handle) && hasItem);
            });
            handle.SharedInstanceData.WriteTrackingMessage(BusinessProcess.Entities.BPTrackingSeverity.Information, "End SaveCDRsToDB.DoWork.End {0}", DateTime.Now);

        }

        protected override CalculateCriteriaValuesInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new CalculateCriteriaValuesInput()
            {
                InputQueue = this.InputQueue.Get(context),
                OutputQueue = this.OutputQueue.Get(context)
            };
        }
    }
}
