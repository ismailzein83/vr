using System;

namespace Retail.NIM.Entities
{
    public class FreeFTTHPath
    {
        public string FDBNumber { get; set; } 

        public long FDB { get; set; }

        public long FDBPort { get; set; }

        public long Splitter { get; set; }

        public long SplitterOutPort { get; set; }

        public long SplitterInPort { get; set; }

        public long OLT { get; set; }

        public long OLTVerticalPort { get; set; }

        public long OLTHorizontalPort { get; set; }

        public long IMS { get; set; }

        public long TID { get; set; }

        public DateTime CreatedTime { get; set; }
    }
}