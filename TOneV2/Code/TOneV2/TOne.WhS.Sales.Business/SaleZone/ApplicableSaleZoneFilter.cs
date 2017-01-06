using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.Business
{
    public class ApplicableSaleZoneFilter : ISaleZoneFilter
    {
        public BulkActionType ActionType { get; set; }

        public bool IsExcluded(ISaleZoneFilterContext context)
        {
            if (context.SaleZone == null)
                throw new ArgumentNullException("SaleZone");

            return this.ActionType.IsZoneApplicable(context.SaleZone.SaleZoneId);
        }
    }
}
