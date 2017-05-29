﻿using System;
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
        public TargetBESynchronizerInitializeContext SynchronizerInitializeContext { get; set; }
        public TargetBEConvertorInitializeContext ConvertorInitializeContext { get; set; }
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
        public InArgument<TargetBESynchronizerInitializeContext> SynchronizerInitializeContext { get; set; }
        [RequiredArgument]
        public InArgument<TargetBEConvertorInitializeContext> ConvertorInitializeContext { get; set; }
        [RequiredArgument]
        public OutArgument<BaseQueue<BatchProcessingContext>> OutputQueue { get; set; }

        protected override void DoWork(ConvertSourceToTargetBEsInput inputArgument,
            AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            //var synchronizerInitializeDataContext = new TargetBESynchronizerInitializeContext();
            //inputArgument.TargetBeSynchronizer.Initialize(synchronizerInitializeDataContext);
            Dictionary<Object, ITargetBE> existingTargetBEsBySourceId = new Dictionary<object, ITargetBE>();

            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {

                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.BatchProcessingContextQueue.TryDequeue((batchProcessingContextQueue) =>
                    {
                        int totalUpdateTargets = 0;
                        int totalInsertTargets = 0;
                        List<ITargetBE> targetsToInsert = new List<ITargetBE>();
                        List<ITargetBE> targetsToUpdate = new List<ITargetBE>();
                        TargetBEConvertorConvertSourceBEsContext targetBeConvertorContext =
                            new TargetBEConvertorConvertSourceBEsContext(handle)
                            {
                                InitializationData = inputArgument.ConvertorInitializeContext.InitializationData,
                                SourceBEBatch = batchProcessingContextQueue.SourceBEBatch
                            };

                        inputArgument.TargetConverter.ConvertSourceBEs(targetBeConvertorContext);
                        List<ITargetBE> targetBEs = targetBeConvertorContext.TargetBEs;

                        foreach (ITargetBE targetBe in targetBEs)
                        {
                            ITargetBE existingBE = null;
                            var beId = targetBe.TargetBEId == null ? targetBe.SourceBEId : targetBe.TargetBEId;
                            if (beId != null && !existingTargetBEsBySourceId.TryGetValue(beId, out existingBE))
                            {
                                TargetBETryGetExistingContext tryGetExistingContext = new TargetBETryGetExistingContext(handle);

                                tryGetExistingContext.InitializationData = inputArgument.SynchronizerInitializeContext.InitializationData;
                                tryGetExistingContext.SourceBEId = targetBe.SourceBEId;
                                tryGetExistingContext.TargetBEId = targetBe.TargetBEId;

                                if (inputArgument.TargetBeSynchronizer.TryGetExistingBE(tryGetExistingContext))
                                    existingBE = tryGetExistingContext.TargetBE;
                            }

                            if (existingBE != null)
                            {
                                MergeTargetBEsContext mergeContext = new MergeTargetBEsContext(handle);
                                mergeContext.ExistingBE = existingBE;
                                mergeContext.NewBE = targetBe;
                                inputArgument.TargetConverter.MergeTargetBEs(mergeContext);
                                if (!inputArgument.TargetConverter.CompareBeforeUpdate ||
                                    String.CompareOrdinal(Serializer.Serialize(mergeContext.FinalBE),
                                            Serializer.Serialize(existingBE)) != 0)
                                {
                                    targetsToUpdate.Add(mergeContext.FinalBE);
                                    totalUpdateTargets++;
                                    var finalBEId = mergeContext.FinalBE.TargetBEId == null ? mergeContext.FinalBE.SourceBEId : mergeContext.FinalBE.TargetBEId;
                                    if (existingTargetBEsBySourceId.ContainsKey(finalBEId))
                                        existingTargetBEsBySourceId[finalBEId] = mergeContext.FinalBE;
                                    else
                                        existingTargetBEsBySourceId.Add(finalBEId, mergeContext.FinalBE);
                                }
                            }
                            else
                            {
                                targetsToInsert.Add(targetBe);
                                totalInsertTargets++;
                                if (beId != null)
                                    existingTargetBEsBySourceId.Add(beId, targetBe);
                            }
                        }
                        if (totalInsertTargets > 0)
                            handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "{0} {1} Converted.", totalInsertTargets, inputArgument.TargetConverter.Name);
                        if (totalUpdateTargets > 0)
                            handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "{0} {1} Converted.", totalUpdateTargets, inputArgument.TargetConverter.Name);

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
                SynchronizerInitializeContext = this.SynchronizerInitializeContext.Get(context),
                ConvertorInitializeContext = this.ConvertorInitializeContext.Get(context),
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
            AsyncActivityHandle _handle;

            public TargetBEConvertorConvertSourceBEsContext(AsyncActivityHandle handle)
            {
                if (handle == null)
                    throw new ArgumentNullException("handle");
                _handle = handle;
            }

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

            public object InitializationData
            {
                get;
                set;
            }
            public void WriteTrackingMessage(Vanrise.Entities.LogEntryType severity, string messageFormat, params object[] args)
            {
                _handle.SharedInstanceData.WriteTrackingMessage(severity, messageFormat, args);
            }

            public void WriteBusinessTrackingMsg(Vanrise.Entities.LogEntryType severity, string messageFormat, params object[] args)
            {
                _handle.SharedInstanceData.WriteBusinessTrackingMsg(severity, messageFormat, args);
            }


            public void WriteHandledException(Exception ex)
            {
                _handle.SharedInstanceData.WriteHandledException(ex);
            }

            public void WriteBusinessHandledException(Exception ex)
            {
                _handle.SharedInstanceData.WriteBusinessHandledException(ex);
            }
        }




        private class TargetBETryGetExistingContext : ITargetBESynchronizerTryGetExistingBEContext
        {
            AsyncActivityHandle _handle;

            public TargetBETryGetExistingContext(AsyncActivityHandle handle)
            {
                if (handle == null)
                    throw new ArgumentNullException("handle");
                _handle = handle;
            }

            public object InitializationData
            {
                set;
                get;
            }

            public object SourceBEId
            {
                get;
                set;
            }

            public object TargetBEId
            {
                get;
                set;
            }
            public ITargetBE TargetBE
            {
                set;
                get;
            }


            public void WriteTrackingMessage(Vanrise.Entities.LogEntryType severity, string messageFormat, params object[] args)
            {
                _handle.SharedInstanceData.WriteTrackingMessage(severity, messageFormat, args);
            }

            public void WriteBusinessTrackingMsg(Vanrise.Entities.LogEntryType severity, string messageFormat, params object[] args)
            {
                _handle.SharedInstanceData.WriteBusinessTrackingMsg(severity, messageFormat, args);
            }


            public void WriteHandledException(Exception ex)
            {
                _handle.SharedInstanceData.WriteHandledException(ex);
            }

            public void WriteBusinessHandledException(Exception ex)
            {
                _handle.SharedInstanceData.WriteBusinessHandledException(ex);
            }
        }

        private class MergeTargetBEsContext : ITargetBEConvertorMergeTargetBEsContext
        {
            AsyncActivityHandle _handle;

            public MergeTargetBEsContext(AsyncActivityHandle handle)
            {
                if (handle == null)
                    throw new ArgumentNullException("handle");
                _handle = handle;
            }

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


            public void WriteTrackingMessage(Vanrise.Entities.LogEntryType severity, string messageFormat, params object[] args)
            {
                _handle.SharedInstanceData.WriteTrackingMessage(severity, messageFormat, args);
            }

            public void WriteBusinessTrackingMsg(Vanrise.Entities.LogEntryType severity, string messageFormat, params object[] args)
            {
                _handle.SharedInstanceData.WriteBusinessTrackingMsg(severity, messageFormat, args);
            }


            public void WriteHandledException(Exception ex)
            {
                _handle.SharedInstanceData.WriteHandledException(ex);
            }

            public void WriteBusinessHandledException(Exception ex)
            {
                _handle.SharedInstanceData.WriteBusinessHandledException(ex);
            }
        }
        #endregion

    }

}
