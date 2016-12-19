using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Invoice.Entities
{
    public class GroupingInvoiceItemDetail
    {
        public InvoiceGroupingDimensionValue[] DimensionValues { get; set; }

        public InvoiceGroupingMeasureValues MeasureValues { get; set; }
     
    }
    public class InvoiceGroupingDimensionValue
    {
        public Object Value { get; set; }
        public string Name { get; set; }
    }
    public class InvoiceGroupingMeasureValues : Dictionary<string, InvoiceGroupingMeasureValue>
    {
    }
    public class InvoiceGroupingMeasureValue
    {
        public object Value { get; set; }
    }
}
