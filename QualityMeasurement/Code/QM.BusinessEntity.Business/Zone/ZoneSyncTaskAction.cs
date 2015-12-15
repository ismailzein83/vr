using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Runtime.Entities;

namespace QM.BusinessEntity.Business
{
    public class ZoneSyncTaskAction : SchedulerTaskAction
    {
        public override void Execute(SchedulerTask task, BaseTaskActionArgument taskActionArgument, Dictionary<string, object> evaluatedExpressions)
        {
            ZoneSyncTaskActionArgument zoneSyncTaskActionArgument = taskActionArgument as ZoneSyncTaskActionArgument;
            SourceZoneSynchronizer sourceZoneSynchronizer = new SourceZoneSynchronizer(zoneSyncTaskActionArgument.SourceZoneReader,zoneSyncTaskActionArgument.SourceCountryReader);
            sourceZoneSynchronizer.Synchronize();
            Console.WriteLine("Zone SyncTaskAction Executed");
        }
    }
}
