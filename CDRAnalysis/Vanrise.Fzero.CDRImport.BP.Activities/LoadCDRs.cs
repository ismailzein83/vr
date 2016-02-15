using System;
using System.Activities;
using System.Collections.Generic;
using System.Configuration;
using Vanrise.BusinessProcess;
using Vanrise.Common;
using Vanrise.Fzero.CDRImport.Data;
using Vanrise.Fzero.CDRImport.Entities;
using Vanrise.Queueing;

namespace Vanrise.Fzero.CDRImport.BP.Activities
{
    #region Arguments Classes

    public class LoadCDRsInput
    {
        public BaseQueue<CDRBatch> OutputQueue { get; set; }

        public List<string> NumberPrefix { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }
    }


    public class LoadCDRsOutput
    {
        public long NumberOfCDRs { get; set; }
    }

    #endregion

    public sealed class LoadCDRs : BaseAsyncActivity<LoadCDRsInput, LoadCDRsOutput>
    {
        #region Arguments

        [RequiredArgument]
        public InOutArgument<BaseQueue<CDRBatch>> OutputQueue { get; set; }

        public InArgument<List<string>> NumberPrefix { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> FromDate { get; set; }


        [RequiredArgument]
        public InArgument<DateTime> ToDate { get; set; }

        public OutArgument<long> NumberOfCDRs { get; set; }

        #endregion

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new MemoryQueue<CDRBatch>());

            base.OnBeforeExecute(context, handle);
        }

        static string configuredDirectory = ConfigurationManager.AppSettings["FraudAnalysis_LoadCDRsDirectory"];

        private CDRBatch BuildCDRBatch(List<CDR> cdrs)
        {
            return new CDRBatch
            {
                CDRs = cdrs
            };
        }

        protected override LoadCDRsInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new LoadCDRsInput
            {
                NumberPrefix = this.NumberPrefix.Get(context),
                FromDate = this.FromDate.Get(context),
                ToDate = this.ToDate.Get(context),
                OutputQueue = this.OutputQueue.Get(context)
            };
        }

        protected override LoadCDRsOutput DoWorkWithResult(LoadCDRsInput inputArgument, AsyncActivityHandle handle)
        {
            List<CDR> cdrs = new List<CDR>();
            int totalCount = 0;
            ICDRDataManager dataManager = CDRDataManagerFactory.GetDataManager<ICDRDataManager>();
            dataManager.LoadCDR(inputArgument.NumberPrefix, inputArgument.FromDate, inputArgument.ToDate, 0, (cdr) =>
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
                totalCount += cdrs.Count;
                inputArgument.OutputQueue.Enqueue(BuildCDRBatch(cdrs));
            }
            return new LoadCDRsOutput
            {
                NumberOfCDRs = totalCount
            };
           
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, LoadCDRsOutput result)
        {
            this.NumberOfCDRs.Set(context, result.NumberOfCDRs);
        }
    }
}
