using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.BusinessEntity.Business;
using Vanrise.Entities;
using Vanrise.BusinessProcess;

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

            SaleCodeManager saleCodeManager = new SaleCodeManager();
            IEnumerable<string> saleCodePrefixes = saleCodeManager.GetDistinctCodeByPrefixes(prefixLenght, effectiveOn, isFuture);

            long validNumberPrefix;

            if (saleCodePrefixes != null)
            {
                foreach (string item in saleCodePrefixes)
                    if (long.TryParse(item, out validNumberPrefix))
                        prefixesHashSet.Add(item);
                    else
                        context.WriteTrackingMessage(LogEntryType.Warning, "Invalid Sale Code Prefix: {0}", item);
            }

            SupplierCodeManager supplierCodeManager = new SupplierCodeManager();
            IEnumerable<string> supplierCodePrefixes = supplierCodeManager.GetDistinctCodeByPrefixes(prefixLenght, effectiveOn, isFuture);

            if (supplierCodePrefixes != null)
            {
                foreach (string item in supplierCodePrefixes)
                    if (long.TryParse(item, out validNumberPrefix))
                        prefixesHashSet.Add(item);
                    else
                        context.WriteTrackingMessage(LogEntryType.Warning, "Invalid Supplier Code Prefix: {0}", item);
            }

            this.DistinctCodePrefixes.Set(context, prefixesHashSet);
            context.GetSharedInstanceData().WriteTrackingMessage(LogEntryType.Information, "Preparing Code Prefixes is done", null);
        }
    }
}
