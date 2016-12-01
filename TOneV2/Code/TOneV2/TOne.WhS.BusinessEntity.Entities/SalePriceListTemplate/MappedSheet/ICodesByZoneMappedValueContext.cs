using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public interface ICodesByZoneMappedValueContext
    {
        SalePLZoneNotification ZoneNotification { get; set; }
        decimal? Rate { get; set; }
        DateTime? RateBED { get; set; }
        DateTime? RateEED { get; set; }
        char Delimiter { get; set; }
        bool HasCodeRange { get; set; }
        char RangeSeparator { get; set; }
        bool IsCommaDecimalSeparator { get; set; }
        object Value { get; set; }
    }
}
