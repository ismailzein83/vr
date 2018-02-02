using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Sales.BP.Activities
{
    public class GetOwnerName : CodeActivity
    {
        [RequiredArgument]
        public InArgument<int> OwnerId { get; set; }

        [RequiredArgument]
        public InArgument<SalePriceListOwnerType> OwnerType { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<int>> SubscriberOwnerIds { get; set; }

        [RequiredArgument]
        public InArgument<bool> IsSubscriber { get; set; }

        [RequiredArgument]
        public OutArgument<string> OwnerName { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            int ownerId = OwnerId.Get(context);
            SalePriceListOwnerType ownerType = OwnerType.Get(context);
            bool isSubscriber = IsSubscriber.Get(context);
            IEnumerable<int> subscriberOwnerIds = SubscriberOwnerIds.Get(context);
            string ownerName = "";

            if (ownerType == SalePriceListOwnerType.Customer)
            {
                var carrierAccountManager = new CarrierAccountManager();
                var carrierAccountName = carrierAccountManager.GetCarrierAccountName(ownerId);
                if (isSubscriber || subscriberOwnerIds == null || subscriberOwnerIds.Count() == 0)
                    ownerName = string.Format("customer '{0}'", carrierAccountName);
                else
                    ownerName = string.Format("publisher customer '{0}'", carrierAccountName);
            }
            else if (ownerType == SalePriceListOwnerType.SellingProduct)
            {
                var sellingProductManager = new SellingProductManager();
                ownerName = string.Format("selling product '{0}'", sellingProductManager.GetSellingProductName(ownerId));
            }
            OwnerName.Set(context, ownerName);
        }
    }
}
