using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Sales.Entities
{
    public class BulkActionZoneValidationInput
    {
        public SalePriceListOwnerType OwnerType { get; set; }

        public int OwnerId { get; set; }

        public DateTime EffectiveOn { get; set; }

        public BulkActionType BulkAction { get; set; }

        public BulkActionZoneFilter BulkActionZoneFilter { get; set; }

        #region Routing Properties

        public int RoutingDatabaseId { get; set; }

        public Guid PolicyConfigId { get; set; }

        public int NumberOfOptions { get; set; }

        public List<CostCalculationMethod> CostCalculationMethods { get; set; }

        public int CurrencyId { get; set; }

        #endregion
    }
}
