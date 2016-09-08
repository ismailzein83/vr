using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Routing.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Entities;
using Vanrise.Queueing;
using Vanrise.Runtime;

namespace TestRuntime.Tasks
{
    public class AnthonyTask : ITask
    {
        public void Execute()
        {
            //PrepareCodePrefixes();
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

            TransactionLockRuntimeService transactionLockRuntimeService = new TransactionLockRuntimeService() { Interval = new TimeSpan(0, 0, 1) };
            runtimeServices.Add(transactionLockRuntimeService);

            Vanrise.Integration.Business.DataSourceRuntimeService dsRuntimeService = new Vanrise.Integration.Business.DataSourceRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(dsRuntimeService);

            BPRegulatorRuntimeService bpRegulatorRuntimeService = new BPRegulatorRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(bpRegulatorRuntimeService);

            RuntimeHost host = new RuntimeHost(runtimeServices);
            host.Start();
            Console.ReadKey();
        }


        private void PrepareCodePrefixes()
        {
            List<List<CodePrefix>> distinctCodePrefixes = new List<List<CodePrefix>>();
            HashSet<string> codesDivided = new HashSet<string>();

            //Dictionaries
            Dictionary<CodePrefixKey, List<CodePrefixInfo>> codePrefixes = new Dictionary<CodePrefixKey, List<CodePrefixInfo>>();
            Dictionary<string, CodePrefixInfo> pendingCodePrefixes = new Dictionary<string, CodePrefixInfo>();

            //Initializint Settings
            TOne.WhS.Routing.Business.ConfigManager configManager = new TOne.WhS.Routing.Business.ConfigManager();
            SubProcessSettings settings = configManager.GetSubProcessSettings();
            int threshold = settings.CodeRangeCountThreshold;
            int maxPrefixLength = settings.MaxCodePrefixLength;

            int prefixLength = 1;
            DateTime? effectiveOn = DateTime.Now;
            bool isFuture = false;

            SupplierCodeManager supplierCodeManager = new SupplierCodeManager();
            IEnumerable<CodePrefixInfo> supplierCodePrefixes = supplierCodeManager.GetDistinctCodeByPrefixes(prefixLength, effectiveOn, isFuture);
            AddCodePrefixes(supplierCodePrefixes, pendingCodePrefixes);

            SaleCodeManager saleCodeManager = new SaleCodeManager();
            IEnumerable<CodePrefixInfo> saleCodePrefixes = saleCodeManager.GetDistinctCodeByPrefixes(prefixLength, effectiveOn, isFuture);
            AddCodePrefixes(saleCodePrefixes, pendingCodePrefixes);

            CheckThreshold(pendingCodePrefixes, codePrefixes, threshold);

            while (pendingCodePrefixes.Count > 0 && prefixLength < maxPrefixLength)
            {
                prefixLength++;

                IEnumerable<string> _pendingCodePrefixes = pendingCodePrefixes.Keys;
                codesDivided.UnionWith(_pendingCodePrefixes);

                pendingCodePrefixes = new Dictionary<string, CodePrefixInfo>();

                supplierCodePrefixes = supplierCodeManager.GetSpecificCodeByPrefixes(prefixLength, _pendingCodePrefixes, effectiveOn, isFuture);
                AddCodePrefixes(supplierCodePrefixes, pendingCodePrefixes);

                saleCodePrefixes = saleCodeManager.GetSpecificCodeByPrefixes(prefixLength, _pendingCodePrefixes, effectiveOn, isFuture);
                AddCodePrefixes(saleCodePrefixes, pendingCodePrefixes);

                CheckThreshold(pendingCodePrefixes, codePrefixes, threshold);
            }

            if (pendingCodePrefixes.Count > 0)
                foreach (KeyValuePair<string, CodePrefixInfo> item in pendingCodePrefixes)
                    codePrefixes.Add(new CodePrefixKey() { Count = item.Value.Count }, new List<CodePrefixInfo>() { item.Value });


            codePrefixes.OrderByDescending(itm => itm.Key.Count).ToDictionary(itm => itm.Key, itm => itm.Value).Values.ToList().ForEach(i =>
            {
                List<CodePrefix> currentCodePrefixes = new List<CodePrefix>();
                foreach (CodePrefixInfo codePrefixInfo in i)
                {
                    CodePrefix codePrefix = new CodePrefix()
                    {
                        Code = codePrefixInfo.CodePrefix,
                        IsCodeDivided = codesDivided.Contains(codePrefixInfo.CodePrefix),
                        CodeCount = codePrefixInfo.Count
                    };
                    currentCodePrefixes.Add(codePrefix);
                }
                distinctCodePrefixes.Add(currentCodePrefixes);
            });
        }

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
                    else
                        Console.WriteLine(string.Format("Invalid Sale Code Prefix: {0}", item.CodePrefix));
            }
        }

        void CheckThreshold(Dictionary<string, CodePrefixInfo> pendingCodePrefixes, Dictionary<CodePrefixKey, List<CodePrefixInfo>> codePrefixes, int threshold)
        {
            Dictionary<string, CodePrefixInfo> _pendingCodePrefixes = new Dictionary<string, CodePrefixInfo>(pendingCodePrefixes).OrderByDescending(itm => itm.Value.Count).ToDictionary(itm => itm.Key, itm => itm.Value);
            List<CodePrefixInfo> codePrefixInfoList = new List<CodePrefixInfo>();
            int count = 0;

            foreach (KeyValuePair<string, CodePrefixInfo> item in _pendingCodePrefixes)
            {
                if (item.Value.Count <= threshold)
                {
                    if (item.Value.Count + count > threshold)
                    {
                        codePrefixes.Add(new CodePrefixKey() { Count = count }, codePrefixInfoList);
                        codePrefixInfoList = new List<CodePrefixInfo>();
                        count = 0;
                    }

                    codePrefixInfoList.Add(item.Value);
                    count += item.Value.Count;

                    pendingCodePrefixes.Remove(item.Key);
                }
            }
            codePrefixes.Add(new CodePrefixKey() { Count = count }, codePrefixInfoList);//To Add the last item
        }

        private class CodePrefixKey
        {
            public int Count { get; set; }
        }
    }
}