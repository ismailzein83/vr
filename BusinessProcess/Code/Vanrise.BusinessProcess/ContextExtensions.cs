using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;
using Vanrise.Entities;

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

        public static void WriteTrackingMessage(this ActivityContext context, LogEntryType severity, string messageFormat, params object[] args)
        {
            WriteTrackingMessages(false, context, severity, messageFormat, args);
        }

        public static void WriteBusinessTrackingMsg(this ActivityContext context, LogEntryType severity, string messageFormat, params object[] args)
        {
            WriteTrackingMessages(true, context, severity, messageFormat, args);
        }

        public static void WriteHandledException(this ActivityContext context, Exception ex)
        {
            WriteHandledExceptionMessages(false, context, ex);
        }

        public static void WriteBusinessHandledException(this ActivityContext context, Exception ex)
        {
            WriteHandledExceptionMessages(true, context, ex);
        }

        static void WriteHandledExceptionMessages(bool writeBusinessTracking, ActivityContext context, Exception ex)
        {
            BPSharedInstanceData instanceData = context.GetSharedInstanceData();
            BPTrackingMessage trackingMessage = new BPTrackingMessage
            {
                TrackingMessage = Utilities.GetExceptionBusinessMessage(ex),
                ExceptionDetail = ex.ToString(),
                EventTime = DateTime.Now,
                ProcessInstanceId = instanceData.InstanceInfo.ProcessInstanceID,
                ParentProcessId = instanceData.InstanceInfo.ParentProcessID,
                Severity = LogEntryType.Warning
            }; 
            BPTrackingChannel.Current.WriteTrackingMessage(trackingMessage);
            if (writeBusinessTracking)
                LoggerFactory.GetExceptionLogger().WriteException(instanceData.InstanceInfo.InputArgument.GetDefinitionTitle(), ex);
        }

        static void WriteTrackingMessages(bool writeBusinessTracking, ActivityContext context, LogEntryType severity, string messageFormat, params object[] args)
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
            if (writeBusinessTracking)
                LoggerFactory.GetLogger().WriteEntry(instanceData.InstanceInfo.InputArgument.GetDefinitionTitle(), severity, messageFormat, args);
        }
    }
}
