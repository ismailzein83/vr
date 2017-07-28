using System;
using System.Collections.Generic;
using Vanrise.Entities;

namespace TOne.WhS.RouteSync.Entities
{
    public enum SwitchSyncResult { Succeed = 0, PartialFailed = 1, Failed = 2 }

    public class SwitchRouteSynchroniserOutput
    {
        public string ErrorBusinessMessage { get; set; }
        public string ExceptionDetail { get; set; }

        public virtual void LogMessage(string switchName, Action<Exception> writeHandledExceptionMessages)
        {
            VRBusinessException ex = new VRBusinessException(string.Format("Switch '{0}' is not synchronised", switchName), new Exception(ExceptionDetail));
            writeHandledExceptionMessages(ex);
        }

        public virtual string GetUniqueMessageKey(string switchId) 
        {
            return switchId;
        }
    }

    public class SwitchSyncOutput
    {
        public string SwitchId { get; set; }

        public SwitchSyncResult SwitchSyncResult { get; set; }

        public List<SwitchRouteSynchroniserOutput> SwitchRouteSynchroniserOutputList { get; set; }
    }
}
