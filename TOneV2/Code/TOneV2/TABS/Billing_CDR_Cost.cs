
namespace TABS
{
    /// <summary>
    /// Billing_CDR_Cost object for NHibernate mapped table Billing_CDR_Cost.
    /// </summary>

    public class Billing_CDR_Cost : Billing_CDR_Pricing_Base
    {
        #region BaseEntity
        public override string Identifier { get { return "Billing_CDR_Cost:" + ID.ToString(); } }
        #endregion
    }
}
