using System;
using System.Collections.Generic;

using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class CodeOnEachRowMappedValueContext : ICodeOnEachRowMappedValueContext
    {
        public string Zone { get; set; }
        public string Code { get; set; }
        public DateTime? CodeBED { get; set; }
        public DateTime? CodeEED { get; set; }
        public decimal? Rate { get; set; }
        public DateTime? RateBED { get; set; }
        public DateTime? RateEED { get; set; }
        public IEnumerable<int> ServicesIds { get; set; }
        public object Value { get; set; }
        public RateChangeType RateChangeType { get; set; }
        public int? CurrencyId { get; set; }
        public int CustomerId { get; set; }
        public CodeChange CodeChangeType { get; set; }
        public string Increment { get; set; }
        public Dictionary<int, SalePLOtherRateNotification> OtherRateByRateTypeId { get; set; }
        public long? ZoneId { get; set; }
    }
}
