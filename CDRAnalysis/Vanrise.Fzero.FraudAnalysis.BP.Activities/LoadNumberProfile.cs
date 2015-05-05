using System;
using System.Activities;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Vanrise.BusinessProcess;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Queueing;
using System.Configuration;
using Vanrise.Fzero.FraudAnalysis.Data;

namespace Vanrise.Fzero.FraudAnalysis.BP.Activities
{

    #region Arguments Classes

    public class LoadNumberProfilesInput
    {
        public BaseQueue<NumberProfileBatch> OutputQueue { get; set; }

    }

    #endregion

    public class LoadNumberProfiles : BaseAsyncActivity<LoadNumberProfilesInput>
    {

        #region Arguments

        [RequiredArgument]
        public  InOutArgument<BaseQueue<NumberProfileBatch>> OutputQueue { get; set; }

        #endregion

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new MemoryQueue<NumberProfileBatch>());
            base.OnBeforeExecute(context, handle);
        }

        protected override void DoWork(LoadNumberProfilesInput inputArgument, AsyncActivityHandle handle)
        {
           int BatchSize = int.Parse( System.Configuration.ConfigurationManager.AppSettings["NumberProfileBatchSize"].ToString());
           handle.SharedInstanceData.WriteTrackingMessage(BusinessProcess.Entities.BPTrackingSeverity.Information, "LoadNumberProfiles.DoWork.Started ");

           INumberProfileDataManager dataManager = FraudDataManagerFactory.GetDataManager<INumberProfileDataManager>();
           dataManager.LoadNumberProfile(DateTime.Parse("2015-03-10 04:00:00"), DateTime.Parse("2015-03-20 06:00:00"), BatchSize, (numberProfiles) =>
           {
              
               inputArgument.OutputQueue.Enqueue(new NumberProfileBatch()
               {
                   numberProfiles = numberProfiles
               });


               handle.SharedInstanceData.WriteTrackingMessage(BusinessProcess.Entities.BPTrackingSeverity.Information, "LoadNumberProfiles.DoWork.Enqueued Count Items: {0} ", numberProfiles.Count);
           });
           handle.SharedInstanceData.WriteTrackingMessage(BusinessProcess.Entities.BPTrackingSeverity.Information, "LoadNumberProfiles.DoWork.Ended");
          
        }

        protected override LoadNumberProfilesInput GetInputArgument(System.Activities.AsyncCodeActivityContext context)
        {
            return new LoadNumberProfilesInput
            {
                OutputQueue = this.OutputQueue.Get(context)
            };
        }

    }
}
