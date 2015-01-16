using System;

namespace TABS
{
    /// <summary>
    /// define the billing invoice base applied to the cost and 
    /// </summary>
    [Serializable]
    public class Billing_Invoice_Base : Components.BaseEntity
    {
        #region DataMembers

        private int _ID;
        private int _InvoiceID;
        private String _Destination;
        private DateTime _FromDate;
        private DateTime _TillDate;
        private Decimal _Duration;
        private Decimal _Rate;
        private ToDRateType _RateType;
        private Decimal _Amount;
        private Currency _Currency;
        private int _NumberOfCalls;

        private Billing_Invoice _Billing_Invoice;

        public virtual int ID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        public virtual int InvoiceID
        {
            get { return _InvoiceID; }
            set { _InvoiceID = value; }
        }

        public virtual String Destination
        {
            get { return _Destination; }
            set { _Destination = value; }
        }

        public virtual DateTime FromDate
        {
            get { return _FromDate; }
            set { _FromDate = value; }
        }

        public virtual DateTime TillDate
        {
            get { return _TillDate; }
            set { _TillDate = value; }
        }

        public virtual Decimal Duration
        {
            get { return _Duration; }
            set { _Duration = value; }
        }

        public virtual Decimal Rate
        {
            get { return _Rate; }
            set { _Rate = value; }
        }

        public virtual ToDRateType RateType
        {
            get { return _RateType; }
            set { _RateType = value; }
        }

        public virtual Decimal Amount
        {
            get { return _Amount; }
            set { _Amount = value; }
        }

        public virtual Currency Currency
        {
            get { return _Currency; }
            set { _Currency = value; }
        }

        public virtual int NumberOfCalls
        {
            get { return _NumberOfCalls; }
            set { _NumberOfCalls = value; }
        }

        public virtual int MonthNumber { get; set; }
        public virtual int YearNumber { get; set; }
        public virtual Billing_Invoice Billing_Invoice
        {
            get { return _Billing_Invoice; }
            set { _Billing_Invoice = value; }
        }


        #endregion

        public override string Identifier
        {
            get { return "Billing_invoice:" + ID.ToString(); }
        }
    }
}
