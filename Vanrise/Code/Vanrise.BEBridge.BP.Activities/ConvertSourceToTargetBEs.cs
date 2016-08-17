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
        public BaseQueue<SourceBatches> SourceBatches { get; set; }
        public TargetBEConvertor TargetConverter { get; set; }
        public TargetBESynchronizer TargetBESynchronizer { get; set; }
        public BaseQueue<TargetBEBatch> TargetBEsToUpdate { get; set; }
        public BaseQueue<TargetBEBatch> TargetBEsToInsert { get; set; }
    }
    public sealed class ConvertSourceToTargetBEs : DependentAsyncActivity<ConvertSourceToTargetBEsInput>
    {
        [RequiredArgument]
        public InArgument<BaseQueue<SourceBatches>> SourceBatches { get; set; }
        [RequiredArgument]
        public InArgument<TargetBEConvertor> TargetConverter { get; set; }
        [RequiredArgument]
        public InArgument<TargetBESynchronizer> TargetBESynchronizer { get; set; }
        [RequiredArgument]
        public InOutArgument<BaseQueue<TargetBEBatch>> TargetBEsToUpdate { get; set; }
        [RequiredArgument]
        public InOutArgument<BaseQueue<TargetBEBatch>> TargetBEsToInsert { get; set; }

        protected override void DoWork(ConvertSourceToTargetBEsInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            List<ITargetBE> targetsToInsert = new List<ITargetBE>();
            List<ITargetBE> targetsToUpdate = new List<ITargetBE>();
            int batchCount = 50;
            int totalUpdateBatchConverted = 0;
            int totalInsertBatchConverted = 0;
            int totalUpdateTargets = 0;
            int totalInsertTargets = 0;
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.SourceBatches.TryDequeue((sourceBatches) =>
                    {

                        TargetBEConvertorConvertSourceBEsContext targetBEConvertorContext = new TargetBEConvertorConvertSourceBEsContext();
                        TargetBETryGetExistingContext targetContext = new TargetBETryGetExistingContext();
                        MergeTargetBEsContext mergeContext = new MergeTargetBEsContext();

                        foreach (var sourceBatch in sourceBatches.SorceBEBatches)
                        {
                            targetBEConvertorContext.SourceBEBatch = sourceBatch;
                            inputArgument.TargetConverter.ConvertSourceBEs(targetBEConvertorContext);
                            List<ITargetBE> targetBEs = targetBEConvertorContext.TargetBEs;
                            handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "Targer BEs count {0}.", targetBEs.Count);
                            foreach (ITargetBE targetBE in targetBEs)
                            {
                                targetContext.SourceBEId = targetBE.SourceBEId;
                                targetContext.TargetBE = targetBE;
                                if (inputArgument.TargetBESynchronizer.TryGetExistingBE(targetContext))
                                {
                                    mergeContext.ExistingBE = targetContext.TargetBE;
                                    mergeContext.NewBE = targetBE;
                                    inputArgument.TargetConverter.MergeTargetBEs(mergeContext);
                                    if (inputArgument.TargetConverter.CompareBeforeUpdate)
                                    {
                                        if (string.Compare(Serializer.Serialize(mergeContext.FinalBE), Serializer.Serialize(targetContext.TargetBE)) != 0)
                                            targetsToUpdate.Add(mergeContext.FinalBE);
                                    }
                                    else
                                        targetsToUpdate.Add(mergeContext.FinalBE);

                                    totalUpdateTargets++;
                                }
                                else
                                {
                                    targetsToInsert.Add(targetBE);
                                    totalInsertTargets++;
                                }
                            }
                            if (targetsToInsert.Count >= batchCount)
                            {
                                UpdateTargetBEList(inputArgument.TargetBEsToInsert, targetsToInsert);
                                totalInsertBatchConverted++;
                            }
                            if (targetsToUpdate.Count >= batchCount)
                            {
                                UpdateTargetBEList(inputArgument.TargetBEsToUpdate, targetsToUpdate);
                                totalUpdateBatchConverted++;
                            }

                            handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "{0} Insert Batches converted, {1} Insert Targets", totalInsertBatchConverted, totalInsertTargets);
                            handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "{0} Update Batches converted, {1} Update Targets", totalUpdateBatchConverted, totalInsertTargets);
                        }

                    });
                } while (!ShouldStop(handle) && hasItem);

                if (targetsToInsert.Count > 0)
                    UpdateTargetBEList(inputArgument.TargetBEsToInsert, targetsToInsert);
                if (targetsToUpdate.Count > 0)
                    UpdateTargetBEList(inputArgument.TargetBEsToUpdate, targetsToUpdate);
            });
        }

        void UpdateTargetBEList(BaseQueue<TargetBEBatch> targetBatch, List<ITargetBE> targets)
        {
            targetBatch.Enqueue(new TargetBEBatch() { TargetBEs = targets });
            targets = new List<ITargetBE>();
        }

        protected override ConvertSourceToTargetBEsInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ConvertSourceToTargetBEsInput
            {
                SourceBatches = this.SourceBatches.Get(context),
                TargetConverter = this.TargetConverter.Get(context),
                TargetBEsToInsert = this.TargetBEsToInsert.Get(context),
                TargetBEsToUpdate = this.TargetBEsToUpdate.Get(context),
                TargetBESynchronizer = this.TargetBESynchronizer.Get(context)
            };
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.TargetBEsToInsert.Get(context) == null)
                this.TargetBEsToInsert.Set(context, new MemoryQueue<TargetBEBatch>());
            if (this.TargetBEsToUpdate.Get(context) == null)
                this.TargetBEsToUpdate.Set(context, new MemoryQueue<TargetBEBatch>());
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
