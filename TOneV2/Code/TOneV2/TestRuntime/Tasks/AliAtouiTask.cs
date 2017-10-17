using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TestRuntime;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Deal.Business;
using TOne.WhS.Deal.Entities;
using TOne.WhS.Routing.Data.SQL;
using TOne.WhS.Routing.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Caching.Runtime;
using Vanrise.Common.Business;
using Vanrise.Integration.Entities;
using Vanrise.Integration.Mappers;
using Vanrise.Queueing;
using Vanrise.Runtime;

namespace TOne.WhS.Runtime.Tasks
{
    public class AliAtouiTask : ITask
    {
        #region Public Methods

        public void Execute()
        {
            #region Runtime
            //ExecuteRuntime executeRuntime = new ExecuteRuntime();
            //executeRuntime.Runtime_Main();
            #endregion

            #region PrepareCodePrefixesTask
            //PrepareCodePrefixesTask prepareCodePrefixesTask = new PrepareCodePrefixesTask();
            //IEnumerable<CodePrefixInfo> codePrefixesResult = prepareCodePrefixesTask.PrepareCodePrefixes_Main();
            //DisplayList(codePrefixesResult);
            #endregion

            #region VRMailMessageTemplateTask
            //VRMailMessageTemplateTask vrMailMessageTemplateTask = new VRMailMessageTemplateTask();
            //vrMailMessageTemplateTask.VRMailMessageTemplate_Main();
            #endregion

            #region Deserialize
            string serializedOptionsDetailsBySupplier = "75~970$22468$0.13000000$1@2$$False$296543~0~1~30~4";
            string serializedOptionsByPolicy = "6d584c11-ce52-4385-a871-3b59505d0f57~75$0.13000000$100$0$11451$0$4$False|cb8cc5ed-afda-4ed7-882d-1377666c141e~75$0.13000000$100$0$11451$0$4$False|e85f9e2f-1ce6-4cc3-9df9-b664e63826f5~75$0.13000000$100$0$11451$0$4$False";

            Deserialize deserialize = new Deserialize();
            string optionsDetailsBySupplierAsJSON = deserialize.DeserializeOptionsDetailsBySupplier(serializedOptionsDetailsBySupplier);
            string OptionsByPolicyAsJSON = deserialize.DeserializeOptionsByPolicy(serializedOptionsByPolicy);
            #endregion 
        }

