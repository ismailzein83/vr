using System;
using System.Collections.Generic;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Entities;

namespace Vanrise.BusinessProcess.Data
{
    public interface IBPTrackingDataManager : IDataManager
    {
        void Insert(BPTrackingMessage trackingMessage);

        void InsertValidationMessages(IEnumerable<ValidationMessage> messages);

        void WriteTrackingMessagesToDB(List<BPTrackingMessage> lstTrackingMsgs);

        BigResult<BPTrackingMessageDetail> GetFilteredBPInstanceTracking(Vanrise.Entities.DataRetrievalInput<BPTrackingQuery> input);
        BigResult<BPTrackingMessage> GetFilteredTrackings(Vanrise.Entities.DataRetrievalInput<BPTrackingQuery> input);

        List<BPTrackingMessage> GetTrackingsFrom(BPTrackingQuery input);

        List<BPTrackingMessage> GetBeforeId(BPTrackingBeforeIdInput input);

        List<BPTrackingMessage> GetUpdated(BPTrackingUpdateInput input);
    }
}
