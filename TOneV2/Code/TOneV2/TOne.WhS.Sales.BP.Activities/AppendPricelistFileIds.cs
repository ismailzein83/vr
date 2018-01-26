using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.BP.Activities
{
    public class AppendPricelistFileIds : CodeActivity
    {
        [RequiredArgument]
        public InArgument<List<long>> AllPricelistFileIds { get; set; }

        [RequiredArgument]
        public InArgument<List<long>> OwnerPricelistFileIds { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            List<long> allPricelistFileIds = AllPricelistFileIds.Get(context);
            List<long> ownerPricelistFileIds = OwnerPricelistFileIds.Get(context);
            if (ownerPricelistFileIds != null && ownerPricelistFileIds.Any())
                allPricelistFileIds.AddRange(ownerPricelistFileIds);
        }
    }
}
