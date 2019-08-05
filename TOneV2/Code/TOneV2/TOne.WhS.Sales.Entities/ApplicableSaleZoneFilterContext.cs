using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Sales.Entities
{
    public class ApplicableSaleZoneFilterContext : ISaleZoneFilterContext
    {
        public int SellingNumberPlanId { get; set; }

        public SaleZone SaleZone { get; set; }

        public object CustomData { get; set; }

        public List<CostCalculationMethod> CostCalculationMethods { get; set; }

        public int? RoutingDatabaseId { get; set; }

        public Guid? PolicyConfigId { get; set; }

        public int? NumberOfOptions { get; set; }

        public int? CurrencyId { get; set; }
    }
}
