using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Runtime;
using Vanrise.Runtime.Entities;

namespace Mediation.Runtime.Tasks
{
    class IsmailTask : ITask
    {
        public void Execute()
        {
            RuntimeHost host = new RuntimeHost(new List<RuntimeService>());
            host.Start();
            TestFileRecordStorage();
            //GenerateRuntimeNodeConfiguration();
            Console.WriteLine("Ismail Task");
        }

        private void TestFileRecordStorage()
        {
            RecordFilterGroup filterGroup = new RecordFilterGroup { LogicalOperator = RecordQueryLogicalOperator.And, Filters = new List<RecordFilter>() };
            filterGroup.Filters.Add(new NumberListRecordFilter { FieldName = "RecordType", CompareOperator = ListRecordFilterOperator.In, Values = new List<decimal>() { 0, 1, 100 } });

            CompareCDRsFromFileAndSQLStorages("2018-07-01", "2018-12-01", null);
            CompareCDRsFromFileAndSQLStorages("2018-07-01", "2018-12-01", filterGroup);
            CompareCDRsFromFileAndSQLStorages("2018-07-26 20:02:18.000", "2018-07-27 23:34:41.000", null);
            CompareCDRsFromFileAndSQLStorages("2018-07-26 20:02:18.000", "2018-07-27 23:34:41.000", filterGroup);
            CompareCDRsFromFileAndSQLStorages("2018-07-28 08:06:48.000", "2018-07-27 23:34:41.000", null);
            CompareCDRsFromFileAndSQLStorages("2018-07-28 08:06:48.000", "2018-07-27 23:34:41.000", filterGroup);
            CompareCDRsFromFileAndSQLStorages("2018-07-26 20:02:18.000", "2018-10-22 08:33:15.000", null);
            CompareCDRsFromFileAndSQLStorages("2018-07-26 20:02:18.000", "2018-10-22 08:33:15.000", filterGroup);
            CompareCDRsFromFileAndSQLStorages("2018-07-29 08:06:48.000", "2018-07-21 08:33:15.000", null);
            CompareCDRsFromFileAndSQLStorages("2018-07-29 08:06:48.000", "2018-10-21 08:33:15.000", filterGroup);
            Console.WriteLine("Test Done");
        }

        private static void CompareCDRsFromFileAndSQLStorages(string fromAsString, string toAsString, RecordFilterGroup filterGroup)
        {
            var dataRecordStorageManager = new DataRecordStorageManager();
            DateTime from = DateTime.Parse(fromAsString);
            DateTime to = DateTime.Parse(toAsString);
            Guid sqlMobileCDRStorageId = new Guid("2b740017-1a95-486b-894f-09b5d092686f");
            Guid fileMobileCDRStorageId = new Guid("b182a12d-4875-4db1-a118-434c78df6460");
            List<dynamic> sqlCDRs = new List<dynamic>();
            dynamic lastSQLCDR = null;
            dataRecordStorageManager.GetDataRecords(sqlMobileCDRStorageId, from, to, filterGroup, () => false,
                (cdr) =>
                {
                    if (lastSQLCDR != null && lastSQLCDR.SetupTime > cdr.SetupTime)
                        Console.WriteLine("received not ordered SQL CDR");
                    sqlCDRs.Add(cdr);
                }, "SetupTime", true);

            sqlCDRs = sqlCDRs.OrderBy(cdr => cdr.SetupTime).ThenBy(cdr => cdr.UniqueIdentifier).ThenBy(cdr => cdr.CallingNumber).ThenBy(cdr => cdr.CalledNumber).ToList();

            dynamic lastCDR = null;
            List<dynamic> fileCDRs = new List<dynamic>();
            dataRecordStorageManager.GetDataRecords(fileMobileCDRStorageId, from, to, filterGroup, () => false,
                (cdr) =>
                {
                    if (lastCDR != null && lastCDR.SetupTime > cdr.SetupTime)
                        Console.WriteLine($"received not ordered File CDR. Record Id '{cdr.Id}'. Previous Record Id '{lastCDR.Id}'");
                    lastCDR = cdr;
                    fileCDRs.Add(cdr);
                }, "SetupTime", true);

            fileCDRs = fileCDRs.OrderBy(cdr => cdr.SetupTime).ThenBy(cdr => cdr.UniqueIdentifier).ThenBy(cdr => cdr.CallingNumber).ThenBy(cdr => cdr.CalledNumber).ToList();

            if (sqlCDRs.Count != fileCDRs.Count)
            {
                Console.WriteLine($"CompareCDRsFromFileAndSQLStorages failed. different count. SQL Count '{sqlCDRs.Count}', File Count '{fileCDRs.Count}'");
                return;
            }
            for(int i=0;i<sqlCDRs.Count;i++)
            {
                var sqlCDR = sqlCDRs[i];
                var fileCDR = fileCDRs[i];                
                string serializedSQLCDR = Serializer.Serialize(CleanCDR(sqlCDR));
                string serializedFileCDR = Serializer.Serialize(CleanCDR(fileCDR));
                if (serializedFileCDR != serializedSQLCDR)
                {
                    Console.WriteLine($"CompareCDRsFromFileAndSQLStorages failed. different CDRs Index '{i}'. SQL Id '{sqlCDR.Id}', File Id '{fileCDR.Id}'");
                    return;
                }
            }
            Console.WriteLine($"{DateTime.Now}: CompareCDRsFromFileAndSQLStorages succeeded. CDR Count: {sqlCDRs.Count}");
            
        }

        private static dynamic CleanCDR(dynamic cdr)
        {
            var cdrCopy = Vanrise.Common.ExtensionMethods.VRDeepCopy(cdr);
            cdrCopy.Id = 0;
            return cdrCopy;
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
