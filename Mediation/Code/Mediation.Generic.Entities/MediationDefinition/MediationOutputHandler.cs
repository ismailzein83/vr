using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess;
using Vanrise.Data;
using Vanrise.Entities;
using Vanrise.Queueing;

namespace Mediation.Generic.Entities
{
    public abstract class MediationOutputHandler
    {
        public abstract Guid ConfigId { get; }

        public abstract void Execute(IMediationOutputHandlerContext context);
    }

    public interface IMediationOutputHandlerContext
    {
        MediationDefinition MediationDefinition { get; }

        BaseQueue<PreparedRecordsBatch> InputQueue { get; }

        void DoWhilePreviousRunning(Action actionToDo);

        void DoWhilePreviousRunning(AsyncActivityStatus previousActivityStatus, Action actionToDo);

        void WriteTrackingMessage(LogEntryType severity, string messageFormat, params object[] args);

        bool ShouldStop();

        long ProcessInstanceId { get; }

        void PrepareDataForDBApply<R, S>(IBulkApplyDataManager<R> dataManager, BaseQueue<S> inputQueue, BaseQueue<object> outputQueue, Func<S, System.Collections.Generic.IEnumerable<R>> GetItems);
    }    
}
