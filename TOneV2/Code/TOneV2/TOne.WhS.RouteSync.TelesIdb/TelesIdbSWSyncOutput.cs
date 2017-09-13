using System;
using Vanrise.Entities;
using TOne.WhS.RouteSync.Entities;

namespace TOne.WhS.RouteSync.TelesIdb
{
    public class TelesIdbSWSyncOutput : SwitchRouteSynchroniserOutput
    {
        public int ItemIndex { get; set; }

        public override void LogMessage(string switchName, Action<Exception, bool> writeHandledExceptionMessages)
        {
            VRBusinessException ex = new VRBusinessException(string.Format("Database {0} for Switch '{1}' is not synchronised", (ItemIndex + 1).ToString(), switchName), new Exception(ExceptionDetail));
            writeHandledExceptionMessages(ex, true);
        }

        public override string GetUniqueMessageKey(string switchId)
        {
            return string.Format("{0}{1}", switchId, ItemIndex);
        }
    }
}
