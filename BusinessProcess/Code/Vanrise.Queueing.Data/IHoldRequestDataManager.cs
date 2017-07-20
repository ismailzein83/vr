using System;
using System.Collections.Generic;
using Vanrise.Entities;
using Vanrise.Queueing.Entities;

namespace Vanrise.Queueing.Data
{
    public interface IHoldRequestDataManager : IDataManager
    {
        List<HoldRequest> GetAllHoldRequests();

        bool AreHoldRequestsUpdated(ref object updateHandle);

        void Delete(long holdRequestId);

        void Insert(long BPInstanceID, Guid executionFlowDefinitionId, DateTime from, DateTime to, List<int> queuesToHold, List<int> queuesToProcess, HoldRequestStatus status);

        void UpdateStatus(long holdRequestId, HoldRequestStatus status);

        DateTimeRange GetDBDateTimeRange();
    }
}