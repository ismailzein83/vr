using System;
using System.Activities;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public NumberRangeDefinition NumberRange { get; set; }

        public BaseQueue<CDRBatch> OutputQueue { get; set; }

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
        
        public InArgument<NumberRangeDefinition> NumberRange { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<CDRBatch>> OutputQueue { get; set; }

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
            //if (String.IsNullOrEmpty(configuredDirectory))
            //    throw new ArgumentNullException("FraudAnalysis_LoadCDRsDirectory");
            //var cdrsBytes = Vanrise.Common.Compressor.Compress(Vanrise.Common.ProtoBufSerializer.Serialize(cdrs));
            //string filePath = !String.IsNullOrEmpty(configuredDirectory) ? System.IO.Path.Combine(configuredDirectory, Guid.NewGuid().ToString()) : System.IO.Path.GetTempFileName();
            //System.IO.File.WriteAllBytes(filePath, cdrsBytes);
            return new CDRBatch
            {
                CDRs = cdrs
                //CDRBatchFilePath = filePath
            };
        }

        protected override LoadCDRsInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new LoadCDRsInput
            {
                NumberRange = this.NumberRange.Get(context),
                FromDate = this.FromDate.Get(context),
                ToDate = this.ToDate.Get(context),
                OutputQueue = this.OutputQueue.Get(context)
            };
        }

        protected override LoadCDRsOutput DoWorkWithResult(LoadCDRsInput inputArgument, AsyncActivityHandle handle)
        {
            List<CDR> cdrs = new List<CDR>();
            int totalCount = 0;
            int index = 0;
            ICDRDataManager dataManager = CDRDataManagerFactory.GetDataManager<ICDRDataManager>();
            dataManager.LoadCDR(inputArgument.FromDate, inputArgument.ToDate, inputArgument.NumberRange != null ? inputArgument.NumberRange.Prefixes : null, (cdr) =>
            {
                cdrs.Add(cdr);
                index++;
                if (cdrs.Count >= 25000)
                {
                    totalCount += cdrs.Count;
                    inputArgument.OutputQueue.Enqueue(BuildCDRBatch(cdrs));
                    cdrs = new List<CDR>();
                    if (index >= 100000)
                    {
                        handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Verbose, "{0} CDRs loaded", totalCount);
                        index = 0;
                    }
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
