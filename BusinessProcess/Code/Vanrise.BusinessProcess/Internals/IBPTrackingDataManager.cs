using System;
using System.Collections.Generic;
namespace Vanrise.BusinessProcess
{
    interface IBPTrackingDataManager
    {
        void WriteTrackingMessagesToDB(List<BPTrackingMessage> lstTrackingMsgs);
    }
}
