using System;

namespace Retail.NIM.Entities
{
    public class IMSPhoneNumber
    {
        public long Id { get; set; }

        public string PhoneNumber { get; set; }

        public long Area { get; set; }

        public long Site { get; set; }

        public long IMS { get; set; }

        public long LocalAreaCode { get; set; }

        public long IMSLAC { get; set; }

        public int Category { get; set; }

        public Guid Status { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedTime { get; set; }

        public int LastModifiedBy { get; set; }

        public DateTime LastModifiedTime { get; set; }
    }
}