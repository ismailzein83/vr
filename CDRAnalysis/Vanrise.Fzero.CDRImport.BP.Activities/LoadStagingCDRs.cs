using System;
using System.Activities;
using System.Collections.Generic;
using System.Configuration;
using Vanrise.BusinessProcess;
using Vanrise.Common;
using Vanrise.Fzero.CDRImport.Entities;
using Vanrise.Fzero.CDRImport.Data;
using Vanrise.Queueing;

namespace Vanrise.Fzero.CDRImport.BP.Activities
{
    #region Arguments Classes

    public class LoadStagingCDRsInput
    {
        public BaseQueue<StagingCDRBatch> OutputQueue { get; set; }

        public DateTime FromDate { get; set; }


        public DateTime ToDate { get; set; }

    }

    #endregion


    

    public sealed class LoadStagingCDRs : BaseAsyncActivity<LoadStagingCDRsInput>
    {
        #region Arguments

        [RequiredArgument]
        public InOutArgument<BaseQueue<StagingCDRBatch>> OutputQueue { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> FromDate { get; set; }


        [RequiredArgument]
        public InArgument<DateTime> ToDate { get; set; }


        #endregion

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new MemoryQueue<StagingCDRBatch>());

            base.OnBeforeExecute(context, handle);
        }

        static string configuredDirectory = ConfigurationManager.AppSettings["LoadCDRsDirectory"];

        protected override void DoWork(LoadStagingCDRsInput inputArgument, AsyncActivityHandle handle)
        {
            List<StagingCDR> cdrs = new List<StagingCDR>();
            int totalCount = 0;
            IStagingCDRDataManager dataManager = CDRDataManagerFactory.GetDataManager<IStagingCDRDataManager>();
            dataManager.LoadStagingCDR(inputArgument.FromDate, inputArgument.ToDate, 0, (cdr) =>
                {
                    cdrs.Add(cdr);
                    if (cdrs.Count >= 100000)
                    {
                        totalCount += cdrs.Count;
                        inputArgument.OutputQueue.Enqueue(BuildCDRBatch(cdrs));
                        handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Verbose, "{0} Staging CDRs loaded", totalCount);
                        cdrs = new List<StagingCDR>();
                    }
                });
            if (cdrs.Count > 0)
            {
                inputArgument.OutputQueue.Enqueue(BuildCDRBatch(cdrs));
            }
        }

        private StagingCDRBatch BuildCDRBatch(List<StagingCDR> cdrs)
        {
            var cdrsBytes = Vanrise.Common.Compressor.Compress(Vanrise.Common.ProtoBufSerializer.Serialize(cdrs));
            string filePath = !String.IsNullOrEmpty(configuredDirectory) ? System.IO.Path.Combine(configuredDirectory, Guid.NewGuid().ToString()) : System.IO.Path.GetTempFileName();
            System.IO.File.WriteAllBytes(filePath, cdrsBytes);
            return new StagingCDRBatch
            {
                StagingCDRBatchFilePath = filePath
            };
        }

        protected override LoadStagingCDRsInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new LoadStagingCDRsInput
            {
                FromDate = this.FromDate.Get(context),
                ToDate = this.ToDate.Get(context),
                OutputQueue = this.OutputQueue.Get(context)
            };
        }
               
    }
}
