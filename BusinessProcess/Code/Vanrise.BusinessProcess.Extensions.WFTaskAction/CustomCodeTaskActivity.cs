using System;
using System.Activities;
using System.Collections.Generic;
using Vanrise.BusinessProcess;
using Vanrise.BusinessProcess.Entities;
using Vanrise.BusinessProcess.Business;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Runtime.Entities;
using Vanrise.BusinessProcess.Extensions.WFTaskAction.Arguments;
using Vanrise.Runtime.Business;
using Vanrise.Entities;
using Vanrise.BusinessProcess.Extensions.WFTaskAction.Arguments.UCT;

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
            var executor = Activator.CreateInstance(executorType) as ICustomCodeHandler;
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

            public void SendMail(UctMailMessage mailMessage)
            {
                List<VRMailAttachement> vrMailMessageAttachments = new List<VRMailAttachement>();
                if(mailMessage.Attachments!=null)
                {
                    vrMailMessageAttachments = new List<VRMailAttachement>();
                     foreach(var attachment in mailMessage.Attachments)
                         vrMailMessageAttachments.Add(new VRMailAttachmentExcel(){
                            Name= attachment.Name,
                            Content = attachment.Content
                         });
                }
                new VRMailManager().SendMail(mailMessage.From, mailMessage.To, mailMessage.Cc, null, mailMessage.Subject, mailMessage.Body, vrMailMessageAttachments, false);
            }

        }

    }
}
