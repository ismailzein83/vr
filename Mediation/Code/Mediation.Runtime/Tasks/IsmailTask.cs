using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Runtime.Entities;

namespace Mediation.Runtime.Tasks
{
    class IsmailTask : ITask
    {
        public void Execute()
        {
            GenerateRuntimeNodeConfiguration();
            Console.WriteLine("Ismail Task");
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
                        new Guid("F650AEBC-2AA7-4ECF-8315-5806193AD1EB"),
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
            runtimeNodeConfigSettings.Processes.Add(new Guid("CB423A72-2FF4-41DB-BC08-AE9C72F9BC4A"), new RuntimeProcessConfiguration
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
                        new Guid("DB5D6A50-C95D-4A12-98DA-9472BEB444A5"),
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
            runtimeNodeConfigSettings.Processes.Add(new Guid("2F0EF0FC-E93F-4403-81A9-4DD5250C9615"), new RuntimeProcessConfiguration
            {
                Name = "Business Process Regulator Service Process",
                Settings = processSettings
            });

            processSettings = new RuntimeProcessConfigurationSettings
            {
                NbOfInstances = 5,
                IsEnabled = true,
                Services = new Dictionary<Guid, RuntimeServiceConfiguration>
                    {
                        {
                        new Guid("5BCFF6F4-A778-4F1D-AD6E-6618D0EFDADE"),
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
            runtimeNodeConfigSettings.Processes.Add(new Guid("F69ECCF4-D63F-45F1-A3F3-93AC51BEE26D"), new RuntimeProcessConfiguration
            {
                Name = "Business Process Services Process",
                Settings = processSettings
            });

            var serializedNodeConfigSettings1 = Vanrise.Common.Serializer.Serialize(runtimeNodeConfigSettings);

            processSettings = new RuntimeProcessConfigurationSettings
            {
                NbOfInstances = 1,
                IsEnabled = true,
                Services = new Dictionary<Guid, RuntimeServiceConfiguration>
                    {
                        {
                        new Guid("6B691C9C-03D9-4B88-AF9F-AE75C3FAC991"),
                         new RuntimeServiceConfiguration
                         {
                              Name = "Queue Regulator Service",
                              Settings = new RuntimeServiceConfigurationSettings
                              {                                   
                                    RuntimeService = new Vanrise.Queueing.QueueRegulatorRuntimeService { Interval = TimeSpan.FromSeconds(1) }
                              }
                         }
                         }
                    }
            };
            runtimeNodeConfigSettings.Processes.Add(new Guid("6AAE4D1F-7C33-4B42-A4CB-C8771C913177"), new RuntimeProcessConfiguration
            {
                Name = "Queue Regulator Service Process",
                Settings = processSettings
            });

            processSettings = new RuntimeProcessConfigurationSettings
            {
                NbOfInstances = 5,
                IsEnabled = true,
                Services = new Dictionary<Guid, RuntimeServiceConfiguration>
                    {
                        {
                        new Guid("784784B8-DCB2-4AF0-81E8-16800C350057"),
                         new RuntimeServiceConfiguration
                         {
                              Name = "Queue Activation Service",
                              Settings = new RuntimeServiceConfigurationSettings
                              {                                   
                                    RuntimeService = new Vanrise.Queueing.QueueActivationRuntimeService { Interval = TimeSpan.FromSeconds(1) }
                              }
                         }
                         }
                    }
            };
            runtimeNodeConfigSettings.Processes.Add(new Guid("C04A9F6F-CCA6-48A4-86E7-66BC49DE7A7D"), new RuntimeProcessConfiguration
            {
                Name = "Queue Activation Services Process",
                Settings = processSettings
            });

            processSettings = new RuntimeProcessConfigurationSettings
            {
                NbOfInstances = 3,
                IsEnabled = true,
                Services = new Dictionary<Guid, RuntimeServiceConfiguration>
                    {
                        {
                        new Guid("B61C1448-FF9B-45A0-B3C4-3CD3744DC213"),
                         new RuntimeServiceConfiguration
                         {
                              Name = "Data Source Service",
                              Settings = new RuntimeServiceConfigurationSettings
                              {                                   
                                    RuntimeService = new Vanrise.Integration.Business.DataSourceRuntimeService { Interval = TimeSpan.FromSeconds(1) }
                              }
                         }
                         }
                    }
            };
            runtimeNodeConfigSettings.Processes.Add(new Guid("29FBFD14-03F3-4F59-918F-1B20F683E286"), new RuntimeProcessConfiguration
            {
                Name = "Data Source Services Process",
                Settings = processSettings
            });

            var serializedNodeConfigSettings = Vanrise.Common.Serializer.Serialize(runtimeNodeConfigSettings);
        }
    }
}
