using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Invoice.Entities
{
    public interface IGroupingInvoiceItemQueryContext
    {
        GroupingInvoiceItemQuery Query { get; }
        InvoiceType GetInvoiceType();
        DimensionItemField GetDimensionItemField(Guid dimensionId);
        AggregateItemField GetAggregateItemField(Guid aggregateId);
    }
    public interface IMeasureEvaluator
    {
        dynamic GetMeasureValue(IGetMeasureValueContext context);
    }
    public interface IGetMeasureValueContext
    {
        dynamic GetAggregateValue(Guid aggregateName);

        List<dynamic> GetAllDimensionValues(Guid dimensionName);

        List<dynamic> GetDistinctDimensionValues(Guid dimensionName);
    }
}
