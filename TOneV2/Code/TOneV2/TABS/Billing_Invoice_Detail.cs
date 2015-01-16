using System;

namespace TABS
{
    /// <summary>
    /// Billing_Invoice_Detail object for NHibernate mapped table Billing_Invoice_Details.
    /// </summary>
    [Serializable]
    public class Billing_Invoice_Detail : Billing_Invoice_Base 
    {
        
        #region BaseEntity
        public override string Identifier { get { return "Billing_Invoice_Detail:" + ID.ToString(); } }
        #endregion
    
    }
}
