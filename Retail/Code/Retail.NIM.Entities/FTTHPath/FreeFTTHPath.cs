using System;

namespace Retail.NIM.Entities
{
    public class FreeFTTHPath
    {
        public long AreaId { get; set; }
        public string AreaName { get; set; }
        public long SiteId { get; set; }
        public string SiteName { get; set; }

        //IMS
        public long IMSId { get; set; }
        public string IMSName { get; set; }
        public string IMSNumber { get; set; }
        public long IMSCardId { get; set; }
        public string IMSCardName { get; set; }
        public long IMSSlotId { get; set; }
        public string IMSSlotName { get; set; }
        public long IMSTIDId { get; set; }
        public string IMSTIDName { get; set; }

        //OLT
        public long OLTId { get; set; }
        public string OLTName { get; set; }
        public long OLTVendorId { get; set; }
        public string OLTVendorName { get; set; } 
        public long OLTHorizontalId { get; set; }
        public string OLTHorizontalName { get; set; }
        public long OLTHorizontalPortId { get; set; }
        public string OLTHorizontalPortName { get; set; }
        //public long OLTVerticalId { get; set; }
        //public string OLTVerticalName { get; set; }
        //public long OLTVerticalPortId { get; set; }
        //public string OLTVerticalPortName { get; set; }

        //Splitter
        public long SplitterId { get; set; }
        public string SplitterName { get; set; }
        //public long SplitterInPortId { get; set; }
        //public string SplitterInPortName { get; set; }
        public long SplitterOutPortId { get; set; }
        public string SplitterOutPortName { get; set; }

        //FDB
        public long FDBId { get; set; }
        public string FDBName { get; set; }
        public string FDBNumber { get; set; }
        public long FDBPortId { get; set; }
        public string FDBPortName { get; set; }
    }
}