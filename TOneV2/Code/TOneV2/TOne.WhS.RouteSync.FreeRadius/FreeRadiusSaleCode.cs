using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.Postgres;

namespace TOne.WhS.RouteSync.FreeRadius
{
    public class FreeRadiusSaleCode : INpgBulkCopy
    {
        public long SaleCodeId { get; set; }

        public string Code { get; set; }
         
        public long ZoneId { get; set; }

        public string ConvertToString()
        {
            return string.Format("{1}{0}{2}{0}{3}", "\t", SaleCodeId, Code, ZoneId);
        }
    }
}
