using System;
using System.Activities;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Vanrise.BusinessProcess;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Queueing;
using System.Configuration;
using Vanrise.Fzero.FraudAnalysis.Data.MySQL;
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


        protected override void DoWork(LoadNumberProfilesInput inputArgument, AsyncActivityHandle handle)
        {
           int BatchSize = int.Parse( System.Configuration.ConfigurationManager.AppSettings["NumberProfileBatchSize"].ToString());
           handle.SharedInstanceData.WriteTrackingMessage(BusinessProcess.Entities.BPTrackingSeverity.Information, "Start LoadNumberProfiles.DoWork.Start {0}", DateTime.Now);

           INumberProfileDataManager dataManager = FraudDataManagerFactory.GetDataManager<INumberProfileDataManager>();
           dataManager.LoadNumberProfile(DateTime.Now.AddHours(-1), DateTime.Now, BatchSize, (numberProfiles) =>
           {
               inputArgument.OutputQueue.Enqueue(new NumberProfileBatch()
               {
                   numberProfiles = numberProfiles
               });
           });

           //handle.SharedInstanceData.WriteTrackingMessage(BusinessProcess.Entities.BPTrackingSeverity.Information, "Start LoadNumberProfiles.DoWork.LoadNumberProfile {0}", DateTime.Now);
           
           //handle.SharedInstanceData.WriteTrackingMessage(BusinessProcess.Entities.BPTrackingSeverity.Information, "Start LoadNumberProfiles.DoWork.Enqueued {0}", DateTime.Now);
           //handle.SharedInstanceData.WriteTrackingMessage(BusinessProcess.Entities.BPTrackingSeverity.Information, "End LoadNumberProfiles.DoWork {0}", DateTime.Now);
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
