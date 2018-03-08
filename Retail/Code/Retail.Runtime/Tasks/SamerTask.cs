using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Business;
using Vanrise.AccountBalance.Entities;
using Vanrise.AccountBalance.MainExtensions.BalancePeriod;
using Vanrise.BusinessProcess;
using Vanrise.Queueing;
using Vanrise.Runtime;

namespace Retail.Runtime.Tasks
{
    public class SamerTask : ITask
    {
        public void Execute()
        {
            var runtimeServices = new List<Vanrise.Runtime.Entities.RuntimeService>();
            BusinessProcessService bpService = new BusinessProcessService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(bpService);
            BPRegulatorRuntimeService bpRegulatorService = new BPRegulatorRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            QueueRegulatorRuntimeService queueRegulatorService = new QueueRegulatorRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            QueueActivationRuntimeService queueActivationRuntimeService = new QueueActivationRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            QueueActivationService queueActivationService = new QueueActivationService() { Interval = new TimeSpan(0, 0, 2) };
            SchedulerService schedulerService = new SchedulerService() { Interval = new TimeSpan(0, 0, 2) };
            Vanrise.Integration.Business.DataSourceRuntimeService dsRuntimeService = new Vanrise.Integration.Business.DataSourceRuntimeService { Interval = new TimeSpan(0, 0, 2) };

            runtimeServices.Add(queueActivationService);
            runtimeServices.Add(schedulerService);
          //  runtimeServices.Add(dsRuntimeService);
            runtimeServices.Add(queueActivationRuntimeService);
            runtimeServices.Add(bpRegulatorService);
            runtimeServices.Add(queueRegulatorService);

            RuntimeHost host = new RuntimeHost(runtimeServices);
            host.Start();

        }
    }
}
