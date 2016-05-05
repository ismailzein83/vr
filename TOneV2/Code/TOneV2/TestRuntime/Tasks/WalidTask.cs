using System;
using System.Collections.Generic;
using System.Timers;
using TOne.CDR.Entities;
using TOne.CDR.QueueActivators;
using Vanrise.BusinessProcess;
using Vanrise.Queueing;
using Vanrise.Queueing.Entities;
using Vanrise.Runtime;

namespace TestRuntime.Tasks
{
    public class WalidTask : ITask
    {
        public void Execute()
        {
            //var switches = new List<TOne.WhS.BusinessEntity.Entities.Switch>();
            //switches.Add(new TOne.WhS.BusinessEntity.Entities.Switch { Name = "Switch 1", SwitchId = 4 });
            //TOne.WhS.DBSync.Data.SQL.SwitchDataManager switchManager = new TOne.WhS.DBSync.Data.SQL.SwitchDataManager();

            //switchManager.MigrateSwitchesToDB(switches);

            //BusinessProcessService bpService = new BusinessProcessService() { Interval = new TimeSpan(0, 0, 2) };
            //QueueActivationService queueActivationService = new QueueActivationService() { Interval = new TimeSpan(0, 0, 2) };

            var runtimeServices = new List<RuntimeService>();
            //runtimeServices.Add(queueActivationService);

            //runtimeServices.Add(bpService);
            SchedulerService schedulerService = new SchedulerService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(schedulerService);

            RuntimeHost host = new RuntimeHost(runtimeServices);
            host.Start();

        }

    }
}
