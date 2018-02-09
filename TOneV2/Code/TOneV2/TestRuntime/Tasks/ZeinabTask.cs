using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess;
//using Vanrise.BusinessProcess.Client;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Queueing;
using Vanrise.Runtime;
using TOne.WhS.DBSync.Business;
using TOne.WhS.DBSync.Business.SwitchMigration;
using TOne.WhS.DBSync.Entities;

namespace TestRuntime
{
    public class ZeinabTask : ITask
    {
        public void Execute()
        {
            #region Runtime
            var runtimeServices = new List<RuntimeService>();

            BusinessProcessService bpService = new BusinessProcessService { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(bpService);

            BPRegulatorRuntimeService bpRegulatorRuntimeService = new BPRegulatorRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(bpRegulatorRuntimeService);

            //SchedulerService schedulerService = new SchedulerService { Interval = new TimeSpan(0, 0, 1) };
            //runtimeServices.Add(schedulerService);

            RuntimeHost host = new RuntimeHost(runtimeServices);
            host.Start();
            Console.ReadKey();
            #endregion
        }
    }
}
