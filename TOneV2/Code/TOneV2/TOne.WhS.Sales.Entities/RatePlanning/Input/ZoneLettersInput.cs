using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Sales.Entities
{
    public class ZoneLettersInput
    {
        public SalePriceListOwnerType OwnerType { get; set; }

        public int OwnerId { get; set; }

        public IEnumerable<int> CountryIds { get; set; }

        public Vanrise.Entities.TextFilterType? ZoneNameFilterType { get; set; }

        public string ZoneNameFilter { get; set; }

        public DateTime EffectiveOn { get; set; }

		public BulkActionType BulkAction { get; set; }

		public BulkActionZoneFilter BulkActionFilter { get; set; }
    }
}
