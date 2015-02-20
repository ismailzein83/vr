using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.CDR.Entities
{
    public class BillingCDRBase
    {

        public long ID { get;set; }

        public DateTime Attempt { get;set; }

        public DateTime? Alert { get;set; }

        public DateTime? Connect { get;set; }

        public DateTime? Disconnect{ get; set; }

        public virtual Decimal DurationInSeconds{ get; set; }

        public  String CDPN { get; set; }

        public  String CGPN { get; set; }

        public  string Port_OUT { get; set; }

        public  string Port_IN { get; set; }

        public  String ReleaseCode { get; set; }
        
        public  String ReleaseSource { get; set; }

        public  int SwitchID { get; set; }

        public  long SwitchCdrID { get; set; }

        public  String Tag { get; set; }

        public  int OurZoneID { get; set; }

        public  int SupplierZoneID { get; set; }

        public  int OriginatingZoneID { get; set; }

        public  string SIP { get; set; }

        public string Extra_Fields { get; set; }

        public string CustomerID { get; set; }

        public string SupplierID { get; set; }

        public string OurCode { get; set; }

        public string SupplierCode { get; set; }

        public bool IsValid { get; set; }

        public bool IsRerouted { get; set; }

        public string CDPNOut { get; set; }

        public int SubscriberID { get; set; }
        
    }
}
