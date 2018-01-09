using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;

namespace TOne.WhS.Sales.BP.Activities
{
    public class ReserveOwnerPriceListId : CodeActivity
    {
        #region Input Arguments
        [RequiredArgument]
        public InArgument<int> RatesToChangeCount { get; set; }

        [RequiredArgument]
        public InArgument<int> RatesToCloseCount { get; set; }
        #endregion

        #region Output Arguments
        [RequiredArgument]
        public OutArgument<int?> ReservedOwnerPriceListId { get; set; }
        #endregion

        protected override void Execute(CodeActivityContext context)
        {
            //int ratesToChangeCount = RatesToChangeCount.Get(context);
            //int ratesToCloseCount = RatesToCloseCount.Get(context);

            //int? ownerPriceListId = null;

            //if (ratesToChangeCount > 0 || ratesToCloseCount > 0)
            //    ownerPriceListId = (int)new SalePriceListManager().ReserveIdRange(1);

            //ReservedOwnerPriceListId.Set(context, ownerPriceListId);
        }
    }
}
