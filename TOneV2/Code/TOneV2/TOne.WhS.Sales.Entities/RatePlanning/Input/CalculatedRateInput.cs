using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Sales.Entities
{
    public class CalculatedRateInput
    {
        public SalePriceListOwnerType OwnerType { get; set; }

        public int OwnerId { get; set; }

        public int? SellingNumberPlanId { get; set; }

        public int? SellingProductId { get; set; }

        public DateTime EffectiveOn { get; set; }

        public int RoutingDatabaseId { get; set; }

        public int PolicyConfigId { get; set; }

        public int NumberOfOptions { get; set; }

        public List<CostCalculationMethod> CostCalculationMethods { get; set; }

        public int? RateCalculationCostColumnConfigId { get; set; }

        public RateCalculationMethod RateCalculationMethod { get; set; }
    }
}
