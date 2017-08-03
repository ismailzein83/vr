using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using TOne.WhS.RouteSync.Entities;
using TOne.WhS.RouteSync.Radius;
using Vanrise.Entities;

namespace TOne.WhS.RouteSync.MVTSRadius
{
    public class MVTSRadiusSWSyncOutput : SwitchRouteSynchroniserOutput
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
