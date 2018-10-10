using System;

namespace Retail.BusinessEntity.Entities
{
    public class ReleaseCause
    {
        public int ReleaseCauseId { get; set; }

        public string ReleaseCode { get; set; }

        public string ReleaseCodeName { get; set; }

        public bool IsDelivered { get; set; }

        public string Description { get; set; }

        public DateTime CreatedTime { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? LastModifiedTime { get; set; }

        public int? LastModifiedBy { get; set; }
    }
}
