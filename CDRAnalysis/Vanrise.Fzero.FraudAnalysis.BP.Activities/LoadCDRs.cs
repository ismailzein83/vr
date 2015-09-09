using System;
using System.Activities;
using System.Collections.Generic;
using System.Configuration;
using Vanrise.BusinessProcess;
using Vanrise.Common;
using Vanrise.Fzero.CDRImport.Entities;
using Vanrise.Fzero.FraudAnalysis.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Queueing;

namespace Vanrise.Fzero.FraudAnalysis.BP.Activities
{
    #region Arguments Classes

    public class LoadCDRsInput
    {
        public BaseQueue<CDRBatch> OutputQueue { get; set; }

        public DateTime FromDate { get; set; }


        public DateTime ToDate { get; set; }

    }

    #endregion

    public sealed class LoadCDRs : BaseAsyncActivity<LoadCDRsInput>
    {
        #region Arguments

        [RequiredArgument]
        public InOutArgument<BaseQueue<CDRBatch>> OutputQueue { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> FromDate { get; set; }


        [RequiredArgument]
        public InArgument<DateTime> ToDate { get; set; }


        #endregion

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new MemoryQueue<CDRBatch>());

            base.OnBeforeExecute(context, handle);
        }

        static string configuredDirectory = ConfigurationManager.AppSettings["LoadCDRsDirectory"];

        protected override void DoWork(LoadCDRsInput inputArgument, AsyncActivityHandle handle)
        {
            List<CDR> cdrs = new List<CDR>();
            int totalCount = 0;
            INormalCDRDataManager dataManager = FraudDataManagerFactory.GetDataManager<INormalCDRDataManager>();
            dataManager.LoadCDR(inputArgument.FromDate, inputArgument.ToDate, 0, (cdr) =>
                {
                    cdrs.Add(cdr);
                    if (cdrs.Count >= 100000)
                    {
                        totalCount += cdrs.Count;
                        inputArgument.OutputQueue.Enqueue(BuildCDRBatch(cdrs));
                        handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Verbose, "{0} CDRs loaded", totalCount);
                        cdrs = new List<CDR>();
                    }
                });
            if (cdrs.Count > 0)
            {
                inputArgument.OutputQueue.Enqueue(BuildCDRBatch(cdrs));
            }
        }

        private CDRBatch BuildCDRBatch(List<CDR> cdrs)
        {
            var cdrsBytes = Vanrise.Common.Compressor.Compress(Vanrise.Common.ProtoBufSerializer.Serialize(cdrs));
            string filePath = !String.IsNullOrEmpty(configuredDirectory) ? System.IO.Path.Combine(configuredDirectory, Guid.NewGuid().ToString()) : System.IO.Path.GetTempFileName();
            System.IO.File.WriteAllBytes(filePath, cdrsBytes);
            return new CDRBatch
            {
                CDRBatchFilePath = filePath
            };
        }

        protected override LoadCDRsInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new LoadCDRsInput
            {
                FromDate = this.FromDate.Get(context),
                ToDate = this.ToDate.Get(context),
                OutputQueue = this.OutputQueue.Get(context)
            };
        }
    }
}
