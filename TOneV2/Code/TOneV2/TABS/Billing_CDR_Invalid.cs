
namespace TABS
{
    public class Billing_CDR_Invalid : Billing_CDR_Base
    {
        private string _CustomerID;
        private string _SupplierID;

        public override string CustomerID
        {
            get { return _CustomerID; }
            set { _CustomerID = value; }
        }

        public override string SupplierID
        {
            get { return _SupplierID; }
            set { _SupplierID = value; }
        }

        public override CarrierAccount Customer
        {
            get
            {
                CarrierAccount customer = null;
                if (CustomerID != null) CarrierAccount.All.TryGetValue(CustomerID, out customer);
                return customer;
            }
            set
            {
                CustomerID = (value == null) ? null : value.CarrierAccountID;
            }
        }

        public override CarrierAccount Supplier
        {
            get
            {
                CarrierAccount supplier = null;
                if (SupplierID != null) CarrierAccount.All.TryGetValue(SupplierID, out supplier);
                return supplier;                
            }
            set
            {
                SupplierID = (value == null) ? null : value.CarrierAccountID;                
            }
        }

        public Billing_CDR_Invalid() { }
        public Billing_CDR_Invalid(Billing_CDR_Base copy) : base(copy) { }

        public override bool IsValid { get { return false; } }

        #region BaseEntity
        public override string Identifier { get { return "Billing_CDR_Invalid:" + ID.ToString(); } }
        #endregion
    }
}
