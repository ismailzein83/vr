using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.BusinessEntity.Business;

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

            if(saleCodePrefixes != null)
            {
                foreach (string item in saleCodePrefixes)
                    prefixesHashSet.Add(item);
            }

            SupplierCodeManager supplierCodeManager = new SupplierCodeManager();
            IEnumerable<string> supplierCodePrefixes = supplierCodeManager.GetDistinctCodeByPrefixes(prefixLenght, effectiveOn, isFuture);

            if (supplierCodePrefixes != null)
            {
                foreach (string item in supplierCodePrefixes)
                    prefixesHashSet.Add(item);
            }

            this.DistinctCodePrefixes.Set(context, prefixesHashSet);
        }
    }
}
