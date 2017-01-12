using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Data;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.Business
{
    public class ApplicableSaleZoneFilter : ISaleZoneFilter
    {
        public BulkActionType ActionType { get; set; }

        public SalePriceListOwnerType OwnerType { get; set; }

        public int OwnerId { get; set; }

        public bool IsExcluded(ISaleZoneFilterContext context)
        {
            if (context.SaleZone == null)
                throw new ArgumentNullException("SaleZone");

            if(context.CustomData != null)
            {
                var ratePlanDataManager = SalesDataManagerFactory.GetDataManager<IRatePlanDataManager>();
                context.CustomData = (object)ratePlanDataManager.GetChanges(this.OwnerType, this.OwnerId, RatePlanStatus.Draft);
            }

            return UtilitiesManager.IsActionApplicableToZone(this.ActionType, context.SaleZone.SaleZoneId, context.CustomData as Changes);
        }
    }
}
