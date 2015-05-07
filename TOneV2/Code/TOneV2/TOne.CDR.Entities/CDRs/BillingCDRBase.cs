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


        public static DateTime? GetFirstValidDateTime(params DateTime?[] times)
        {
            DateTime? result = null;
            foreach (var time in times)
                if (time.HasValue) return time;
            return result;
        }

        /// <summary>
        /// Calculated PDD in seconds
        /// </summary>
        public virtual decimal PDDInSeconds
        {
            get
            {
                // If not alert, connect
                var time = GetFirstValidDateTime(Alert, Connect);
                var pdd = time.HasValue ? // alert or connect
                        (decimal)time.Value.Subtract(Attempt).TotalSeconds
                        : (Disconnect.HasValue ? (decimal)Disconnect.Value.Subtract(Attempt).TotalSeconds - DurationInSeconds : 0);
                return pdd < 0 ? 0 : pdd;
            }
        }

        public BillingCDRBase()
        {
        }
        public BillingCDRBase(BillingCDRBase copy)
        {
            if (copy != null)
            {
                this.ID  = copy.ID ;
                this.Attempt  = copy.Attempt ;
                this.Alert  = copy.Alert ;
                this.Connect  = copy.Connect ;
                this.Disconnect  = copy.Disconnect ;
                this.DurationInSeconds  = copy.DurationInSeconds ;
                this.CDPN   = copy.CDPN  ;
                this.CGPN   = copy.CGPN  ;
                this.Port_OUT   = copy.Port_OUT  ;
                this.Port_IN   = copy.Port_IN  ;
                this.ReleaseCode  = copy.ReleaseCode ;
                this.ReleaseSource   = copy.ReleaseSource  ;
                this.SwitchID   = copy.SwitchID  ;
                this.SwitchCdrID   = copy.SwitchCdrID  ;
                this.Tag   = copy.Tag  ;
                this.OurZoneID   = copy.OurZoneID  ;
                this.SupplierZoneID   = copy.SupplierZoneID  ;
                this.OriginatingZoneID   = copy.OriginatingZoneID  ;
                this.SIP   = copy.SIP  ;
                this.Extra_Fields   = copy.Extra_Fields  ;
                this.CustomerID   = copy.CustomerID  ;
                this.SupplierID   = copy.SupplierID  ;
                this.OurCode   = copy.OurCode  ;
                this.SupplierCode   = copy.SupplierCode  ;
                this.IsValid   = copy.IsValid  ;
                this.IsRerouted   = copy.IsRerouted  ;
                this.CDPNOut   = copy.CDPNOut  ;
                this.SubscriberID   = copy.SubscriberID  ;
            }
        }
        
    }
}
