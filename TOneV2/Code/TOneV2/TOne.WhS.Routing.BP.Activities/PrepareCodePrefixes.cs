using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.BusinessEntity.Business;
using Vanrise.Entities;
using Vanrise.BusinessProcess;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Routing.BP.Activities
{

    public sealed class PrepareCodePrefixes : CodeActivity
    {
        [RequiredArgument]
        public InArgument<int> CodePrefixLength { get; set; }

        [RequiredArgument]
        public InArgument<DateTime?> EffectiveOn { get; set; }

        [RequiredArgument]
        public InArgument<bool> IsFuture { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<string>> DistinctCodePrefixes { get; set; }


        protected override void Execute(CodeActivityContext context)
        {

            int prefixLenght = this.CodePrefixLength.Get(context);
            DateTime? effectiveOn = this.EffectiveOn.Get(context);
            bool isFuture = this.IsFuture.Get(context);

            HashSet<string> prefixesHashSet = new HashSet<string>();
            List<CodePrefixInfo> codePrefixes = new List<CodePrefixInfo>();
            long validNumberPrefix;

            SupplierCodeManager supplierCodeManager = new SupplierCodeManager();
            IEnumerable<CodePrefixInfo> supplierCodePrefixes = supplierCodeManager.GetDistinctCodeByPrefixes(prefixLenght, effectiveOn, isFuture);

            if (supplierCodePrefixes != null)
            {
                foreach (CodePrefixInfo item in supplierCodePrefixes)
                    if (long.TryParse(item.CodePrefix, out validNumberPrefix))
                    {
                        if (!prefixesHashSet.Contains(item.CodePrefix))
                        {
                            codePrefixes.Add(item);
                            prefixesHashSet.Add(item.CodePrefix);
                        }
                    }
                    else
                        context.WriteTrackingMessage(LogEntryType.Warning, "Invalid Supplier Code Prefix: {0}", item.CodePrefix);
            }


            SaleCodeManager saleCodeManager = new SaleCodeManager();
            IEnumerable<CodePrefixInfo> saleCodePrefixes = saleCodeManager.GetDistinctCodeByPrefixes(prefixLenght, effectiveOn, isFuture);

            if (saleCodePrefixes != null)
            {
                foreach (CodePrefixInfo item in saleCodePrefixes)
                    if (long.TryParse(item.CodePrefix, out validNumberPrefix))
                    {
                        if (!prefixesHashSet.Contains(item.CodePrefix))
                        {
                            codePrefixes.Add(item);
                            prefixesHashSet.Add(item.CodePrefix);
                        }
                    }
                    else
                        context.WriteTrackingMessage(LogEntryType.Warning, "Invalid Sale Code Prefix: {0}", item.CodePrefix);
            }

            IEnumerable<string> result = codePrefixes.OrderBy(itm => itm.Count).Select(itm => itm.CodePrefix);

            this.DistinctCodePrefixes.Set(context, result);
            context.GetSharedInstanceData().WriteTrackingMessage(LogEntryType.Information, "Preparing Code Prefixes is done", null);
        }
    }
}
