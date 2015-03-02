using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vanrise.BusinessProcess.Entities;
using Vanrise.BusinessProcess.Web.Models;

namespace Vanrise.BusinessProcess.Web.ModelMappers
{
    public static class BPMappers
    {
        public static BPTrackingMessageModel MapTrackingMessage(BPTrackingMessage msg)
        {
            return new BPTrackingMessageModel
            {
               
                ProcessInstanceId = msg.ProcessInstanceId,
                ParentProcessId = msg.ParentProcessId,
                Severity = msg.Severity,
                SeverityDescription = msg.Severity.ToString(),               
                Message = msg.Message,
                EventTime = msg.EventTime
            };
        }

        public static List<BPTrackingMessageModel> MapTrackingMessages(List<BPTrackingMessage> msgs)
        {
            List<BPTrackingMessageModel> models = new List<BPTrackingMessageModel>();
            if(msgs != null)
                foreach(var msg in msgs)
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
                StatusDescription = ins.Status.ToString()
               
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
    }
}