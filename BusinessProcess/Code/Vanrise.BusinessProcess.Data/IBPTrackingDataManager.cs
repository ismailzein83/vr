using System;
using System.Collections.Generic;
using Vanrise.BusinessProcess.Entities;
namespace Vanrise.BusinessProcess.Data
{
    public interface IBPTrackingDataManager : IDataManager
    {
        void WriteTrackingMessagesToDB(List<BPTrackingMessage> lstTrackingMsgs);

        System.Collections.Generic.List<BPTrackingMessage> GetTrackingsByInstanceId(long processInstanceID, List<int> trackingSeverity, String message);
    }
}
