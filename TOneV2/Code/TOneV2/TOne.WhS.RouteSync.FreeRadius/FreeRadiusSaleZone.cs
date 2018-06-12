using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.Postgres;

namespace TOne.WhS.RouteSync.FreeRadius
{
    public class FreeRadiusSaleZone : INpgBulkCopy
    {
        public long SaleZoneId { get; set; }

        public string Name { get; set; }

        public int SellingNumberPlanId { get; set; }

        public string ConvertToString()
        {
            return string.Format("{1}{0}{2}{0}{3}", "\t", SaleZoneId, Name, SellingNumberPlanId);
        }
    }
}
