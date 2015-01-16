
namespace TABS
{
    /// <summary>
    /// Billing_CDR_Sale object for NHibernate mapped table Billing_CDR_Sale.
    /// </summary>

    public class Billing_CDR_Sale : Billing_CDR_Pricing_Base
    {
        #region BaseEntity
        public override string Identifier { get { return "Billing_CDR_Sale:" + ID.ToString(); } }
        #endregion
    }
}
