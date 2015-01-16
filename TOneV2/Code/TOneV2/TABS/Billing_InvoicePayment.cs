using System;

namespace TABS
{
    public class Billing_InvoicePayment : Components.BaseEntity
    {
        #region DataMembers
        public virtual int InvoicePaymentID { get; set; }
        public virtual Billing_Invoice Billing_Invoice { get; set; }
        public virtual DateTime? DueDate { get; set; }
        public virtual decimal Amount { get; set; }
        public virtual DateTime? PaidDate { get; set; }
        #endregion

        #region BaseEntity implementation

        public override string Identifier { get { return "Billing_InvoicePayment:" + InvoicePaymentID.ToString(); } }
        #endregion
    }
}
