using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vanrise.BusinessProcess.Entities;
using Vanrise.BusinessProcess.Web.Models;
using Vanrise.Runtime.Entities;

namespace Vanrise.BusinessProcess.Web.ModelMappers
{
    public class BPMappers
    {
        public static BPTrackingMessageModel MapTrackingMessage(BPTrackingMessage msg)
        {
            return new BPTrackingMessageModel
            {
                TrackingId = msg.Id,
                ProcessInstanceId = msg.ProcessInstanceId,
                ParentProcessId = msg.ParentProcessId,
                Severity = msg.Severity,
                SeverityDescription = msg.Severity.ToString(),
                Message = msg.TrackingMessage,
                EventTime = msg.EventTime
            };
        }

        public static List<BPTrackingMessageModel> MapTrackingMessages(List<BPTrackingMessage> msgs)
        {
            List<BPTrackingMessageModel> models = new List<BPTrackingMessageModel>();
            if (msgs != null)
                foreach (var msg in msgs)
                {
                    models.Add(MapTrackingMessage(msg));
                }
            return models;
        }


        public static BPInstanceModel MapInstance(BPInstance ins)
        {
            return new BPInstanceModel
            {
                ProcessInstanceID = ins.ProcessInstanceID,
                Title = ins.Title,
                ParentProcessID = ins.ProcessInstanceID,
                DefinitionID = ins.DefinitionID,
                WorkflowInstanceID = ins.WorkflowInstanceID,
                Status = ins.Status,
                RetryCount = ins.RetryCount,
                InputArgument = ins.InputArgument,
                LastMessage = ins.LastMessage,
                CreatedTime = ins.CreatedTime,
                StatusDescription = ins.Status.ToString(),
                StatusUpdatedTime = ins.StatusUpdatedTime

            };
        }

        public static List<BPInstanceModel> MapTMapInstances(List<BPInstance> instances)
        {
            List<BPInstanceModel> models = new List<BPInstanceModel>();
            if (instances != null)
                foreach (var ins in instances)
                {
                    models.Add(MapInstance(ins));
                }
            return models;
        }


        public static SchedulerTaskModel MapSchedulerTask(SchedulerTask task)
        {
            return new SchedulerTaskModel
            {
                TaskId = task.TaskId,
                Name = task.Name,
                IsEnabled = task.IsEnabled,
                Status = task.Status,
                NextRunTime = task.NextRunTime,
                LastRunTime = task.LastRunTime,
                ActionTypeId = task.ActionTypeId,
                ActionInfo = task.ActionInfo,
                TriggerInfo = task.TriggerInfo,
                TaskSettings = task.TaskSettings, 
                StatusDescription = SchedulerTask.GetEnumDescription(task.Status)
            };
        }

        public static List<SchedulerTaskModel> MapTMapSchedulerTasks(List<SchedulerTask> tasks)
        {
            List<SchedulerTaskModel> models = new List<SchedulerTaskModel>();
            if (tasks != null)
                foreach (var task in tasks)
                {
                    models.Add(MapSchedulerTask(task));
                }
            return models;
        }


    }
}