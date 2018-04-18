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

        public static bool ShouldStop(this ActivityContext context)
        {
            return context.GetSharedInstanceData().InstanceInfo.Status != Entities.BPInstanceStatus.Running;
        }

        public static void WriteTrackingMessage(this ActivityContext context, LogEntryType severity, string messageFormat, params object[] args)
        {
            context.GetSharedInstanceData().WriteTrackingMessage(severity, messageFormat, args);
        }

        public static void WriteTrackingMessageToParentProcess(this ActivityContext context, LogEntryType severity, string messageFormat, params object[] args)
        {
            context.GetSharedInstanceData().WriteTrackingMessageToParentProcess(severity, messageFormat, args);
        }

        public static void WriteBusinessTrackingMsg(this ActivityContext context, LogEntryType severity, string messageFormat, params object[] args)
        {
            context.GetSharedInstanceData().WriteBusinessTrackingMsg(severity, messageFormat, args);
        }

        public static void WriteHandledException(this ActivityContext context, Exception ex)
        {
            context.GetSharedInstanceData().WriteHandledException(ex);
        }

        public static void WriteBusinessHandledException(this ActivityContext context, Exception ex)
        {
            context.GetSharedInstanceData().WriteBusinessHandledException(ex);
        }
    }
}
