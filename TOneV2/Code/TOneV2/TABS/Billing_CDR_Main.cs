
namespace TABS
{
    /// <summary>
    /// Billing_CDR_Main object for NHibernate mapped table Billing_CDR_Main.
    /// </summary>

    public class Billing_CDR_Main : Billing_CDR_Base
    {
        private CarrierAccount _Customer;
        private CarrierAccount _Supplier;

        public override CarrierAccount Customer
        {
            get { return _Customer; }
            set { _Customer = value; }
        }

        public override CarrierAccount Supplier
        {
            get { return _Supplier; }
            set { _Supplier = value; }
        }

        public override string CustomerID
        {
            get { return _Customer == null ? null : _Customer.CarrierAccountID; }
            set { _Customer = (value != null && CarrierAccount.All.ContainsKey(value)) ? CarrierAccount.All[value] : null; }
        }

        public override string SupplierID
        {
            get { return (_Supplier == null) ? null : _Supplier.CarrierAccountID; }
            set { _Supplier = (value != null && CarrierAccount.All.ContainsKey(value)) ? CarrierAccount.All[value] : null; }
        }

        public Billing_CDR_Main() { }
        public Billing_CDR_Main(Billing_CDR_Base copy) : base(copy) { }

        private Billing_CDR_Cost _Billing_CDR_Cost;
        private Billing_CDR_Sale _Billing_CDR_Sale;

        public virtual Billing_CDR_Cost Billing_CDR_Cost
        {
            get { return _Billing_CDR_Cost; }
            set { _Billing_CDR_Cost = value; }
        }

        public virtual Billing_CDR_Sale Billing_CDR_Sale
        {
            get { return _Billing_CDR_Sale; }
            set { _Billing_CDR_Sale = value; }
        }

        public override Zone OurZone
        {
            get { return base.OurZone; }
            set { base.OurZone = value; if (_Billing_CDR_Sale != null) _Billing_CDR_Sale.Zone = value; }
        }

        public override Zone SupplierZone
        {
            get { return base.SupplierZone; }
            set { base.SupplierZone = value; if (_Billing_CDR_Cost != null) _Billing_CDR_Cost.Zone = value; }
        }

        public override bool IsValid { get { return true; } }

        #region BaseEntity
        public override string Identifier { get { return "Billing_CDR_Main:" + ID.ToString(); } }
        #endregion
    }
}
