using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.BusinessProcess;
using Vanrise.Queueing;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Fzero.FraudAnalysis.Data;
using System.Configuration;

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
            INumberProfileDataManager dataManager = FraudDataManagerFactory.GetDataManager<INumberProfileDataManager>();
            dataManager.LoadCDR(inputArgument.FromDate, inputArgument.ToDate, 0, (cdr) =>
                {
                    cdrs.Add(cdr);
                    if (cdrs.Count >= 100000)
                    {                        
                        inputArgument.OutputQueue.Enqueue(BuildCDRBatch(cdrs));
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
