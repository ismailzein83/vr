using System;
using System.Collections.Generic;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Entities;

namespace Vanrise.BusinessProcess.Data
{
    public interface IBPTrackingDataManager : IDataManager
    {
        void Insert(BPTrackingMessage trackingMessage);

        void WriteTrackingMessagesToDB(List<BPTrackingMessage> lstTrackingMsgs);

        List<BPTrackingMessage> GetBPInstanceTrackingMessages(long processInstanceId, List<LogEntryType> severities);

        List<BPTrackingMessage> GetBeforeId(BPTrackingBeforeIdInput input);

        List<BPTrackingMessage> GetUpdated(BPTrackingUpdateInput input);
    }
}