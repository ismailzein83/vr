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
    public class FillFactValuesInput
    {
        public BaseQueue<CDRBatch> InputQueue { get; set; }

        public BaseQueue<DWFactBatch> OutputQueue { get; set; }

        public DWDimensionDictionary BTSs { get; set; }
    }
    # endregion


    public class FillFactValues : DependentAsyncActivity<FillFactValuesInput>
    {
        #region Arguments

        [RequiredArgument]
        public InArgument<BaseQueue<CDRBatch>> InputQueue { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<DWFactBatch>> OutputQueue { get; set; }

        [RequiredArgument]
        public InArgument<DWDimensionDictionary> BTSs { get; set; }

        # endregion

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new MemoryQueue<DWFactBatch>());

            base.OnBeforeExecute(context, handle);
        }

        protected override void DoWork(FillFactValuesInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            int batchSize = int.Parse(System.Configuration.ConfigurationManager.AppSettings["DWFactBatchSize"]);
            List<DWFact> dwFactBatch = new List<DWFact>();
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
                                DWFact tempDWFact = new DWFact();
                                tempDWFact.CDRId = cdr.Id;
                                dwFactBatch.Add(tempDWFact);
                            }
                            cdrsCount += cdrs.Count;

                            dwFactBatch = SendDWFacttoOutputQueue(inputArgument, handle, batchSize, dwFactBatch, false);

                            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Verbose, "{0} CDRs filled dimensions", cdrsCount);

                        });
                }
                while (!ShouldStop(handle) && hasItem);

            });

            dwFactBatch = SendDWFacttoOutputQueue(inputArgument, handle, batchSize, dwFactBatch, true);

            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Finished Loading CDRs from Database to Memory");
        }

        private static List<DWFact> SendDWFacttoOutputQueue(FillFactValuesInput inputArgument, AsyncActivityHandle handle, int batchSize, List<DWFact> dwFactBatch, bool IsLastBatch)
        {
            if (dwFactBatch.Count >= batchSize || IsLastBatch)
            {
                handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Verbose, "{0} Data warehouse CDRs Sent", dwFactBatch);
                inputArgument.OutputQueue.Enqueue(new DWFactBatch()
                {
                    DWFacts = dwFactBatch
                });
                dwFactBatch = new List<DWFact>();
            }
            return dwFactBatch;
        }

        protected override FillFactValuesInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new FillFactValuesInput
            {
                InputQueue = this.InputQueue.Get(context),
                OutputQueue = this.OutputQueue.Get(context)
            };
        }
    }
}
