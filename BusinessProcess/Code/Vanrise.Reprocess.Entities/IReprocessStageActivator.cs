using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;
using Vanrise.Queueing;
using Vanrise.Queueing.Entities;

namespace Vanrise.Reprocess.Entities
{
    public interface IReprocessStageActivator
    {
        object InitializeStage(IReprocessStageActivatorInitializingContext context);

        void ExecuteStage(IReprocessStageActivatorExecutionContext context);

        void FinalizeStage(IReprocessStageActivatorFinalizingContext context);

        int? GetStorageRowCount(IReprocessStageActivatorGetStorageRowCountContext context);

        void CommitChanges(IReprocessStageActivatorCommitChangesContext context);

        void DropStorage(IReprocessStageActivatorDropStorageContext context);

        List<string> GetOutputStages(List<string> stageNames);

        BaseQueue<IReprocessBatch> GetQueue();

        List<BatchRecord> GetStageBatchRecords(IReprocessStageActivatorPreparingContext context);
    }

    public interface IReprocessStageActivatorInitializingContext
    {
        long ProcessId { get; }
    }

    public interface IReprocessStageActivatorExecutionContext
    {
        BaseQueue<IReprocessBatch> InputQueue { get; }

        void DoWhilePreviousRunning(Action actionToDo);

        void DoWhilePreviousRunning(AsyncActivityStatus previousActivityStatus, Action actionToDo);

        void WriteTrackingMessage(LogEntryType severity, string messageFormat);

        RecordFilterGroup GetRecordFilterGroup(Guid? dataRecordTypeId);

        bool ShouldStop();

        void EnqueueBatch(string stageName, IReprocessBatch batch);

        DateTime From { get; }

        DateTime To { get; }

        List<string> StageNames { get; }

        string CurrentStageName { get; }

        long ProcessInstanceId { get; }

        object InitializationStageOutput { get; }

        QueueExecutionFlowStage QueueExecutionFlowStage { get; }
    }

    public interface IReprocessStageActivatorFinalizingContext
    {
        long ProcessInstanceId { get; }

        string CurrentStageName { get; }

        BatchRecord BatchRecord { get; }

        void WriteTrackingMessage(LogEntryType severity, string messageFormat);

        void DoWhilePreviousRunning(AsyncActivityStatus previousActivityStatus, Action actionToDo);

        object InitializationStageOutput { get; }

        RecordFilterGroup GetRecordFilterGroup(Guid? dataRecordTypeId);
    }

    public interface IReprocessStageActivatorGetStorageRowCountContext
    {
        object InitializationStageOutput { get; }
    }

    public interface IReprocessStageActivatorCommitChangesContext
    {
        RecordFilterGroup GetRecordFilterGroup(Guid? dataRecordTypeId);

        object InitializationStageOutput { get; }

        DateTime From { get; }

        DateTime To { get; }
    }

    public interface IReprocessStageActivatorDropStorageContext
    {
        object InitializationStageOutput { get; }
    }

    public interface IReprocessStageActivatorPreparingContext
    {
        long ProcessInstanceId { get; }

        string CurrentStageName { get; }

        DateTime StartDate { get; }

        DateTime EndDate { get; }
    }

    public abstract class BatchRecord
    {
        public abstract string GetBatchTitle();
    }
}