        public static MappingOutput DataSourceMapData(IImportedData data, MappedBatchItemsToEnqueue mappedBatches)
        {
            var cdrs = new List<dynamic>();
            var dataRecordTypeManager = new Vanrise.GenericData.Business.DataRecordTypeManager();
            Type cdrRuntimeType = dataRecordTypeManager.GetDataRecordRuntimeType("CDR");

            long startingId;
            int batchSize = 50000;
            var dataRecordVanriseType = new Vanrise.GenericData.Entities.DataRecordVanriseType("CDR");

            Vanrise.Common.Business.IDManager.Instance.ReserveIDRange(dataRecordVanriseType, batchSize, out startingId);

            var importedData = ((Vanrise.Integration.Entities.DBReaderImportedData)(data));

            IDataReader reader = importedData.Reader;

            long currentCDRId = startingId;
            int rowCount = 0;
            while (reader.Read())
            {
                dynamic cdr = Activator.CreateInstance(cdrRuntimeType) as dynamic;
                cdr.Id = currentCDRId;
                cdr.SwitchId = 83;
                cdr.IDonSwitch = Utils.GetReaderValue<long>(reader, "IDonSwitch");
                cdr.Tag = reader["Tag"] as string;
                cdr.AttemptDateTime = (DateTime)reader["AttemptDateTime"];
                cdr.AlertDateTime = Utils.GetReaderValue<DateTime>(reader, "AlertDateTime");
                cdr.ConnectDateTime = Utils.GetReaderValue<DateTime>(reader, "ConnectDateTime");
                cdr.DisconnectDateTime = Utils.GetReaderValue<DateTime>(reader, "DisconnectDateTime");
                cdr.DurationInSeconds = Utils.GetReaderValue<Decimal>(reader, "DurationInSeconds");
                cdr.InTrunk = reader["IN_TRUNK"] as string;
                cdr.InCircuit = reader["IN_CIRCUIT"] != DBNull.Value ? Convert.ToInt64(reader["IN_CIRCUIT"]) : default(Int64);
                cdr.InCarrier = reader["IN_CARRIER"] as string;
                cdr.InIP = reader["IN_IP"] as string;
                cdr.OutTrunk = reader["OUT_TRUNK"] as string;
                cdr.OutCircuit = reader["OUT_CIRCUIT"] != DBNull.Value ? Convert.ToInt64(reader["OUT_CIRCUIT"]) : default(Int64);
                cdr.OutCarrier = reader["OUT_CARRIER"] as string;
                cdr.OutIP = reader["OUT_IP"] as string;

                cdr.CGPN = reader["CGPN"] as string;
                cdr.CDPN = reader["CDPN"] as string;
                cdr.CauseFromReleaseCode = reader["CAUSE_FROM_RELEASE_CODE"] as string;
                cdr.CauseFrom = reader["CAUSE_FROM"] as string;
                cdr.CauseToReleaseCode = reader["CAUSE_TO_RELEASE_CODE"] as string;
                cdr.CauseTo = reader["CAUSE_TO"] as string;
                cdr.IsRerouted = reader["IsRerouted"] != DBNull.Value ? ((reader["IsRerouted"] as string) == "Y") : false;
                cdr.CDPNOut = reader["CDPNOut"] as string;
                cdr.CDPNIn = reader["CDPNIn"] as string;
                cdr.SIP = reader["SIP"] as string;

                cdrs.Add(cdr);
                importedData.LastImportedId = reader["CDRID"];

                currentCDRId++;
                rowCount++;
                if (rowCount == batchSize)
                    break;

            }
            if (cdrs.Count > 0)
            {
                var batch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(cdrs, "#RECORDSCOUNT# of Raw CDRs", "CDR");
                mappedBatches.Add("Distribute Raw CDRs Stage", batch);
            }
            else
                importedData.IsEmpty = true;

            Vanrise.Integration.Entities.MappingOutput result = new Vanrise.Integration.Entities.MappingOutput();
            result.Result = Vanrise.Integration.Entities.MappingResult.Valid;

            return result;
        }

        #endregion

        #region Private Methods

        private void DisplayList(IEnumerable<CodePrefixInfo> codePrefixes)
        {
            foreach (CodePrefixInfo item in codePrefixes)
                Console.WriteLine(item.CodePrefix + "   " + item.Count);

            Console.WriteLine("\n");
        }

        private static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        #endregion
    }

    public class ExecuteRuntime
    {
        public void Runtime_Main()
        {
            var runtimeServices = new List<RuntimeService>();

            SchedulerService schedulerService = new SchedulerService() { Interval = new TimeSpan(0, 0, 1) };
            runtimeServices.Add(schedulerService);

            BPRegulatorRuntimeService bpRegulatorRuntimeService = new BPRegulatorRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(bpRegulatorRuntimeService);

            BusinessProcessService bpService = new BusinessProcessService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(bpService);

            QueueRegulatorRuntimeService queueRegulatorService = new QueueRegulatorRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(queueRegulatorService);

            QueueActivationRuntimeService queueActivationService = new QueueActivationRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(queueActivationService);

            SummaryQueueActivationRuntimeService summaryQueueActivationService = new SummaryQueueActivationRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(summaryQueueActivationService);

            Vanrise.Integration.Business.DataSourceRuntimeService dsRuntimeService = new Vanrise.Integration.Business.DataSourceRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(dsRuntimeService);

            Vanrise.Common.Business.BigDataRuntimeService bigDataService = new Vanrise.Common.Business.BigDataRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(bigDataService);

            CachingRuntimeService cachingRuntimeService = new CachingRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(cachingRuntimeService);

            CachingDistributorRuntimeService cachingDistributorRuntimeService = new CachingDistributorRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(cachingDistributorRuntimeService);

            DataGroupingExecutorRuntimeService dataGroupingExecutorRuntimeService = new Vanrise.Common.Business.DataGroupingExecutorRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(dataGroupingExecutorRuntimeService);

            DataGroupingDistributorRuntimeService dataGroupingDistributorRuntimeService = new Vanrise.Common.Business.DataGroupingDistributorRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(dataGroupingDistributorRuntimeService);

            RuntimeHost host = new RuntimeHost(runtimeServices);
            host.Start();

            Console.ReadKey();
        }
    }

