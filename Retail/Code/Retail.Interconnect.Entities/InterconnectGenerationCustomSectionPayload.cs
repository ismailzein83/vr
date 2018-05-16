using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Retail.BusinessEntity.Entities;

namespace Retail.Interconnect.Entities
{
    public class InterconnectGenerationCustomSectionPayload
    {
        public int? TimeZoneId { get; set; }
        public decimal? Commission { get; set; }
        public CommissionType CommissionType { get; set; }
    }
}
