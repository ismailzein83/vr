using System;
using System.Collections.Generic;
using Vanrise.BusinessProcess.Entities;
namespace Vanrise.BusinessProcess.Data
{
    public interface IBPTrackingDataManager : IDataManager
    {
        void Insert(BPTrackingMessage trackingMessage);

        void WriteTrackingMessagesToDB(List<BPTrackingMessage> lstTrackingMsgs);

        TrackingResult GetTrackingsByInstanceId(Vanrise.Entities.DataRetrievalInput<TrackingQuery> input);
    }
}
