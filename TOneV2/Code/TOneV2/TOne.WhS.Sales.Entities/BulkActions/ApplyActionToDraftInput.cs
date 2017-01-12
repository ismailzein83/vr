using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Sales.Entities
{
    public class ApplyActionToDraftInput
    {
        public BulkActionZoneFilter BulkActionFilter { get; set; }

        public SalePriceListOwnerType OwnerType { get; set; }

        public int OwnerId { get; set; }

        public int CurrencyId { get; set; }

        public int RoutingDatabaseId { get; set; }

        public Guid PolicyConfigId { get; set; }

        public int NumberOfOptions { get; set; }

        public List<CostCalculationMethod> CostCalculationMethods { get; set; }

        public BulkActionType BulkAction { get; set; }
    }
}
