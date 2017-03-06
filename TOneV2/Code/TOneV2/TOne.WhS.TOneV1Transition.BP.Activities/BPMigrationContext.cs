using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.DBSync.Entities;
using Vanrise.BusinessProcess;

namespace TOne.WhS.TOneV1Transition.BP.Activities
{
    public class BPMigrationContext : MigrationContext
    {
        BPSharedInstanceData _bpSharedInstanceData;

        public BPMigrationContext(BPSharedInstanceData bpSharedInstanceData)
        {
            _bpSharedInstanceData = bpSharedInstanceData;
        }

        public override void WriteException(Exception ex)
        {
            _bpSharedInstanceData.WriteBusinessHandledException(ex);
        }

        public override void WriteInformation(string message)
        {
            _bpSharedInstanceData.WriteBusinessTrackingMsg(Vanrise.Entities.LogEntryType.Information, message);
        }

        public override void WriteEntry(Vanrise.Entities.LogEntryType logEntryType, string message)
        {
            _bpSharedInstanceData.WriteBusinessTrackingMsg(logEntryType, message);
        }

        public override void WriteVerbose(string message)
        {
            _bpSharedInstanceData.WriteBusinessTrackingMsg(Vanrise.Entities.LogEntryType.Verbose, message);
        }

        public override void WriteError(string message)
        {
            _bpSharedInstanceData.WriteBusinessTrackingMsg(Vanrise.Entities.LogEntryType.Error, message);
        }

        public override void WriteWarning(string message)
        {
            _bpSharedInstanceData.WriteBusinessTrackingMsg(Vanrise.Entities.LogEntryType.Warning, message);
        }
    }
}
