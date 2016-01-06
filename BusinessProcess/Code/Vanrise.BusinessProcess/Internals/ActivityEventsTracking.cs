using System;
using System.Activities.Tracking;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.BusinessProcess
{
    internal class ActivityEventsTracking: TrackingParticipant
    {
        BPInstance _bpInstance;
        public ActivityEventsTracking(BPInstance bpInstance)
        {
            _bpInstance = bpInstance;
        }
        protected override void Track(TrackingRecord record, TimeSpan timeout)
        {
            
            string trackingMessage = null;
            ActivityStateRecord activityStateRecord = record as ActivityStateRecord;
            if (activityStateRecord != null)
            {
                trackingMessage = string.Format("{0} - {1}", activityStateRecord.Activity.Name, activityStateRecord.State);
            }
            else
            {
                WorkflowInstanceRecord workflowInstanceRecord = record as WorkflowInstanceRecord;
                if (workflowInstanceRecord != null)
                {
                    trackingMessage = string.Format("Workflow - {0}", workflowInstanceRecord.State);
                }
            }
            if (trackingMessage != null)
            {
                BPTrackingChannel.Current.WriteTrackingMessage(new Entities.BPTrackingMessage
                {
                    ProcessInstanceId = _bpInstance.ProcessInstanceID,
                    TrackingMessage = trackingMessage,
                    EventTime = record.EventTime.ToLocalTime(),
                    Severity = Common.LogEntryType.Verbose
                });
            }
        }
    }
}
