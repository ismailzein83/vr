using System;

namespace Retail.NIM.Entities
{
    public class Path
    {
        public long Id { get; set; }

        public string PhoneNumber { get; set; }

        public string FullPhoneNumber { get; set; }

        public long Switch { get; set; }

        public long Device { get; set; }

        public long MDF { get; set; }

        public long MDFHorizontal { get; set; }

        public long MDFHorizontalPort { get; set; }

        public long MDFVertical { get; set; }

        public long MDFVerticalPort { get; set; }

        public long Cabinet { get; set; }

        public long CabinetPrimarySide { get; set; }

        public long CabinetPrimarySidePort { get; set; }

        public long CabinetSecondarySide { get; set; }

        public long CabinetSecondarySidePort { get; set; }

        public long DP { get; set; }

        public long DPPort { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedTime { get; set; }

        public int LastModifiedBy { get; set; }

        public DateTime LastModifiedTime { get; set; }
    }
}
