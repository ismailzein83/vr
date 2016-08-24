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
    public sealed class PrepareCodePrefixes : CodeActivity
    {
        [RequiredArgument]
        public InArgument<DateTime?> EffectiveOn { get; set; }

        [RequiredArgument]
        public InArgument<bool> IsFuture { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<CodePrefix>> DistinctCodePrefixes { get; set; }


        protected override void Execute(CodeActivityContext context)
        {
            List<CodePrefix> distinctCodePrefixes = new List<CodePrefix>();
            HashSet<string> codesDivided = new HashSet<string>();

            //Dictionaries
            Dictionary<string, CodePrefixInfo> codePrefixes = new Dictionary<string, CodePrefixInfo>();
            Dictionary<string, CodePrefixInfo> pendingCodePrefixes = new Dictionary<string, CodePrefixInfo>();

            //Initializint Settings
            TOne.WhS.Routing.Business.ConfigManager configManager = new TOne.WhS.Routing.Business.ConfigManager();
            SubProcessSettings settings = configManager.GetSubProcessSettings();
            int threshold = settings.CodeRangeCountThreshold;
            int maxPrefixLength = settings.MaxCodePrefixLength;

            int prefixLength = 1;
            DateTime? effectiveOn = this.EffectiveOn.Get(context);
            bool isFuture = this.IsFuture.Get(context);

            SupplierCodeManager supplierCodeManager = new SupplierCodeManager();
            IEnumerable<CodePrefixInfo> supplierCodePrefixes = supplierCodeManager.GetDistinctCodeByPrefixes(prefixLength, effectiveOn, isFuture);
            AddCodePrefixes(supplierCodePrefixes, pendingCodePrefixes, context);

            SaleCodeManager saleCodeManager = new SaleCodeManager();
            IEnumerable<CodePrefixInfo> saleCodePrefixes = saleCodeManager.GetDistinctCodeByPrefixes(prefixLength, effectiveOn, isFuture);
            AddCodePrefixes(saleCodePrefixes, pendingCodePrefixes, context);

            if (maxPrefixLength == 1)
            {
                codePrefixes = pendingCodePrefixes;
            }
            else
            {
                CheckThreshold(pendingCodePrefixes, codePrefixes, threshold);

                while (pendingCodePrefixes.Count > 0 && prefixLength < maxPrefixLength)
                {
                    prefixLength++;

                    IEnumerable<string> _pendingCodePrefixes = pendingCodePrefixes.Keys;
                    codesDivided.UnionWith(_pendingCodePrefixes);

                    pendingCodePrefixes = new Dictionary<string, CodePrefixInfo>();

                    supplierCodePrefixes = supplierCodeManager.GetSpecificCodeByPrefixes(prefixLength, _pendingCodePrefixes, effectiveOn, isFuture);
                    AddCodePrefixes(supplierCodePrefixes, pendingCodePrefixes, context);

                    saleCodePrefixes = saleCodeManager.GetSpecificCodeByPrefixes(prefixLength, _pendingCodePrefixes, effectiveOn, isFuture);
                    AddCodePrefixes(saleCodePrefixes, pendingCodePrefixes, context);

                    CheckThreshold(pendingCodePrefixes, codePrefixes, threshold);
                }

                if (pendingCodePrefixes.Count > 0)
                    foreach (KeyValuePair<string, CodePrefixInfo> item in pendingCodePrefixes)
                        codePrefixes.Add(item.Key, item.Value);
            }

            codePrefixes.Values.OrderByDescending(itm => itm.Count).ToList().ForEach(i =>
                distinctCodePrefixes.Add(new CodePrefix()
                {
                    Code = i.CodePrefix,
                    IsCodeDivided = codesDivided.Contains(i.CodePrefix),
                    CodeCount = i.Count
                }));

            this.DistinctCodePrefixes.Set(context, distinctCodePrefixes);
            context.GetSharedInstanceData().WriteTrackingMessage(LogEntryType.Information, "Preparing Code Prefixes is done", null);
        }

        #region Private Methods

        void AddCodePrefixes(IEnumerable<CodePrefixInfo> codePrefixes, Dictionary<string, CodePrefixInfo> pendingCodePrefixes, CodeActivityContext context)
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
                        context.WriteTrackingMessage(LogEntryType.Warning, "Invalid Sale Code Prefix: {0}", item.CodePrefix);
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
        #endregion
    }
}
