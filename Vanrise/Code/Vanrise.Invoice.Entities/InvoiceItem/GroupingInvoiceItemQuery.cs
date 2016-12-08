using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Invoice.Entities
{
    public class GroupingInvoiceItemQuery
    {
        public long InvoiceId { get; set; }
        public Guid InvoiceTypeId { get; set; }
        public Guid ItemGroupingId { get; set; }
        public List<Guid> DimensionIds { get; set; }
        public List<Guid> MeasureIds { get; set; }
        public List<InvoiceGroupingDimensionFilter> Filters { get; set; }
    }
    public class InvoiceGroupingDimensionFilter
    {
        public Guid DimensionId { get; set; }

        public List<Object> FilterValues { get; set; }
    }
}
