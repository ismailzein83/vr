﻿using System;
using System.Collections.Generic;
using System.Linq;
using TestRuntime;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Deal.Business;
using TOne.WhS.Deal.Entities;
using TOne.WhS.Routing.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Common.Business;
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

            //byte[] lastTimestamp = AliAtouiTask.StringToByteArray("00000000000055F5");
            //DealDetailedProgressManager dealDetailedProgressManager = new DealDetailedProgressManager();
            //DealEvaluatorProcessState first_dealEvaluatorProcessState = dealDetailedProgressManager.GetDealEvaluatorProcessState(null);
            //DealEvaluatorProcessState second_dealEvaluatorProcessState = dealDetailedProgressManager.GetDealEvaluatorProcessState(lastTimestamp);
            //DealEvaluatorProcessState third_dealEvaluatorProcessState = dealDetailedProgressManager.GetDealEvaluatorProcessState(second_dealEvaluatorProcessState.MaxTimestamp);
        } 
        #endregion

        #region Private Methods
        void DisplayList(IEnumerable<CodePrefixInfo> codePrefixes)
        {
            foreach (CodePrefixInfo item in codePrefixes)
                Console.WriteLine(item.CodePrefix + "   " + item.Count);

            Console.WriteLine("\n");
        }

        static byte[] StringToByteArray(string hex)
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

            BusinessProcessService bpService = new BusinessProcessService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(bpService);

            QueueRegulatorRuntimeService queueRegulatorService = new QueueRegulatorRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(queueRegulatorService);

            QueueActivationRuntimeService queueActivationService = new QueueActivationRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(queueActivationService);

            SummaryQueueActivationRuntimeService summaryQueueActivationService = new SummaryQueueActivationRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(summaryQueueActivationService);

            SchedulerService schedulerService = new SchedulerService() { Interval = new TimeSpan(0, 0, 1) };
            runtimeServices.Add(schedulerService);

            Vanrise.Common.Business.BigDataRuntimeService bigDataService = new Vanrise.Common.Business.BigDataRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(bigDataService);

            Vanrise.Integration.Business.DataSourceRuntimeService dsRuntimeService = new Vanrise.Integration.Business.DataSourceRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(dsRuntimeService);

            BPRegulatorRuntimeService bpRegulatorRuntimeService = new BPRegulatorRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(bpRegulatorRuntimeService);

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
}
