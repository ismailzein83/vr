using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.BusinessProcess
{
    public static class ContextExtensions
    {
        public static BPSharedInstanceData GetSharedInstanceData(this ActivityContext context)
        {
            var sharedData = context.GetExtension<BPSharedInstanceData>();
            if (sharedData == null)
                throw new NullReferenceException("BPSharedInstanceData");
            return sharedData;
        }

        public static void WriteTrackingMessage(this ActivityContext context, BPTrackingSeverity severity, string messageFormat, params object[] args)
        {
            BPSharedInstanceData instanceData = context.GetSharedInstanceData();
            BPTrackingMessage trackingMessage = new BPTrackingMessage
            {
                TrackingMessage = String.Format(messageFormat, args),
                EventTime = DateTime.Now,
                ProcessInstanceId = instanceData.InstanceInfo.ProcessInstanceID,
                ParentProcessId = instanceData.InstanceInfo.ParentProcessID,
                Severity = severity
            };
            BPTrackingChannel.Current.WriteTrackingMessage(trackingMessage);
        }
    }
}
