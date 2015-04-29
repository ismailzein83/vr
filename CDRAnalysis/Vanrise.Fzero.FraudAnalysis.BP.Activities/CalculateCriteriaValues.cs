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
    }

    public class CalculateCriteriaValuesOutput
    {
        public BaseQueue<NumberCriteriaBatch> OutputQueue { get; set; }
    }

    #endregion
    public class CalculateCriteriaValues : BaseAsyncActivity<CalculateCriteriaValuesInput, CalculateCriteriaValuesOutput>
    {
        #region Arguments

        [RequiredArgument]
        public InArgument<BaseQueue<NumberProfileBatch>> InputQueue { get; set; }

        public OutArgument<BaseQueue<NumberCriteriaBatch>> OutputQueue { get; set; }

        #endregion


        //protected override CalculateCriteriaValuesOutput DoWorkWithResult(CalculateCriteriaValuesInput inputArgument, AsyncActivityHandle handle)
        //{
        //    return new CalculateCriteriaValuesOutput
        //    {
        //        value = new CriteriaManager().GetCriteriaValue(inputArgument.criteria,inputArgument.numberProfile)
        //    };
        //}

        //protected override CalculateCriteriaValuesInput GetInputArgument(AsyncCodeActivityContext context)
        //{
        //    return new CalculateCriteriaValuesInput();
        //}

        //protected override void OnWorkComplete(AsyncCodeActivityContext context, CalculateCriteriaValuesOutput result)
        //{
        //    this.value.Set(context, result.value);
        //}



        protected override CalculateCriteriaValuesOutput DoWorkWithResult(CalculateCriteriaValuesInput inputArgument, AsyncActivityHandle handle)
        {


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
                            dataManager.LoadNumberProfile( item.numberProfiles,);
                            handle.SharedInstanceData.WriteTrackingMessage(BusinessProcess.Entities.BPTrackingSeverity.Information, "End SaveCDRsToDB.DoWork.SavedtoDB {0}", DateTime.Now);
                        });
                }
                while (!ShouldStop(handle) && hasItem);
            });
            handle.SharedInstanceData.WriteTrackingMessage(BusinessProcess.Entities.BPTrackingSeverity.Information, "End SaveCDRsToDB.DoWork.End {0}", DateTime.Now);



            //return new CalculateCriteriaValuesOutput
            //{
            //    value = new CriteriaManager().GetCriteriaValues(inputArgument.InputQueue.)
            //};
        }

        protected override CalculateCriteriaValuesInput GetInputArgument(AsyncCodeActivityContext context)
        {
            throw new NotImplementedException();
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, CalculateCriteriaValuesOutput result)
        {
            throw new NotImplementedException();
        }
    }
}
