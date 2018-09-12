using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class SaleEntityZoneRoutingProductHistoryRecord : Vanrise.Entities.IDateEffectiveSettingsEditable, Vanrise.Entities.IDateEffectiveSettings
	{
        public int RoutingProductId { get; set; }

        public long? SaleZoneId { get; set; }

        public SaleEntityZoneRoutingProductSource Source { get; set; }

        public int SaleEntityId { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }
    }

    public class SaleEntityZoneRoutingProductHistoryRecordDetail
    {
        public SaleEntityZoneRoutingProductHistoryRecord Entity { get; set; }

        public string RoutingProductName { get; set; }

        public IEnumerable<int> ServiceIds { get; set; }

        public string SaleEntityName { get; set; }
    }
}
