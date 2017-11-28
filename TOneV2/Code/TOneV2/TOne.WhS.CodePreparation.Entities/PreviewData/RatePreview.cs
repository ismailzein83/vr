using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.CodePreparation.Entities;

namespace TOne.WhS.CodePreparation.Entities
{
    public class RatePreview
    {
        public string ZoneName { get; set; }

        public SalePriceListOwnerType OnwerType { get; set; }

        public RateChangeType ChangeType { get; set; }

        public int OwnerId { get; set; }

        public decimal Rate { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }
        public int? CurrencyId { get; set; }
    }

    public class RatePreviewDetail
    {
        public RatePreview Entity { get; set; }

        public string OwnerTypeDescription { get; set; }

        public string OwnerName { get; set; }
        public string CurrencySymbol { get; set; }
    }

    public class ZoneRoutingProductPreview
    {
        public string ZoneName { get; set; }

        public SalePriceListOwnerType OwnerType { get; set; }
        
        public int OwnerId { get; set; }

        public int RoutingProductId { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }

        public ZoneRoutingProductChangeType ChangeType { get; set; }
    }

    public class ZoneRoutingProductPreviewDetail
    {
        public ZoneRoutingProductPreview Entity { get; set; }

        public string RoutingProductName { get; set; }

        public string OwnerTypeDescription { get; set; }

        public string OwnerName { get; set; }
        public string ChangeTypeDescription { get; set; }
        public IEnumerable<int> RoutingProductServicesIds { get; set; }

    }

    public enum ZoneRoutingProductChangeType
    {
        [Description("Same")]
        NotChanged = 0,

        [Description("New")]
        New = 1,

        [Description("Deleted")]
        Deleted = 2,

    }
}
