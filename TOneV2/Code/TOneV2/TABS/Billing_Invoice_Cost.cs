using System;

namespace TABS
{
    /// <summary>
    /// Billing_Invoice_Cost object for NHibernate mapped table Billing_Invoice_Costs.
    /// </summary>
    [Serializable]
    public class Billing_Invoice_Cost : Billing_Invoice_Base
    {
        protected CarrierAccount _Supplier;

        public virtual CarrierAccount Supplier
        {
            get { return _Supplier; }
            set { _Supplier = value; }
        }

        #region BaseEntity
        public override string Identifier { get { return "Billing_Invoice_Cost:" + ID.ToString(); } }
        #endregion
    }
}
