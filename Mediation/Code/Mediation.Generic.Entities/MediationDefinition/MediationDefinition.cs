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
    public class MediationDefinition
    {
        public int MediationDefinitionId { get; set; }
        public string Name { get; set; }
        public Guid ParsedRecordTypeId { get; set; }
        public Guid CookedRecordTypeId { get; set; }
        public ParsedRecordIdentificationSetting ParsedRecordIdentificationSetting { get; set; }
        public UpdateCookedFromParsed CookedFromParsedSettings { get; set; }
        public CookedCDRDataStoreSetting CookedCDRDataStoreSetting { get; set; }

        public List<MediationOutputHandlerDefinition> OutputHandlers { get; set; }
    }

    public class ParsedRecordIdentificationSetting
    {
        public string SessionIdField { get; set; }
        public string EventTimeField { get; set; }
        public List<StatusMapping> StatusMappings { get; set; }

    }

    public class UpdateCookedFromParsed
    {
        public Guid TransformationDefinitionId { get; set; }

        public string ParsedRecordName { get; set; }

        public string CookedRecordName { get; set; }
    }

    public class CookedCDRDataStoreSetting
    {
        public Guid DataRecordStorageId { get; set; }
    }

    public class MediationOutputHandlerDefinition
    {
        public string OutputRecordName { get; set; }

        public MediationOutputHandler Handler { get; set; }
    }

    public abstract class MediationOutputHandler
    {
        public abstract Guid ConfigId { get; }

        public abstract void Execute(IMediationOutputHandlerContext context);
    }

    public interface IMediationOutputHandlerContext
    {
        MediationDefinition MediationDefinition { get; }

        BaseQueue<PreparedCdrBatch> InputQueue { get; }

        void DoWhilePreviousRunning(Action actionToDo);

        void DoWhilePreviousRunning(AsyncActivityStatus previousActivityStatus, Action actionToDo);

        void WriteTrackingMessage(LogEntryType severity, string messageFormat, params object[] args);

        bool ShouldStop();

        long ProcessInstanceId { get; }

        void PrepareDataForDBApply<R, S>(IBulkApplyDataManager<R> dataManager, BaseQueue<S> inputQueue, BaseQueue<object> outputQueue, Func<S, System.Collections.Generic.IEnumerable<R>> GetItems);
    }
}
