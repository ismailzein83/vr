using System.Activities;
using System.Collections.Generic;
using Vanrise.BusinessProcess;
using Vanrise.Common;
using Vanrise.Fzero.CDRImport.Entities;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Queueing;

namespace Vanrise.Fzero.FraudAnalysis.BP.Activities
{
    #region Argument Classes
    public class FillDimensionValuesInput
    {
        public BaseQueue<CDRBatch> InputQueue { get; set; }

        public BaseQueue<DWCDRBatch> OutputQueue { get; set; }
    }
    # endregion


    public class FillDimensionValues : DependentAsyncActivity<FillDimensionValuesInput>
    {
        #region Arguments

        [RequiredArgument]
        public InOutArgument<BaseQueue<CDRBatch>> InputQueue { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<DWCDRBatch>> OutputQueue { get; set; }

        # endregion

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new MemoryQueue<DWCDRBatch>());

            base.OnBeforeExecute(context, handle);
        }

        protected override void DoWork(FillDimensionValuesInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            int batchSize = int.Parse(System.Configuration.ConfigurationManager.AppSettings["DWCDRBatchSize"]);
            List<DWCDR> dwCDRBatch = new List<DWCDR>();
            int cdrsCount = 0;
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.InputQueue.TryDequeue(
                        (cdrBatch) =>
                        {
                            var serializedCDRs = Vanrise.Common.Compressor.Decompress(System.IO.File.ReadAllBytes(cdrBatch.CDRBatchFilePath));
                            System.IO.File.Delete(cdrBatch.CDRBatchFilePath);
                            var cdrs = Vanrise.Common.ProtoBufSerializer.Deserialize<List<CDR>>(serializedCDRs);
                            foreach (var cdr in cdrs)
                            {
                                DWCDR tempDWCDR = new DWCDR();
                                tempDWCDR.CDRId = cdr.Id;
                                dwCDRBatch.Add(tempDWCDR);
                            }
                            cdrsCount += cdrs.Count;

                            dwCDRBatch = SendDWCDRtoOutputQueue(inputArgument, handle, batchSize, dwCDRBatch);

                            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Verbose, "{0} CDRs filled dimensions", cdrsCount);

                        });
                }
                while (!ShouldStop(handle) && hasItem);
            });

            dwCDRBatch = SendDWCDRtoOutputQueue(inputArgument, handle, batchSize, dwCDRBatch);

            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Finished Loading CDRs from Database to Memory");
        }

        private static List<DWCDR> SendDWCDRtoOutputQueue(FillDimensionValuesInput inputArgument, AsyncActivityHandle handle, int batchSize, List<DWCDR> dwCDRBatch)
        {
            if (dwCDRBatch.Count >= batchSize)
            {
                handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Verbose, "{0} Data warehouse CDRs Sent", dwCDRBatch);
                inputArgument.OutputQueue.Enqueue(new DWCDRBatch()
                {
                    DWCDRs = dwCDRBatch
                });
                dwCDRBatch = new List<DWCDR>();
            }
            return dwCDRBatch;
        }

        protected override FillDimensionValuesInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new FillDimensionValuesInput
            {
                InputQueue = this.InputQueue.Get(context),
                OutputQueue = this.OutputQueue.Get(context)
            };
        }
    }
}
