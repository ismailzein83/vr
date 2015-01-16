using System;
using System.Collections.Generic;


namespace TABS
{
    /// <summary>
    /// Billing_Invoice object for NHibernate mapped table Billing_Invoice.
    /// </summary>
    [Serializable]
    public class Billing_Invoice : Components.BaseEntity
    {
        #region DataMembers
        private bool _IsSent;
        private int _InvoiceID;
        private DateTime _BeginDate;
        private DateTime _EndDate;
        private DateTime _IssueDate;
        private DateTime _DueDate;
        protected CarrierAccount _Customer;
        protected CarrierAccount _Supplier;


        private string _SerialNumber;
        private Decimal _Duration = Decimal.Zero;
        private Decimal _Amount;
        private Currency _Currency;
        private String _IsLocked;
        private String _IsPaid;
        private String _FileName;
        private byte[] _InvoiceAttachement;
        private DateTime? _PaidDate;
        private Decimal _PaidAmount;
        public decimal? VatValue { get; set; }

        private int _NumberOfCalls;

        private IList<Billing_Invoice_Detail> _Billing_Invoice_Details;
        private IList<Billing_Invoice_Cost> _Billing_Invoice_Costs;

        private string _IsAutomatic;

        public virtual bool IsAutomatic
        {
            get { return "Y".Equals(_IsAutomatic); }
            set { _IsAutomatic = value ? "Y" : "N"; }
        }

        public virtual bool IsSent
        {
            get { return _IsSent; }
            set { _IsSent = value; }
        }
        public virtual int InvoiceID
        {
            get { return _InvoiceID; }
            set { _InvoiceID = value; }
        }

        public virtual DateTime BeginDate
        {
            get { return _BeginDate; }
            set { _BeginDate = value; }
        }

        public virtual DateTime EndDate
        {
            get { return _EndDate; }
            set { _EndDate = value; }
        }

        public virtual DateTime IssueDate
        {
            get { return _IssueDate; }
            set { _IssueDate = value; }
        }

        public virtual DateTime DueDate
        {
            get { return _DueDate; }
            set { _DueDate = value; }
        }

        public virtual DateTime? PaidDate
        {
            get { return _PaidDate; }
            set { _PaidDate = value; }
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

        public virtual byte[] InvoiceAttachement
        {
            get { return _InvoiceAttachement; }
            set { _InvoiceAttachement = value; }
        }

        public virtual string SerialNumber
        {
            get { return _SerialNumber; }
            set { _SerialNumber = value; }
        }


        public virtual Decimal Duration
        {
            get { return _Duration; }
            set { _Duration = value; }
        }

        public virtual Decimal Amount
        {
            get { return _Amount; }
            set { _Amount = value; }
        }
        public virtual string FileName
        {
            get { return _FileName; }
            set { _FileName = value; }
        }

        public virtual Currency Currency
        {
            get { return _Currency; }
            set { _Currency = value; }
        }


        public virtual bool IsLocked
        {
            get { return "Y".Equals(_IsLocked); }
            set { _IsLocked = value ? "Y" : "N"; }
        }

        public virtual bool IsPaid
        {
            get { return "Y".Equals(_IsPaid); }
            set { _IsPaid = value ? "Y" : "N"; }
        }

        public virtual Decimal PaidAmount
        {
            get { return _PaidAmount; }
            set { _PaidAmount = value; }
        }

        public virtual int NumberOfCalls
        {
            get { return _NumberOfCalls; }
            set { _NumberOfCalls = value; }
        }

        public virtual string InvoicePrintedNote { get; set; }

        public virtual string InvoiceNotes { get; set; }


        public virtual IList<Billing_Invoice_Cost> Billing_Invoice_Costs
        {
            get
            {
                if (_Billing_Invoice_Costs == null)
                {
                    _Billing_Invoice_Costs = ObjectAssembler.GetBillingInvoiceCosts(this);
                }
                return _Billing_Invoice_Costs;
            }
            set
            {
                _Billing_Invoice_Costs = value;
            }
        }


        public virtual IList<Billing_Invoice_Detail> Billing_Invoice_Details
        {
            get
            {
                if (_Billing_Invoice_Details == null)
                {
                    _Billing_Invoice_Details = ObjectAssembler.GetBillingInvoiceDetails(this);
                }
                return _Billing_Invoice_Details;
            }
            set
            {
                _Billing_Invoice_Details = value;
            }
        }

        public InvoiceData Data { get; protected set; }

        public virtual byte[] SourceFileBytes
        {
            get
            {
                lock (this)
                {
                    if (InvoiceID > 0 && this.Data == null)
                        this.Data = TABS.ObjectAssembler.Get<InvoiceData>(this.InvoiceID);
                    if (this.Data == null)
                        this.Data = new InvoiceData();
                }
                return this.Data.SourceFileBytes;
            }
            set
            {
                if (this.Data == null)
                    this.Data = new InvoiceData();
                this.Data.SourceFileBytes = value;
            }
        }
        public string DefinitionDisplay
        {
            get
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.AppendFormat("{0}", this.Customer.Equals(TABS.CarrierAccount.SYSTEM) ? "Supplier " + Supplier.Name : "Customer " + Customer.Name);
                sb.AppendFormat(" From {0:yyyy-MM-dd}", this.BeginDate);
                sb.AppendFormat(" Till {0:yyyy-MM-dd}", this.EndDate);
                return sb.ToString();
            }
        }

        public bool IsImported
        {
            get { return this.Billing_Invoice_Details == null || this.Billing_Invoice_Details.Count == 0; }
        }

        public virtual string SourceFileName { get; set; }

        #endregion


        #region BaseEntity implementation

        public override string Identifier { get { return "Billing_Invoice:" + InvoiceID.ToString(); } }
        #endregion
    }
}