    public class PrepareCodePrefixesTask
    {
        #region Public Method
        public IEnumerable<CodePrefixInfo> PrepareCodePrefixes_Main()
        {
            Console.WriteLine("Ali Atoui: PrepareCodePrefixes");

            //Dictionaries
            Dictionary<string, CodePrefixInfo> codePrefixes = new Dictionary<string, CodePrefixInfo>();
            Dictionary<string, CodePrefixInfo> pendingCodePrefixes = new Dictionary<string, CodePrefixInfo>();

            //Initializint Settings
            SettingManager settingManager = new SettingManager();
            RouteSettingsData settings = settingManager.GetSetting<RouteSettingsData>(Routing.Business.Constants.RouteSettings);
            int threshold = settings.SubProcessSettings.CodeRangeCountThreshold;
            int maxPrefixLength = settings.SubProcessSettings.MaxCodePrefixLength;
            int prefixLength = 1;
            DateTime? effectiveOn = DateTime.Now;
            bool isFuture = false;


            SupplierCodeManager supplierCodeManager = new SupplierCodeManager();
            IEnumerable<CodePrefixInfo> supplierCodePrefixes = supplierCodeManager.GetDistinctCodeByPrefixes(prefixLength, effectiveOn, isFuture);
            AddCodePrefixes(supplierCodePrefixes, pendingCodePrefixes);

            SaleCodeManager saleCodeManager = new SaleCodeManager();
            IEnumerable<CodePrefixInfo> saleCodePrefixes = saleCodeManager.GetDistinctCodeByPrefixes(prefixLength, effectiveOn, isFuture);
            AddCodePrefixes(saleCodePrefixes, pendingCodePrefixes);

            DisplayDictionary(pendingCodePrefixes);

            if (maxPrefixLength == 1)
                return pendingCodePrefixes.Values.OrderByDescending(x => x.Count);

            CheckThreshold(pendingCodePrefixes, codePrefixes, threshold);

            while (pendingCodePrefixes.Count > 0 && prefixLength < maxPrefixLength)
            {
                prefixLength++;

                IEnumerable<string> _pendingCodePrefixes = pendingCodePrefixes.Keys;
                pendingCodePrefixes = new Dictionary<string, CodePrefixInfo>();

                supplierCodePrefixes = supplierCodeManager.GetSpecificCodeByPrefixes(prefixLength, _pendingCodePrefixes, effectiveOn, isFuture);
                AddCodePrefixes(supplierCodePrefixes, pendingCodePrefixes);

                saleCodePrefixes = saleCodeManager.GetSpecificCodeByPrefixes(prefixLength, _pendingCodePrefixes, effectiveOn, isFuture);
                AddCodePrefixes(saleCodePrefixes, pendingCodePrefixes);

                CheckThreshold(pendingCodePrefixes, codePrefixes, threshold);
            }

            if (pendingCodePrefixes.Count > 0)
                foreach (KeyValuePair<string, CodePrefixInfo> item in pendingCodePrefixes)
                    codePrefixes.Add(item.Key, item.Value);

            DisplayDictionary(codePrefixes);

            return codePrefixes.Values.OrderByDescending(x => x.Count);
        }
        #endregion

