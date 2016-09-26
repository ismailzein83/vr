using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.BusinessProcess
{
    public class BPContext : IBPContext
    {
        AsyncActivityHandle _asyncActivityHandle;
        ActivityContext _activityContext;
        public BPContext(AsyncActivityHandle asyncActivityHandle)
        {
            if (asyncActivityHandle == null)
                throw new ArgumentNullException("asyncActivityHandle");
            _asyncActivityHandle = asyncActivityHandle;
        }

        public BPContext(ActivityContext activityContext)
        {
            if (activityContext == null)
                throw new ArgumentNullException("activityContext");
            _activityContext = activityContext;
        }

        public void WriteTrackingMessage(LogEntryType severity, string messageFormat, params object[] args)
        {
            if (_activityContext != null)
                _activityContext.WriteTrackingMessage(severity, messageFormat, args);
            else
                _asyncActivityHandle.SharedInstanceData.WriteTrackingMessage(severity, messageFormat, args);

        }
    }
}
