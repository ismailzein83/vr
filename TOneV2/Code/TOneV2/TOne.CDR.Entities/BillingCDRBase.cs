using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.CDR.Entities
{
    public class BillingCDRBase
    {
        
        private long _ID;
        private DateTime _Attempt;
        private DateTime? _Alert;
        private DateTime? _Connect;
        private DateTime? _Disconnect;
        private Decimal _DurationInSeconds = Decimal.Zero;
        private String _CDPN;
        private String _CGPN;
        private String _ReleaseCode;
        private String _ReleaseSource;
        private int _Switch;
        private long _SwitchCdrID;
        private string _Tag;
        private int _OurZone;
        private int _SupplierZone;
        private int _OriginatingZone;
        private string _Port_IN;
        private string _Port_OUT;

        public  long ID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        public  DateTime Attempt
        {
            get { return _Attempt; }
            set { _Attempt = value; }
        }

        public  DateTime? Alert
        {
            get { return _Alert; }
            set { _Alert = value; }
        }

        public  DateTime? Connect
        {
            get { return _Connect; }
            set { _Connect = value; }
        }

        public  DateTime? Disconnect
        {
            get { return _Disconnect; }
            set { _Disconnect = value; }
        }

        public virtual Decimal DurationInSeconds
        {
            get { return _DurationInSeconds; }
            set { _DurationInSeconds = value; }
        }

        public  String CDPN
        {
            get { return _CDPN; }
            set { _CDPN = value; }
        }

        public  String CGPN
        {
            get { return _CGPN; }
            set { _CGPN = value; }
        }

        public  string Port_OUT
        {
            get { return _Port_OUT; }
            set { _Port_OUT = value; }
        }

        public  string Port_IN
        {
            get { return _Port_IN; }
            set { _Port_IN = value; }
        }

        public  String ReleaseCode
        {
            get { return _ReleaseCode; }
            set { _ReleaseCode = value; }
        }
        
        public  String ReleaseSource
        {
            get { return _ReleaseSource; }
            set { _ReleaseSource = value; }
        }

        public  int SwitchID
        {
            get { return _Switch; }
            set { _Switch = value; }
        }

        public  long SwitchCdrID
        {
            get { return _SwitchCdrID; }
            set { _SwitchCdrID = value; }
        }

        public  String Tag
        {
            get { return _Tag; }
            set { _Tag = value; }
        }

        public  int OurZoneID
        {
            get { return _OurZone; }
            set { _OurZone = value; }
        }

        public  int SupplierZoneID
        {
            get { return _SupplierZone; }
            set { _SupplierZone = value; }
        }

        public  int OriginatingZoneID
        {
            get { return _OriginatingZone; }
            set { _OriginatingZone = value; }
        }

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
