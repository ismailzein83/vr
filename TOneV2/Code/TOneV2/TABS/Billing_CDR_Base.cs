using System;
using System.Collections.Generic;

namespace TABS
{
    public abstract class Billing_CDR_Base : Components.BaseEntity
    {
        public abstract bool IsValid { get; }

        #region DataMembers

        protected long _ID;
        protected DateTime _Attempt;
        protected DateTime? _Alert;
        protected DateTime? _Connect;
        protected DateTime? _Disconnect;
        protected Decimal _DurationInSeconds = Decimal.Zero;
        protected String _CDPN;
        protected String _CGPN;
        protected String _ReleaseCode;
        protected String _ReleaseSource;
        protected Switch _Switch;
        protected long _SwitchCdrID;
        protected string _Tag;
        protected Zone _OurZone;
        protected Zone _SupplierZone;
        protected Zone _OriginatingZone;
        private string _Port_IN;
        private string _Port_OUT;
        public virtual bool IsRerouted { get; set; }
        public virtual string CDPNOut { get; set; }

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
        public virtual string OurCode { get; set; }
        public virtual string SupplierCode { get; set; }
        public Billing_CDR_Base()
        {

        }

        /// <summary>
        /// Initialize as a copy of the given Billing CDR
        /// </summary>
        /// <param name="copy"></param>
        public Billing_CDR_Base(Billing_CDR_Base copy)
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
                this.ReleaseCode = copy.ReleaseCode;
                this.ReleaseSource = copy.ReleaseSource;
                this.Switch = copy.Switch;
                this.SwitchCdrID = copy.SwitchCdrID;
                this.Tag = copy.Tag;
                this.Extra_Fields = copy.Extra_Fields;
                this.OurZone = copy.OurZone;
                this.SupplierZone = copy.SupplierZone;
                this.CustomerID = copy.CustomerID;
                this.SupplierID = copy.SupplierID;
                this.Port_IN = copy.Port_IN;
                this.Port_OUT = copy.Port_OUT;
                this.IsRerouted = copy.IsRerouted;
                this.OurCode = copy.OurCode;
                this.SupplierCode = copy.SupplierCode;
                this.CDPNOut = copy.CDPNOut;
            }
        }


        public virtual int SubscriberID
        {
            get;
            set;
        }

        public virtual long ID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        /// <summary>
        /// The time the call was intiated (Attempt of Call)
        /// </summary>
        public virtual DateTime Attempt
        {
            get { return _Attempt; }
            set { _Attempt = value; }
        }

        /// <summary>
        /// The time the call was connected (to supplier)
        /// </summary>
        public virtual DateTime? Alert
        {
            get { return _Alert; }
            set { _Alert = value; }
        }

        /// <summary>
        /// The time the call was connected (to supplier)
        /// </summary>
        public virtual DateTime? Connect
        {
            get { return _Connect; }
            set { _Connect = value; }
        }

        /// <summary>
        /// The time the call was disconnected
        /// </summary>
        public virtual DateTime? Disconnect
        {
            get { return _Disconnect; }
            set { _Disconnect = value; }
        }

        public virtual Decimal DurationInSeconds
        {
            get { return _DurationInSeconds; }
            set { _DurationInSeconds = value; }
        }

        /// <summary>
        /// Call Destination Pin Number (Who is being Called)
        /// </summary>
        public virtual String CDPN
        {
            get { return _CDPN; }
            set { _CDPN = value; }
        }

        /// <summary>
        /// Call Generating Pin Number (Who Called)
        /// </summary>       
        public virtual String CGPN
        {
            get { return _CGPN; }
            set { _CGPN = value; }
        }

        /// <summary>
        /// Port (Trunk or IP:Port) Out
        /// </summary>
        public virtual string Port_OUT
        {
            get { return _Port_OUT; }
            set { _Port_OUT = value; }
        }

        /// <summary>
        /// Port (Trunk or IP:Port) IN
        /// </summary>
        public virtual string Port_IN
        {
            get { return _Port_IN; }
            set { _Port_IN = value; }
        }

        /// <summary>
        /// The Release Code Generated by Disconnecting Side
        /// </summary>
        public virtual String ReleaseCode
        {
            get { return _ReleaseCode; }
            set { _ReleaseCode = value; }
        }
        
        /// <summary>
        /// The Disconnecting Side
        /// </summary>
        public virtual String ReleaseSource
        {
            get { return _ReleaseSource; }
            set { _ReleaseSource = value; }
        }

        /// <summary>
        /// The Switch From which this CDR originated
        /// </summary>
        public virtual Switch Switch
        {
            get { return _Switch; }
            set { _Switch = value; }
        }

        /// <summary>
        /// The Id of the CDR on the switch
        /// </summary>
        public virtual long SwitchCdrID
        {
            get { return _SwitchCdrID; }
            set { _SwitchCdrID = value; }
        }

        /// <summary>
        /// Additional Information to identify the CDR on the switch
        /// </summary>
        public virtual String Tag
        {
            get { return _Tag; }
            set { _Tag = value; }
        }

        public virtual string Extra_Fields { get; set; }

        public abstract string CustomerID { get; set; }
        public abstract string SupplierID { get; set; }

        public abstract CarrierAccount Customer { get; set; }
        public abstract CarrierAccount Supplier { get; set; }

        public virtual Zone OurZone
        {
            get { return _OurZone; }
            set { _OurZone = value; }
        }

        public virtual Zone SupplierZone
        {
            get { return _SupplierZone; }
            set { _SupplierZone = value; }
        }

        public virtual Zone OriginatingZone
        {
            get { return _OriginatingZone; }
            set { _OriginatingZone = value; }
        }

        public virtual string SIP { get; set; }
        #endregion

        /// <summary>
        /// Get the given named values of extra fields as a single comma separated string of pairs: name1=value1,name2=value2,...
        /// Names with empty or null values WILL be omitted.
        /// </summary>
        /// <param name="namedValues"></param>
        /// <param name="requiredFields"></param>
        /// <returns></returns>
        public static string Get_Extra_Fields(Dictionary<string, string> namedValues)
        {
            return CDR.Get_Extra_Fields(namedValues);
        }

        /// <summary>
        /// Get a dictionary of name/values from the extra fields string
        /// </summary>
        /// <param name="extraFields"></param>
        /// <returns></returns>
        public static Dictionary<string, string> Get_Extra_Fields(string extraFields)
        {
            return CDR.Get_Extra_Fields(extraFields);
        }
    }
}
