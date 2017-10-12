using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class SalePricelistCodeChange
    {
        public long? ZoneId { get; set; }
        public string ZoneName { get; set; }
        public string RecentZoneName { get; set; }
        public int CountryId { get; set; }
        public string Code { get; set; }
        public CodeChange ChangeType { get; set; }
        public long PricelistId { get; set; }
        public long BatchId { get; set; }
        public DateTime BED { get; set; }
        public DateTime? EED { get; set; }

    }
    public class SalePricelistCode
    {
        public string Code { get; set; }
        public DateTime BED { get; set; }
        public DateTime? EED { get; set; }
    }

    public enum CodeChange
    {
        [Description("Not Changed")]
        NotChanged = 0,

        [Description("New")]
        New = 1,
        [Description("Closed")]
        Closed = 2,
        [Description("Moved")]
        Moved = 3
    }
}
