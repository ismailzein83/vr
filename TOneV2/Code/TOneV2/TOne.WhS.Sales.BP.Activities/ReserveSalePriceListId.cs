using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;

namespace TOne.WhS.Sales.BP.Activities
{
    public class ReserveSalePriceListId : CodeActivity
    {
        #region Input Arguments

        [RequiredArgument]
        public InArgument<int> NewRatesCount { get; set; }

        [RequiredArgument]
        public InArgument<int> ChangedRatesCount { get; set; }

        [RequiredArgument]
        public InArgument<bool> DoesNewDefaultServiceExist { get; set; }

        [RequiredArgument]
        public InArgument<int> ChangedDefaultServicesCount { get; set; }

        [RequiredArgument]
        public InArgument<int> NewSaleZoneServicesCount { get; set; }

        [RequiredArgument]
        public InArgument<int> ChangedSaleZoneServicesCount { get; set; }

        #endregion

        #region Output Arguments

        [RequiredArgument]
        public OutArgument<int?> SalePriceListId { get; set; }
        
        #endregion

        protected override void Execute(CodeActivityContext context)
        {
            int newRatesCount = NewRatesCount.Get(context);
            int changedRatesCount = ChangedRatesCount.Get(context);

            bool doesNewDefaultServiceExist = DoesNewDefaultServiceExist.Get(context);
            int changedDefaultServicesCount = ChangedDefaultServicesCount.Get(context);
            int newSaleZoneServicesCount = NewSaleZoneServicesCount.Get(context);
            int changedSaleZoneServicesCount = ChangedSaleZoneServicesCount.Get(context);

            int? salePriceListId = null;

            if (newRatesCount > 0 || changedRatesCount > 0 || doesNewDefaultServiceExist || changedDefaultServicesCount > 0 || newSaleZoneServicesCount > 0 || changedSaleZoneServicesCount > 0)
            {
                var salePriceListManager = new SalePriceListManager();
                long startingId = salePriceListManager.ReserveIdRange(1);
                salePriceListId = Convert.ToInt32(startingId);
            }

            SalePriceListId.Set(context, salePriceListId);
        }
    }
}
