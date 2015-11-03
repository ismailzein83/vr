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
                "Alert", "Connect", "Disconnect", "CGPN", "PortOut", "PortIn", "ReleaseCode", "ReleaseSource", "SaleZoneID", "SupplierZoneID", "OriginatingZoneID", "SaleCode"
                , "SupplierCode");
        }
        
        public int ID { get; set; }
        public DateTime Attempt { get; set; }
        public int CustomerId { get; set; }
        public int SupplierId { get; set; }

    
        public DateTime? Alert { get; set; }

        public DateTime? Connect { get; set; }

        public DateTime? Disconnect { get; set; }

        public virtual Decimal DurationInSeconds { get; set; }

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



        public BillingCDRBase(BillingCDRBase copy)
        {
            if (copy != null)
            {
                this.ID = copy.ID;
                this.Attempt = copy.Attempt;
                this.Alert = copy.Alert;
                this.Connect = copy.Connect;
                this.Disconnect = copy.Disconnect;
                this.DurationInSeconds = copy.DurationInSeconds;
                this.CDPN = copy.CDPN;
                this.CGPN = copy.CGPN;
                this.PortOut = copy.PortOut;
                this.PortIn = copy.PortIn;
                this.ReleaseCode = copy.ReleaseCode;
                this.ReleaseSource = copy.ReleaseSource;
                this.SaleZoneID = copy.SaleZoneID;
                this.SupplierZoneID = copy.SupplierZoneID;
                this.OriginatingZoneID = copy.OriginatingZoneID;
                this.CustomerId = copy.CustomerId;
                this.SupplierId = copy.SupplierId;
                this.SaleCode = copy.SaleCode;
                this.SupplierCode = copy.SupplierCode;
            }
        }
        public BillingCDRBase()
        { }
    }
}
