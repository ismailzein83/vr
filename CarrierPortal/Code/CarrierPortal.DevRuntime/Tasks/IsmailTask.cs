using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Runtime;
using Vanrise.Runtime.Entities;

namespace CarrierPortal.DevRuntime.Tasks
{
    public class IsmailTask : ITask
    {
        public void Execute()
        {
            GenerateRuntimeNodeConfiguration();
            SchedulerService schedulerService = new SchedulerService() { Interval = new TimeSpan(0, 0, 1) };
            var runtimeServices = new List<RuntimeService>();
           
            runtimeServices.Add(schedulerService);

            RuntimeHost host = new RuntimeHost(runtimeServices);
            host.Start();
        }


        private void GenerateRuntimeNodeConfiguration()
        {
            var runtimeNodeConfigSettings = new Vanrise.Runtime.Entities.RuntimeNodeConfigurationSettings
            {
                Processes = new Dictionary<Guid, RuntimeProcessConfiguration>()
            };
            var processSettings = new RuntimeProcessConfigurationSettings
            {
                NbOfInstances = 1,
                IsEnabled = true,
                Services = new Dictionary<Guid, RuntimeServiceConfiguration>
                    {
                        {
                        new Guid("9C366B5E-6AEA-4A1A-B653-D213283348A7"),
                         new RuntimeServiceConfiguration
                         {
                              Name = "Scheduler Service",
                              Settings = new RuntimeServiceConfigurationSettings
                              {                                   
                                    RuntimeService = new Vanrise.Runtime.SchedulerService{ Interval = TimeSpan.FromSeconds(1) }
                              }
                         }
                         }
                    }
            };
            runtimeNodeConfigSettings.Processes.Add(new Guid("12CEAB1F-27F1-411E-B4EA-C871C7B2BBFE"), new RuntimeProcessConfiguration
            {
                Name = "Scheduler Service Process",
                Settings = processSettings
            });

            processSettings = new RuntimeProcessConfigurationSettings
            {
                NbOfInstances = 2,
                IsEnabled = true,
                Services = new Dictionary<Guid, RuntimeServiceConfiguration>
                    {
                        {
                        new Guid("40D44055-CCB6-40BC-B4D8-1DFA3D6042AD"),
                         new RuntimeServiceConfiguration
                         {
                              Name = "Big Data Service",
                              Settings = new RuntimeServiceConfigurationSettings
                              {                                   
                                    RuntimeService = new Vanrise.Common.Business.BigDataRuntimeService { Interval = TimeSpan.FromSeconds(1) }
                              }
                         }
                         }
                    }
            };
            runtimeNodeConfigSettings.Processes.Add(new Guid("8877FE4C-3DCB-47EA-81F0-C745EA6032E1"), new RuntimeProcessConfiguration
            {
                Name = "Big Data Services Process",
                Settings = processSettings
            });

            var serializedNodeConfigSettings = Vanrise.Common.Serializer.Serialize(runtimeNodeConfigSettings);
        }

    }
}
