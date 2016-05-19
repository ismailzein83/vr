using System;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class SaleCode : ICode
    {
        public long SaleCodeId { get; set; }

        public string Code { get; set; }

        public long ZoneId { get; set; }

        public int CodeGroupId { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }

        public string SourceId { get; set; }
    }
}

