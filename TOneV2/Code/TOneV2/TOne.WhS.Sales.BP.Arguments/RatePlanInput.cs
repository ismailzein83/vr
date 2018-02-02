using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Sales.BP.Arguments
{
    public class RatePlanInput : Vanrise.BusinessProcess.Entities.BaseProcessInputArgument
    {
        public SalePriceListOwnerType OwnerType { get; set; }

        public int OwnerId { get; set; }

        public int CurrencyId { get; set; }

        public DateTime EffectiveDate { get; set; }

        public IEnumerable<int> SubscriberOwnerIds { get; set; }

        public override string EntityId
        {
            get
            {
                return String.Format("{0}_{1}",(int) OwnerType, OwnerId);
            }
        }
        public override string GetTitle()
        {
            var ratePlanInputManager = new RatePlanInputManager();
            return ratePlanInputManager.GetTitle(this.OwnerType, this.OwnerId);
        }

        public bool FollowPublisherRatesBED { get; set; }
    }
}