        #region Private Methods
        void AddCodePrefixes(IEnumerable<CodePrefixInfo> codePrefixes, Dictionary<string, CodePrefixInfo> pendingCodePrefixes)
        {
            long _validNumberPrefix;
            CodePrefixInfo _codePrefixInfo;

            if (codePrefixes != null)
            {
                foreach (CodePrefixInfo item in codePrefixes)
                    if (long.TryParse(item.CodePrefix, out _validNumberPrefix))
                    {
                        if (pendingCodePrefixes.TryGetValue(item.CodePrefix, out _codePrefixInfo))
                        {
                            _codePrefixInfo.Count += item.Count;
                        }
                        else
                        {
                            pendingCodePrefixes.Add(item.CodePrefix, item);
                        }
                    }
                //else
                //    context.WriteTrackingMessage(LogEntryType.Warning, "Invalid Sale Code Prefix: {0}", item.CodePrefix);
            }
        }
        void CheckThreshold(Dictionary<string, CodePrefixInfo> pendingCodePrefixes, Dictionary<string, CodePrefixInfo> codePrefixes, int threshold)
        {
            Dictionary<string, CodePrefixInfo> _pendingCodePrefixes = new Dictionary<string, CodePrefixInfo>(pendingCodePrefixes);
            foreach (KeyValuePair<string, CodePrefixInfo> item in _pendingCodePrefixes)
                if (item.Value.Count <= threshold)
                {
                    codePrefixes.Add(item.Key, item.Value);
                    pendingCodePrefixes.Remove(item.Key);
                }
        }
        void DisplayDictionary(Dictionary<string, CodePrefixInfo> codePrefixes)
        {
            IEnumerable<CodePrefixInfo> _list = codePrefixes.Values.OrderBy(x => x.CodePrefix);

            foreach (CodePrefixInfo item in _list)
                Console.WriteLine(item.CodePrefix + "   " + item.Count);

            Console.WriteLine("\n");
        }
        #endregion
    }

    public class VRMailMessageTemplateTask
    {
        #region Public Method
        public void VRMailMessageTemplate_Main()
        {
            Console.WriteLine("Ali Atoui: VRMailMessageTemplate");

            Guid guid = new Guid("E21CD125-61F0-4091-A03E-200CFE33F6E3");
            Carrier carrier = new Carrier() { Id = 100, CustomerId = 101 };
            User user = new User() { Email = "aatoui@vanrise.com", Name = "Ali Atoui" };

            Dictionary<string, dynamic> objects = new Dictionary<string, dynamic>();
            objects.Add("Carrier-ON", carrier);
            objects.Add("AliAtoui-ON", user);

            VRMailManager vrMailManager = new VRMailManager();
            vrMailManager.SendMail(guid, objects);

            Console.ReadLine();
        }
        #endregion

        #region Private Classes
        private class Carrier
        {
            public int Id { get; set; }

            public int CustomerId { get; set; }
        }
        private class User
        {
            public string Email { get; set; }
            public string Name { get; set; }
        }
        #endregion
    }

    public class Deserialize
    {
        RPRouteDataManager rpRouteDataManager = new RPRouteDataManager();

        #region Public Methods

        public string DeserializeOptionsDetailsBySupplier(string serializedOptionsDetailsBySupplier)
        {
            Dictionary<int, RPRouteOptionSupplier> optionsDetailsBySupplier = rpRouteDataManager.DeserializeOptionsDetailsBySupplier(serializedOptionsDetailsBySupplier);
            return Vanrise.Common.Serializer.Serialize(optionsDetailsBySupplier, true);
        }

        public  string DeserializeOptionsByPolicy(string serializedOptionsByPolicy)
        {
            Dictionary<Guid, IEnumerable<RPRouteOption>> optionsByPolicy  = rpRouteDataManager.DeserializeOptionsByPolicy(serializedOptionsByPolicy);
            return Vanrise.Common.Serializer.Serialize(optionsByPolicy, true);
        }

        #endregion
    }
}
