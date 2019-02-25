using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Invoice.Entities
{
    public class SupplierGenerationCustomSectionPayload: BaseGenerationCustomSectionPayload
    {
      
    }
    public class ResolvedInvoicePayloadObject
    {
        public string Offset { get; set; }
        public TimeSpan? OffsetValue { get; set; }
        public int? TimeZoneId { get; set; }
        public decimal? Commission { get; set; }
        public CommissionType? CommissionType { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public DateTime ToDateForBillingTransaction { get; set; }

    }
}
