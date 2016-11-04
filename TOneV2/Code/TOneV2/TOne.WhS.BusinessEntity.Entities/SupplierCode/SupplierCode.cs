using System;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class SupplierCode : ICode, IBusinessEntity
    {
        public long SupplierCodeId { get; set; }

        public string Code { get; set; }

        public long ZoneId { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }

        public string SourceId { get; set; }

        public int? CodeGroupId { get; set; }
    }
}
