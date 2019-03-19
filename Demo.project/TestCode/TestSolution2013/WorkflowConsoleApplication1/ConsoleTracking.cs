using System;
using System.Activities.Tracking;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowConsoleApplication1
{
    public class ConsoleTracking : TrackingParticipant
    {
        
        protected override void Track(TrackingRecord record, TimeSpan timeout)
        {
            ActivityStateRecord activityStateRecord = record as ActivityStateRecord;
            if (activityStateRecord != null)
            {
                Console.WriteLine("{0}: {1} - {2} - {3}", activityStateRecord.EventTime.ToLocalTime(), activityStateRecord.Activity.Name, activityStateRecord.State, activityStateRecord.InstanceId);
                return;
            }

            WorkflowInstanceRecord workflowInstanceRecord = record as WorkflowInstanceRecord;
            if (workflowInstanceRecord != null)
            {
                Console.WriteLine("{0}: Workflow - {1} - {2}", workflowInstanceRecord.EventTime.ToLocalTime(), workflowInstanceRecord.State, workflowInstanceRecord.InstanceId);
                return;
            }
            CustomTrackingRecord customTracking = record as CustomTrackingRecord;
            if(customTracking != null)
            {
                Console.WriteLine("{0}: Workflow - {1} - {2}", customTracking.EventTime.ToLocalTime(), customTracking.Name, customTracking.InstanceId);
                return;
            }

        }
    }
}
