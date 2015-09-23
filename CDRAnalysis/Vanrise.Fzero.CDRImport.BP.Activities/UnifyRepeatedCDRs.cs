using System;
using System.Activities;
using System.Collections.Generic;
using Vanrise.BusinessProcess;
using Vanrise.Common;
using Vanrise.Fzero.CDRImport.Entities;
using Vanrise.Fzero.CDRImport.Business;
using Vanrise.Fzero.CDRImport.Data;
using Vanrise.Fzero.CDRImport.Entities;
using Vanrise.Queueing;
using System.Configuration;

namespace Vanrise.Fzero.CDRImport.BP.Activities
{

    #region Arguments Classes

    public class UnifyRepeatedCDRsInput
    {
        public BaseQueue<StagingCDRBatch> InputQueue { get; set; }

        public BaseQueue<CDRBatch> OutputQueue { get; set; }
        
        public DateTime FromDate { get; set; }
        
        public DateTime ToDate { get; set; }
    }

    #endregion

    public class UnifyRepeatedCDRs : DependentAsyncActivity<UnifyRepeatedCDRsInput>
    {

        #region Arguments

        [RequiredArgument]
        public InOutArgument<BaseQueue<StagingCDRBatch>> InputQueue { get; set; }

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

        protected override void DoWork(UnifyRepeatedCDRsInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            List<CDR> cdrBatch = new List<CDR>();
            CDR currentCDR = new CDR();
            string currentCGPN = null ;
            string currentCDPN = null;

            int cdrsCount = 0;
            int totalCount = 0;

            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {

                    hasItem = inputArgument.InputQueue.TryDequeue(
                        (stagingcdrBatch) =>
                        {
                            var serializedCDRs = Vanrise.Common.Compressor.Decompress(System.IO.File.ReadAllBytes(stagingcdrBatch.StagingCDRBatchFilePath));
                            System.IO.File.Delete(stagingcdrBatch.StagingCDRBatchFilePath);
                            var stagingCDRs = Vanrise.Common.ProtoBufSerializer.Deserialize<List<StagingCDR>>(serializedCDRs);
                            foreach (var stagingCDR in stagingCDRs)
                            {
                                if (currentCGPN != stagingCDR.CGPN || currentCDPN !=stagingCDR.CDPN)
                                {
                                    if (currentCGPN != null && currentCDPN!=null)
                                    {
                                        cdrBatch.Add(currentCDR);
                                        if (cdrBatch.Count >= 100000)
                                        {
                                            totalCount += cdrBatch.Count;
                                            inputArgument.OutputQueue.Enqueue(BuildCDRBatch(cdrBatch));
                                            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Verbose, "{0} CDRs Unified", totalCount);
                                            cdrBatch = new List<CDR>();
                                        }
                                    }
                                    currentCGPN = stagingCDR.CGPN;
                                    currentCDPN = stagingCDR.CDPN;
                                    
                                    
                                   // currentCDR.

                                }
                                else
                                {
                                   
                                }
                            }
                            cdrsCount += stagingCDRs.Count;
                            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Verbose, "{0} CDRs profiled", cdrsCount);

                        });
                }
                while (!ShouldStop(handle) && hasItem);
            });
            if (cdrBatch.Count > 0)
            {
                inputArgument.OutputQueue.Enqueue(BuildCDRBatch(cdrBatch));
            }

            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Finished Loading CDRs from Database to Memory");

           
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

        protected override UnifyRepeatedCDRsInput GetInputArgument2(System.Activities.AsyncCodeActivityContext context)
        {
            return new UnifyRepeatedCDRsInput
            {
                InputQueue = this.InputQueue.Get(context),
                OutputQueue = this.OutputQueue.Get(context),
                FromDate = this.FromDate.Get(context),
                ToDate = this.ToDate.Get(context)
            };
        }

    }
}
