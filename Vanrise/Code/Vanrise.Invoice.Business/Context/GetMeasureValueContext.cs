using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.Business.Context
{
    public class GetMeasureValueContext : IGetMeasureValueContext
    {
        GroupingInvoiceItemQuery _query;
        InvoiceItemRecord _invoiceItemRecord;
        IGroupingInvoiceItemQueryContext _groupingInvoiceItemQueryContext;
        public GetMeasureValueContext(IGroupingInvoiceItemQueryContext groupingInvoiceItemQueryContext, InvoiceItemRecord invoiceItemRecord)
        {
            if (groupingInvoiceItemQueryContext == null)
                throw new ArgumentNullException("groupingInvoiceItemQueryContext");
            if (groupingInvoiceItemQueryContext.Query == null)
                throw new ArgumentNullException("groupingInvoiceItemQueryContext.Query");
            if (invoiceItemRecord == null)
                throw new ArgumentNullException("invoiceItemRecord");
            _groupingInvoiceItemQueryContext = groupingInvoiceItemQueryContext;
            _query = _groupingInvoiceItemQueryContext.Query;
            _invoiceItemRecord = invoiceItemRecord;
        }
        public dynamic GetAggregateValue(Guid aggregateId)
        {
            InvoiceItemAggValue aggValue;
            if (!_invoiceItemRecord.AggValuesByAggId.TryGetValue(aggregateId, out aggValue))
                throw new NullReferenceException(String.Format("aggValue. AggId '{0}'", aggregateId));
            return aggValue.Value;
        }
        public List<dynamic> GetAllDimensionValues(Guid dimensionId)
        {
            InvoiceItemGroupingValue groupingValue;
            if (!_invoiceItemRecord.GroupingValuesByDimensionId.TryGetValue(dimensionId, out groupingValue))
                throw new NullReferenceException(String.Format("groupingValue. dimensionId '{0}'", dimensionId));
            var allValues = groupingValue.AllValues;
            if (allValues == null)
                throw new NullReferenceException("allValues");
            return allValues;
        }
        public List<dynamic> GetDistinctDimensionValues(Guid dimensionId)
        {
            return GetAllDimensionValues(dimensionId).Distinct().ToList();
        }
    }
}
