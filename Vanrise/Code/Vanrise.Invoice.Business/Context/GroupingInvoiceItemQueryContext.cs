using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.Business.Context
{
    public class GroupingInvoiceItemQueryContext: IGroupingInvoiceItemQueryContext
    {
        GroupingInvoiceItemQuery _query;
        InvoiceType _invoiceType;
        Dictionary<Guid, DimensionItemField> _dimensions;
        Dictionary<Guid, AggregateItemField> _aggregates;

        public GroupingInvoiceItemQueryContext(GroupingInvoiceItemQuery query)
        {
            if (query == null)
                throw new ArgumentNullException("query");
            _query = query;
            InvoiceTypeManager invoiceTypeManager = new InvoiceTypeManager();
            var invoiceTypeId = query.InvoiceTypeId;
            _invoiceType = invoiceTypeManager.GetInvoiceType(invoiceTypeId);
            if (_invoiceType == null)
                throw new NullReferenceException(String.Format("invoiceType. ID '{0}'", invoiceTypeId));
            if (_invoiceType.Settings == null)
                throw new NullReferenceException(String.Format("invoiceType.Settings. ID '{0}'", invoiceTypeId));
            var itemGrouping = _invoiceType.Settings.ItemGroupings.FirstOrDefault(x => x.ItemGroupingId == query.ItemGroupingId);
            _dimensions = itemGrouping.DimensionItemFields.ToDictionary(x => x.DimensionItemFieldId, v => v);
            _aggregates = itemGrouping.AggregateItemFields.ToDictionary(x => x.AggregateItemFieldId, v => v);
        }
        public InvoiceType GetInvoiceType()
        {
            return _invoiceType;
        }

        public GroupingInvoiceItemQuery Query
        {
            get { return _query; }
        }

        public DimensionItemField GetDimensionItemField(Guid dimensionId)
        {
            if (_dimensions == null)
                throw new NullReferenceException("_dimensions");
            DimensionItemField dimension;
            if (!_dimensions.TryGetValue(dimensionId, out dimension))
                throw new NullReferenceException(String.Format("dimension '{0}'", dimensionId));
            return dimension;
        }

        public AggregateItemField GetAggregateItemField(Guid aggregateId)
        {
            if (_aggregates == null)
                throw new NullReferenceException("_aggregates");
            AggregateItemField aggregate;
            if (!_aggregates.TryGetValue(aggregateId, out aggregate))
                throw new NullReferenceException(String.Format("aggregate '{0}'", aggregateId));
            return aggregate;
        }

    }
}
