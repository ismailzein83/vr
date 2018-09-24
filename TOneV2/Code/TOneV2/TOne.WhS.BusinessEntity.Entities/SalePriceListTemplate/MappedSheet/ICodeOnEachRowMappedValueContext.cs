using System;
using System.Collections.Generic;
namespace TOne.WhS.BusinessEntity.Entities
{
    public interface ICodeOnEachRowMappedValueContext
    {
        string Zone { get; set; }
        string Code { get; set; }
        DateTime? CodeBED { get; set; }
        DateTime? CodeEED { get; set; }
        decimal? Rate { get; set; }
        DateTime? RateBED { get; set; }
        DateTime? RateEED { get; set; }
        RateChangeType RateChangeType { get; set; }
        IEnumerable<int> ServicesIds { get; set; }
        object Value { get; set; }
        int? CurrencyId { get; set; }
        int CustomerId { get; set; }
        CodeChange CodeChangeType { get; set; }
        string Increment { get; set; }
        Dictionary<int, SalePLOtherRateNotification> OtherRateByRateTypeId { get; set; }
        long? ZoneId { get; set; }
    }
}
