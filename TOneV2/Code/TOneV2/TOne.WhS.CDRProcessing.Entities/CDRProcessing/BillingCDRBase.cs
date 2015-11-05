using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Integration.Entities;

namespace TOne.WhS.CDRProcessing.Entities
{
    public class BillingCDRBase
    {
        static BillingCDRBase()
        {
            Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(BillingCDRBase), "ID", "Attempt", "CustomerId", "SupplierId", "DurationInSeconds",
                "Alert", "Connect", "Disconnect", "CGPN", "CDPN", "PortOut", "PortIn", "ReleaseCode", "ReleaseSource", "SaleZoneID", "SupplierZoneID", "OriginatingZoneID", "SaleCode"
                , "SupplierCode");
        }
        
        public int ID { get; set; }
        public DateTime Attempt { get; set; }
        public int CustomerId { get; set; }
        public int SupplierId { get; set; }

    
        public DateTime? Alert { get; set; }

        public DateTime? Connect { get; set; }

        public DateTime? Disconnect { get; set; }

        public int DurationInSeconds { get; set; }

        public String CDPN { get; set; }

        public String CGPN { get; set; }

        public string PortOut{ get; set; }

        public string PortIn { get; set; }

        public String ReleaseCode { get; set; }

        public String ReleaseSource { get; set; }
       
        public long SaleZoneID { get; set; }

        public long SupplierZoneID { get; set; }

        public int OriginatingZoneID { get; set; }

        public string SaleCode { get; set; }

        public string SupplierCode { get; set; }
    }
}
