using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TestRuntime;
using Vanrise.BusinessProcess;
using Vanrise.Queueing;
using Vanrise.Runtime;
using Vanrise.Runtime.Entities;

namespace TOne.WhS.Runtime.Tasks
{
    public class RodiTask : ITask
    {
        public void Execute()
        {
            #region Runtime
            var runtimeServices = new List<RuntimeService>();

            BusinessProcessService bpService = new BusinessProcessService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(bpService);
            BPRegulatorRuntimeService bpRegulatorService = new BPRegulatorRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            QueueRegulatorRuntimeService queueRegulatorService = new QueueRegulatorRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            QueueActivationRuntimeService queueActivationRuntimeService = new QueueActivationRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            QueueActivationService queueActivationService = new QueueActivationService() { Interval = new TimeSpan(0, 0, 2) };
            SchedulerService schedulerService = new SchedulerService() { Interval = new TimeSpan(0, 0, 2) };
            Vanrise.Integration.Business.DataSourceRuntimeService dsRuntimeService = new Vanrise.Integration.Business.DataSourceRuntimeService { Interval = new TimeSpan(0, 0, 2) };

            runtimeServices.Add(queueActivationService);
            // runtimeServices.Add(schedulerService);
            runtimeServices.Add(dsRuntimeService);
            runtimeServices.Add(queueActivationRuntimeService);
            runtimeServices.Add(bpRegulatorService);
            runtimeServices.Add(queueRegulatorService);

            var assemblies = new List<Assembly>();

            var tOneFiles = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "TOne*Entities.dll");
            var vanriseFiles = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "Vanrise*Entities.dll");

            foreach (var file in tOneFiles)
                assemblies.Add(Assembly.LoadFile(file));
            foreach (var file in vanriseFiles)
                assemblies.Add(Assembly.LoadFile(file));

            Vanrise.Common.Business.UtilityManager.GenerateDocumentationForEnums(assemblies);

            RuntimeHost host = new RuntimeHost(runtimeServices);
            host.Start();

            #endregion
        }
    }
}
