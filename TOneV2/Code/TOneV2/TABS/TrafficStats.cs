using System;

namespace TABS
{
    public class TrafficStats
    {
        private Zone _OurZone;
        private Zone _SupplierZone;
        private Zone _OriginatingZone;
        private CarrierAccount _Customer;
        private CarrierAccount _Supplier;
        private Switch _Switch;
        private DateTime _FirstCDRAttempt;
        private DateTime _LastCDRAttempt;
        private int _Attempts;
        private int _SuccessfulAttempts;
        private int _DeliveredAttempts;
        private decimal _DurationsInSeconds;
        private TimeSpan? _PDD;
        private decimal _MaxDurationInSeconds;
        private string _Port_IN;
        private string _Port_OUT;
        public bool Saveable { get; set; }
        private TimeSpan _Utilization;
        private String _ReleaseCode;
        private String _SupplierCode;
        private DateTime _CallDate;
        private decimal _PGAD;
        private int _CeiledDuration;
       
        #region Properties
        public virtual long ID { get; set; }

        public virtual int Attempts
        {
            get { return _Attempts; }
            set { _Attempts = value; }
        }

        public virtual decimal PGAD
        {
            get { return _PGAD; }
            set { _PGAD = value; }
        }

        public virtual int CeiledDuration
        {
            get { return _CeiledDuration; }
            set { _CeiledDuration = value; }
        }

        public virtual DateTime CallDate
        {
            get { return _CallDate; }
            set { _CallDate = value; }
        }

        public virtual int SuccessfulAttempts
        {
            get { return _SuccessfulAttempts; }
            set { _SuccessfulAttempts = value; }
        }
        public virtual int DeliveredAttempts
        {
            get { return _DeliveredAttempts; }
            set { _DeliveredAttempts = value; }

        }
        public virtual decimal DurationsInSeconds
        {
            get { return _DurationsInSeconds; }
            set { _DurationsInSeconds = value; }
        }

        public virtual TimeSpan? PDD
        {
            get { return _PDD; }
            set { _PDD = value; }
        }

        public virtual decimal? PDDInSeconds
        {
            get { return _PDD.HasValue ? (decimal?)(_PDD.Value.TotalSeconds) : null; }
            set { _PDD = value.HasValue ? (TimeSpan?)TimeSpan.FromSeconds((double)value) : null; }
        }

        public virtual DateTime FirstCDRAttempt
        {
            get { return _FirstCDRAttempt; }
            set { _FirstCDRAttempt = value; }
        }
        public virtual DateTime LastCDRAttempt
        {
            get { return _LastCDRAttempt; }
            set { _LastCDRAttempt = value; }
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

        public virtual Zone OurZone
        {
            get { return _OurZone; }
            set { _OurZone = value; }
        }

        public virtual Zone OriginatingZone
        {
            get { return _OriginatingZone; }
            set { _OriginatingZone = value; }
        }

        public virtual Zone SupplierZone
        {
            get { return _SupplierZone; }
            set { _SupplierZone = value; }
        }

        public virtual CarrierAccount Customer
        {
            get { return _Customer; }
            set { _Customer = value; }
        }

        public virtual CarrierAccount Supplier
        {
            get { return _Supplier; }
            set { _Supplier = value; }
        }

        public virtual Switch Switch
        {
            get { return _Switch; }
            set { _Switch = value; }
        }

        public virtual decimal MaxDurationInSeconds
        {
            get { return _MaxDurationInSeconds; }
            set { _MaxDurationInSeconds = value; }
        }

        public virtual TimeSpan Utilization
        {
            get { return _Utilization; }
            set { _Utilization = value; }
        }

        public virtual decimal UtilizationInSeconds
        {
            get { return (decimal)_Utilization.TotalSeconds; }
            set { _Utilization = TimeSpan.FromSeconds((double)value); }
        }

        public virtual string ReleaseCode
        {
            get { return _ReleaseCode; }
            set { _ReleaseCode = value; }
        }

        public virtual string SupplierCode
        {
            get { return _SupplierCode; }
            set { _SupplierCode = value; }
        }

        public virtual int NumberOfCalls { get; set; }
        public virtual int DeliveredNumberOfCalls { get; set; }
        public virtual int ReleaseSourceAParty { get; set; }

        #endregion
    }
}
