using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Runtime.Entities;

namespace SOM.Runtime.Tasks
{
    class IsmailTask : ITask
    {
        public void Execute()
        {
            GenerateRuntimeNodeConfiguration();
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
                        new Guid("30BE260C-C77E-4861-8C03-653CC52F9791"),
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
            runtimeNodeConfigSettings.Processes.Add(new Guid("0FD035C0-64D7-4D5A-9888-AC03E48C4096"), new RuntimeProcessConfiguration
            {
                Name = "Scheduler Service Process",
                Settings = processSettings
            });

            processSettings = new RuntimeProcessConfigurationSettings
            {
                NbOfInstances = 1,
                IsEnabled = true,
                Services = new Dictionary<Guid, RuntimeServiceConfiguration>
                    {
                        {
                        new Guid("59E3194A-62BB-4E9D-B2A8-7B497F96F26A"),
                         new RuntimeServiceConfiguration
                         {
                              Name = "Business Process Regulator Service",
                              Settings = new RuntimeServiceConfigurationSettings
                              {                                   
                                    RuntimeService = new Vanrise.BusinessProcess.BPRegulatorRuntimeService { Interval = TimeSpan.FromSeconds(1) }
                              }
                         }
                         }
                    }
            };
            runtimeNodeConfigSettings.Processes.Add(new Guid("949323E6-04BB-4D88-A722-AFC1BF19C751"), new RuntimeProcessConfiguration
            {
                Name = "Business Process Regulator Service Process",
                Settings = processSettings
            });

            processSettings = new RuntimeProcessConfigurationSettings
            {
                NbOfInstances = 3,
                IsEnabled = true,
                Services = new Dictionary<Guid, RuntimeServiceConfiguration>
                    {
                        {
                        new Guid("E4651BE4-0191-450E-9786-9F52F2C3F974"),
                         new RuntimeServiceConfiguration
                         {
                              Name = "Business Process Service",
                              Settings = new RuntimeServiceConfigurationSettings
                              {                                   
                                    RuntimeService = new Vanrise.BusinessProcess.BusinessProcessService { Interval = TimeSpan.FromSeconds(1) }
                              }
                         }
                         }
                    }
            };
            runtimeNodeConfigSettings.Processes.Add(new Guid("C337541D-1AF7-44ED-8B27-D1A57D9D628F"), new RuntimeProcessConfiguration
            {
                Name = "Business Process Services Process",
                Settings = processSettings
            });


            var serializedNodeConfigSettings = Vanrise.Common.Serializer.Serialize(runtimeNodeConfigSettings);
        }

    }
}
