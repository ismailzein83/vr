using System;
using System.Collections.Generic;
using Vanrise.BusinessProcess.Entities;
namespace Vanrise.BusinessProcess.Data
{
    public interface IBPTrackingDataManager
    {
        void WriteTrackingMessagesToDB(List<BPTrackingMessage> lstTrackingMsgs);

        System.Collections.Generic.List<BPTrackingMessage> GetTrackingsByInstanceId(long ProcessInstanceID);
    }
}
