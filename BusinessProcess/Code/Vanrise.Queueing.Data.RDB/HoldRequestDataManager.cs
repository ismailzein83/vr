using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Queueing.Data.RDB
{
    class HoldRequestDataManager : IHoldRequestDataManager
    {
        public bool AreHoldRequestsUpdated(ref object updateHandle)
        {
            throw new NotImplementedException();
        }

        public void Delete(long holdRequestId)
        {
            throw new NotImplementedException();
        }

        public void DeleteHoldRequestByBPInstanceId(long bpInstanceId)
        {
            throw new NotImplementedException();
        }

        public List<Queueing.Entities.HoldRequest> GetAllHoldRequests()
        {
            throw new NotImplementedException();
        }

        public Vanrise.Entities.DateTimeRange GetDBDateTimeRange()
        {
            throw new NotImplementedException();
        }

        public void Insert(long BPInstanceID, Guid executionFlowDefinitionId, DateTime from, DateTime to, List<int> queuesToHold, List<int> queuesToProcess, Queueing.Entities.HoldRequestStatus status)
        {
            throw new NotImplementedException();
        }

        public void UpdateStatus(long holdRequestId, Queueing.Entities.HoldRequestStatus status)
        {
            throw new NotImplementedException();
        }
    }
}
