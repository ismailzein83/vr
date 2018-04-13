using System;
using System.Activities;
using System.Collections.Generic;
using Vanrise.BusinessProcess;
using Vanrise.BusinessProcess.Entities;
using Vanrise.BusinessProcess.Business;
using Vanrise.Common;
using Vanrise.Runtime.Entities;
using Vanrise.BusinessProcess.Extensions.WFTaskAction.Arguments;
using Vanrise.Runtime.Business;
using Vanrise.Entities;

namespace Vanrise.BusinessProcess.WFActivities
{
    public sealed class CustomCodeTaskActivity : BaseCodeActivity
    {

        protected override void VRExecute(IBaseCodeActivityContext context)
        {
            CustomCodeTaskManager _customCodeTaskManager = new CustomCodeTaskManager();
            var taskId = context.ActivityContext.GetSharedInstanceData().InstanceInfo.TaskId;
            if (!taskId.HasValue)
                throw new NullReferenceException("Task Id");
            var executorType = _customCodeTaskManager.GetCachedCustomCodeTaskTypeByTaskId(taskId.Value);
            executorType.ThrowIfNull("Executor Type");
            var executor = Activator.CreateInstance(executorType) as  ICustomCodeHandler;
            var customCodeContext = new CustomCodeExecutionContext(context);
            executor.Execute(customCodeContext);
        }

        private class CustomCodeExecutionContext : ICustomCodeExecutionContext
        {
            IBaseCodeActivityContext _context;
            public CustomCodeExecutionContext(IBaseCodeActivityContext context)
            {
                _context = context;
            }
            public void LogException(Exception ex)
            {
                _context.ActivityContext.WriteHandledException(ex);
            }

            public void LogError(string messageFormat, params object[] args)
            {
                _context.ActivityContext.WriteTrackingMessage(LogEntryType.Error, messageFormat, args);
            }

            public void LogWarning(string messageFormat, params object[] args)
            {
                _context.ActivityContext.WriteTrackingMessage(LogEntryType.Warning, messageFormat, args);
            }

            public void LogInfo(string messageFormat, params object[] args)
            {
                _context.ActivityContext.WriteTrackingMessage(LogEntryType.Information, messageFormat, args);
            }

            public void SendMail(string from, string to, string cc, string subject, string body)
            {
                throw new NotImplementedException();
            }
        }
       
    }
}
