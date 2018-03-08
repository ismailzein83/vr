using QM.CLITester.iTestIntegration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vanrise.Runtime;
using Vanrise.Runtime.Entities;

namespace QM.Runtime.Tasks
{
    public class AliAtouiTask : ITask
    {
        public void Execute()
        {
            //object test = null;
            //ZoneITestReader zoneITestReader = new ZoneITestReader();
            //zoneITestReader.GetChangedItems(ref test);

            #region Runtime
            ExecuteRuntime executeRuntime = new ExecuteRuntime();
            executeRuntime.Runtime_Main();
            #endregion
        }

        public class ExecuteRuntime
        {
            public void Runtime_Main()
            {
                var runtimeServices = new List<RuntimeService>();

                //BusinessProcessService bpService = new BusinessProcessService() { Interval = new TimeSpan(0, 0, 2) };
                //runtimeServices.Add(bpService);

                //QueueRegulatorRuntimeService queueRegulatorService = new QueueRegulatorRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
                //runtimeServices.Add(queueRegulatorService);

                //QueueActivationRuntimeService queueActivationService = new QueueActivationRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
                //runtimeServices.Add(queueActivationService);

                //SummaryQueueActivationRuntimeService summaryQueueActivationService = new SummaryQueueActivationRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
                //runtimeServices.Add(summaryQueueActivationService);

                SchedulerService schedulerService = new SchedulerService() { Interval = new TimeSpan(0, 0, 1) };
                runtimeServices.Add(schedulerService);

                Vanrise.Common.Business.BigDataRuntimeService bigDataService = new Vanrise.Common.Business.BigDataRuntimeService { Interval = new TimeSpan(0, 0, 2) };
                runtimeServices.Add(bigDataService);

                //Vanrise.Integration.Business.DataSourceRuntimeService dsRuntimeService = new Vanrise.Integration.Business.DataSourceRuntimeService { Interval = new TimeSpan(0, 0, 2) };
                //runtimeServices.Add(dsRuntimeService);

                //BPRegulatorRuntimeService bpRegulatorRuntimeService = new BPRegulatorRuntimeService { Interval = new TimeSpan(0, 0, 2) };
                //runtimeServices.Add(bpRegulatorRuntimeService);

                RuntimeHost host = new RuntimeHost(runtimeServices);
                host.Start();

                Console.ReadKey();
            }
        }
    }
}
