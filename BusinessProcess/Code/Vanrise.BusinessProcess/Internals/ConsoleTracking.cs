using System;
using System.Activities.Tracking;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vanrise.BusinessProcess
{
    public class ConsoleTracking : TrackingParticipant
    {
        internal static ConsoleTracking Instance = new ConsoleTracking();
        protected override void Track(TrackingRecord record, TimeSpan timeout)
        {
            ActivityStateRecord activityStateRecord = record as ActivityStateRecord;
            if (activityStateRecord != null)
            {
                Console.WriteLine("{0}: {1} - {2}", activityStateRecord.EventTime, activityStateRecord.Activity.Name, activityStateRecord.State);
            }
        }
    }
}
