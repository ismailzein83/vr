using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.BEBridge.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Queueing;
using Vanrise.Common;

namespace Vanrise.BEBridge.BP.Activities
{
    public class ConvertSourceToTargetBEsInput
    {
        public TargetBEConvertor TargetConverter { get; set; }
        public TargetBESynchronizer TargetBeSynchronizer { get; set; }
        public BaseQueue<BatchProcessingContext> BatchProcessingContextQueue { get; set; }
        public BaseQueue<BatchProcessingContext> OutputQueue { get; set; }

    }
    public sealed class ConvertSourceToTargetBEs : DependentAsyncActivity<ConvertSourceToTargetBEsInput>
    {
        [RequiredArgument]
        public InArgument<TargetBEConvertor> TargetConverter { get; set; }
        [RequiredArgument]
        public InArgument<TargetBESynchronizer> TargetBeSynchronizer { get; set; }
        [RequiredArgument]
        public InArgument<BaseQueue<BatchProcessingContext>> BatchProcessingContextQueue { get; set; }
        [RequiredArgument]
        public OutArgument<BaseQueue<BatchProcessingContext>> OutputQueue { get; set; }

        protected override void DoWork(ConvertSourceToTargetBEsInput inputArgument,
            AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            int totalUpdateTargets = 0;
            int totalInsertTargets = 0;

            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {

                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.BatchProcessingContextQueue.TryDequeue((batchProcessingContextQueue) =>
                    {
                        List<ITargetBE> targetsToInsert = new List<ITargetBE>();
                        List<ITargetBE> targetsToUpdate = new List<ITargetBE>();
                        TargetBEConvertorConvertSourceBEsContext targetBeConvertorContext =
                            new TargetBEConvertorConvertSourceBEsContext();

                        targetBeConvertorContext.SourceBEBatch = batchProcessingContextQueue.SourceBEBatch;
                        inputArgument.TargetConverter.ConvertSourceBEs(targetBeConvertorContext);
                        List<ITargetBE> targetBEs = targetBeConvertorContext.TargetBEs;
                        handle.SharedInstanceData.WriteTrackingMessage(
                            Vanrise.Entities.LogEntryType.Information, "Targert BEs count {0}.", targetBEs.Count);
                        foreach (ITargetBE targetBe in targetBEs)
                        {
                            TargetBETryGetExistingContext targetContext = new TargetBETryGetExistingContext();
                            MergeTargetBEsContext mergeContext = new MergeTargetBEsContext();

                            targetContext.SourceBEId = targetBe.SourceBEId;
                            targetContext.TargetBE = targetBe;
                            if (inputArgument.TargetBeSynchronizer.TryGetExistingBE(targetContext))
                            {
                                mergeContext.ExistingBE = targetContext.TargetBE;
                                mergeContext.NewBE = targetBe;
                                inputArgument.TargetConverter.MergeTargetBEs(mergeContext);
                                if (inputArgument.TargetConverter.CompareBeforeUpdate)
                                {
                                    if (
                                        String.CompareOrdinal(Serializer.Serialize(mergeContext.FinalBE),
                                            Serializer.Serialize(targetContext.TargetBE)) != 0)
                                        targetsToUpdate.Add(mergeContext.FinalBE);
                                }
                                else
                                    targetsToUpdate.Add(mergeContext.FinalBE);

                                totalUpdateTargets++;
                            }
                            else
                            {
                                targetsToInsert.Add(targetBe);
                                totalInsertTargets++;
                            }
                        }
                        handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "{0} Insert Targets Converted.", totalInsertTargets);
                        handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "{0} Update Targets Converted.", totalUpdateTargets);
                        totalUpdateTargets = 0;
                        totalInsertTargets = 0;

                        batchProcessingContextQueue.SetTargetBEs(targetsToInsert, targetsToUpdate);
                        inputArgument.OutputQueue.Enqueue(batchProcessingContextQueue);

                    });
                } while (!ShouldStop(handle) && hasItem);

            });
        }

        protected override ConvertSourceToTargetBEsInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ConvertSourceToTargetBEsInput
            {
                TargetConverter = this.TargetConverter.Get(context),
                TargetBeSynchronizer = this.TargetBeSynchronizer.Get(context),
                BatchProcessingContextQueue = this.BatchProcessingContextQueue.Get(context),
                OutputQueue = this.OutputQueue.Get(context)
            };
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.BatchProcessingContextQueue.Get(context) == null)
                this.BatchProcessingContextQueue.Set(context, new MemoryQueue<BatchProcessingContext>());
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new MemoryQueue<BatchProcessingContext>());
            base.OnBeforeExecute(context, handle);
        }

        #region Context Implementation
        private class TargetBEConvertorConvertSourceBEsContext : ITargetBEConvertorConvertSourceBEsContext
        {
            public SourceBEBatch SourceBEBatch
            {
                set;
                get;
            }

            public List<ITargetBE> TargetBEs
            {
                set;
                get;
            }
        }

        private class TargetBETryGetExistingContext : ITargetBESynchronizerTryGetExistingBEContext
        {

            public object SourceBEId
            {
                get;
                set;
            }

            public ITargetBE TargetBE
            {
                set;
                get;
            }
        }

        private class MergeTargetBEsContext : ITargetBEConvertorMergeTargetBEsContext
        {
            public ITargetBE ExistingBE
            {
                get;
                set;
            }

            public ITargetBE FinalBE
            {
                set;
                get;
            }

            public ITargetBE NewBE
            {
                get;
                set;
            }
        }
        #endregion

    }

}
