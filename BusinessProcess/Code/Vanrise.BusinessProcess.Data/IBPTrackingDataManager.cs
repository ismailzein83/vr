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

        BigResult<BPTrackingMessage> GetFilteredTrackings(Vanrise.Entities.DataRetrievalInput<TrackingQuery> input);

        List<BPTrackingMessage> GetTrackingsFrom(TrackingQuery input);
    }
}
