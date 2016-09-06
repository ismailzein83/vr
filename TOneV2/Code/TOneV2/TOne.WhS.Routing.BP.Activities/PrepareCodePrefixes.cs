using System;
using System.Collections.Generic;
using System.Linq;
using System.Activities;
using TOne.WhS.BusinessEntity.Business;
using Vanrise.Entities;
using Vanrise.BusinessProcess;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.BP.Activities
{
    public class PrepareCodePrefixesInput
    {
        public DateTime? EffectiveOn { get; set; }
        public bool IsEffectiveInFuture { get; set; }
    }

    public class PrepareCodePrefixesOutput
    {
        public IEnumerable<IEnumerable<CodePrefix>> DistinctCodePrefixes { get; set; }
    }

    public sealed class PrepareCodePrefixes : BaseAsyncActivity<PrepareCodePrefixesInput, PrepareCodePrefixesOutput>
    {
        [RequiredArgument]
        public InArgument<DateTime?> EffectiveOn { get; set; }

        [RequiredArgument]
        public InArgument<bool> IsFuture { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<IEnumerable<CodePrefix>>> DistinctCodePrefixes { get; set; }

        protected override PrepareCodePrefixesOutput DoWorkWithResult(PrepareCodePrefixesInput inputArgument, AsyncActivityHandle handle)
        {
            List<List<CodePrefix>> distinctCodePrefixes = new List<List<CodePrefix>>();
            HashSet<string> codesDivided = new HashSet<string>();

            //Dictionaries
            Dictionary<CodePrefixKey, CodePrefixInfo> codePrefixes = new Dictionary<CodePrefixKey, CodePrefixInfo>();
            Dictionary<string, CodePrefixInfo> pendingCodePrefixes = new Dictionary<string, CodePrefixInfo>();

            //Initializint Settings
            TOne.WhS.Routing.Business.ConfigManager configManager = new TOne.WhS.Routing.Business.ConfigManager();
            SubProcessSettings settings = configManager.GetSubProcessSettings();
            int threshold = settings.CodeRangeCountThreshold;
            int maxPrefixLength = settings.MaxCodePrefixLength;

            int prefixLength = 1;
            DateTime? effectiveOn = inputArgument.EffectiveOn;
            bool isFuture = inputArgument.IsEffectiveInFuture;

            SupplierCodeManager supplierCodeManager = new SupplierCodeManager();
            IEnumerable<CodePrefixInfo> supplierCodePrefixes = supplierCodeManager.GetDistinctCodeByPrefixes(prefixLength, effectiveOn, isFuture);
            AddCodePrefixes(supplierCodePrefixes, pendingCodePrefixes, handle);

            SaleCodeManager saleCodeManager = new SaleCodeManager();
            IEnumerable<CodePrefixInfo> saleCodePrefixes = saleCodeManager.GetDistinctCodeByPrefixes(prefixLength, effectiveOn, isFuture);
            AddCodePrefixes(saleCodePrefixes, pendingCodePrefixes, handle);

            CheckThreshold(pendingCodePrefixes, codePrefixes, threshold);

            while (pendingCodePrefixes.Count > 0 && prefixLength < maxPrefixLength)
            {
                prefixLength++;

                IEnumerable<string> _pendingCodePrefixes = pendingCodePrefixes.Keys;
                codesDivided.UnionWith(_pendingCodePrefixes);

                pendingCodePrefixes = new Dictionary<string, CodePrefixInfo>();

                supplierCodePrefixes = supplierCodeManager.GetSpecificCodeByPrefixes(prefixLength, _pendingCodePrefixes, effectiveOn, isFuture);
                AddCodePrefixes(supplierCodePrefixes, pendingCodePrefixes, handle);

                saleCodePrefixes = saleCodeManager.GetSpecificCodeByPrefixes(prefixLength, _pendingCodePrefixes, effectiveOn, isFuture);
                AddCodePrefixes(saleCodePrefixes, pendingCodePrefixes, handle);

                CheckThreshold(pendingCodePrefixes, codePrefixes, threshold);
            }

            if (pendingCodePrefixes.Count > 0)
                foreach (KeyValuePair<string, CodePrefixInfo> item in pendingCodePrefixes)
                    codePrefixes.Add(new CodePrefixKey() { Count = item.Value.Count }, item.Value);

            Dictionary<CodePrefixKey, List<CodePrefixInfo>> groupCodePrefix = GroupCodePrefixes(codePrefixes, threshold);
            groupCodePrefix.OrderByDescending(itm => itm.Key.Count).ToDictionary(itm => itm.Key, itm => itm.Value).Values.ToList().ForEach(i =>
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

            return new PrepareCodePrefixesOutput()
            {
                DistinctCodePrefixes = distinctCodePrefixes
            };
        }


        protected override PrepareCodePrefixesInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new PrepareCodePrefixesInput()
            {
                EffectiveOn = this.EffectiveOn.Get(context),
                IsEffectiveInFuture = this.IsFuture.Get(context),
            };
        }
        protected override void OnWorkComplete(AsyncCodeActivityContext context, PrepareCodePrefixesOutput result)
        {
            this.DistinctCodePrefixes.Set(context, result.DistinctCodePrefixes);
            context.GetSharedInstanceData().WriteTrackingMessage(LogEntryType.Information, "Preparing Code Prefixes is done", null);
        }

        #region Private Methods

        void AddCodePrefixes(IEnumerable<CodePrefixInfo> codePrefixes, Dictionary<string, CodePrefixInfo> pendingCodePrefixes, AsyncActivityHandle handle)
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
                        handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Warning, "Invalid Sale Code Prefix: {0}", item.CodePrefix);
            }
        }

        void CheckThreshold(Dictionary<string, CodePrefixInfo> pendingCodePrefixes, Dictionary<CodePrefixKey, CodePrefixInfo> codePrefixes, int threshold)
        {
            Dictionary<string, CodePrefixInfo> _pendingCodePrefixes = new Dictionary<string, CodePrefixInfo>(pendingCodePrefixes).OrderByDescending(itm => itm.Value.Count).ToDictionary(itm => itm.Key, itm => itm.Value);

            foreach (KeyValuePair<string, CodePrefixInfo> item in _pendingCodePrefixes)
            {
                if (item.Value.Count <= threshold)
                {
                    codePrefixes.Add(new CodePrefixKey() { Count = item.Value.Count }, item.Value);
                    pendingCodePrefixes.Remove(item.Key);
                }
            }
        }

        private Dictionary<CodePrefixKey, List<CodePrefixInfo>> GroupCodePrefixes(Dictionary<CodePrefixKey, CodePrefixInfo> codePrefixes, int threshold)
        {
            Dictionary<CodePrefixKey, List<CodePrefixInfo>> result = new Dictionary<CodePrefixKey, List<CodePrefixInfo>>();
            codePrefixes = codePrefixes.OrderByDescending(itm => itm.Key.Count).ToDictionary(itm => itm.Key, itm => itm.Value);

            List<CodePrefixInfo> codePrefixInfoList = new List<CodePrefixInfo>();
            int count = 0;

            foreach (KeyValuePair<CodePrefixKey, CodePrefixInfo> item in codePrefixes)
            {
                if (item.Value.Count + count > threshold && count > 0)
                {
                    result.Add(new CodePrefixKey() { Count = count }, codePrefixInfoList);
                    codePrefixInfoList = new List<CodePrefixInfo>();
                    count = 0;
                }

                codePrefixInfoList.Add(item.Value);
                count += item.Value.Count;


            }
            result.Add(new CodePrefixKey() { Count = count }, codePrefixInfoList);//to add last item
            return result;
        }

        private class CodePrefixKey
        {
            public int Count { get; set; }
        }
        #endregion
    }
}
