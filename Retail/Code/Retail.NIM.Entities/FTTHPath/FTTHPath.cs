using System;

namespace Retail.NIM.Entities
{
    public class FTTHPath
    {
        public long Area { get; set; }

        public long Site { get; set; }

        public long LocalAreaCode { get; set; }

        public string PhoneNumber { get; set; }

        public string FullPhoneNumber { get; set; }

        public long IMS { get; set; }

        public long IMSCard { get; set; }

        public long IMSSlot { get; set; }

        public long IMSTID { get; set; }

        public long OLT { get; set; }

        public long OLTHorizontal { get; set; }

        public long OLTHorizontalPort { get; set; }

        public long Splitter { get; set; }

        public long SplitterOutPort { get; set; }

        public long FDB { get; set; }

        public long FDBPort { get; set; }

        public long Id { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedTime { get; set; }

        public int LastModifiedBy { get; set; }

        public DateTime LastModifiedTime { get; set; }
    }
}