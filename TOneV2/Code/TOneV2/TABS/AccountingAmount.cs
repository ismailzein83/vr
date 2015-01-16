using System;

namespace TABS
{
    [Serializable]
    public abstract class PrePostTransaction : Components.BaseEntity
    {
        public virtual long ID { get; set; }

        public virtual CarrierProfile CustomerProfile { get; set; }
        public virtual CarrierAccount Customer { get; set; }
        public virtual CarrierProfile SupplierProfile { get; set; }
        public virtual CarrierAccount Supplier { get; set; }

        public virtual AmountType Type { get; set; }
        public virtual Decimal Amount { get; set; }
        public virtual Currency Currency { get; set; }

        public virtual DateTime Date { get; set; }
        public virtual DateTime LastUpdate { get; set; }

        public virtual string ReferenceNumber { get; set; }
        public virtual string Tag { get; set; }
        public virtual string Note { get; set; }

        public virtual object Carrier
        {
            get
            {
                if (CustomerProfile != null) return CustomerProfile;
                else if (SupplierProfile != null) return SupplierProfile;
                else if (Customer != null) return Customer;
                else if (Supplier != null) return Supplier;
                else return null;
            }
        }

        public virtual string CarrierID
        {
            get
            {
                if (CustomerProfile != null) return CustomerProfile.ProfileID.ToString();
                else if (SupplierProfile != null) return SupplierProfile.ProfileID.ToString();
                else if (Customer != null) return Customer.CarrierAccountID;
                else if (Supplier != null) return Supplier.CarrierAccountID;
                else return null;
            }
        }

        public virtual string CarrierName
        {
            get
            {
                if (CustomerProfile != null) return "Profile - " + CustomerProfile.Name;
                else if (SupplierProfile != null) return "Profile - " + SupplierProfile.Name;
                else if (Customer != null) return "Account - " + Customer.Name;
                else if (Supplier != null) return "Account - " + Supplier.Name;
                else return null;
            }
        }

        public virtual bool isActiveCarrier
        {
            get
            {
                if (CustomerProfile != null) return CustomerProfile.isActive;
                else if (SupplierProfile != null) return SupplierProfile.isActive;
                else if (Customer != null) return Customer.isActive;
                else if (Supplier != null) return Supplier.isActive;
                else return false;
            }
        }

        public virtual bool HasCustomerSMSOnPayment
        {
            get
            {
                if (CustomerProfile != null) return CustomerProfile.CustomerSMSOnPayment;
                else if (SupplierProfile != null) return SupplierProfile.CustomerSMSOnPayment;
                else if (Customer != null) return Customer.CustomerSMSOnPayment;
                else if (Supplier != null) return Supplier.CustomerSMSOnPayment;
                else return false;
            }
        }
        public virtual bool HasCustomerMailOnPayment
        {
            get
            {
                if (CustomerProfile != null) return CustomerProfile.CustomerMailOnPayment;
                else if (SupplierProfile != null) return SupplierProfile.CustomerMailOnPayment;
                else if (Customer != null) return Customer.CustomerMailOnPayment;
                else if (Supplier != null) return Supplier.CustomerMailOnPayment;
                else return false;
            }
        }
        public virtual bool HasSupplierSMSOnPayment
        {
            get
            {
                if (CustomerProfile != null) return CustomerProfile.SupplierSMSOnPayment;
                else if (SupplierProfile != null) return SupplierProfile.SupplierSMSOnPayment;
                else if (Customer != null) return Customer.SupplierSMSOnPayment;
                else if (Supplier != null) return Supplier.SupplierSMSOnPayment;
                else return false;
            }
        }
        public virtual bool HasSupplierMailOnPayment
        {
            get
            {
                if (CustomerProfile != null) return CustomerProfile.SupplierMailOnPayment;
                else if (SupplierProfile != null) return SupplierProfile.SupplierMailOnPayment;
                else if (Customer != null) return Customer.SupplierMailOnPayment;
                else if (Supplier != null) return Supplier.SupplierMailOnPayment;
                else return false;
            }
        }
        public virtual string SMSPhoneNumber
        {
            get
            {
                if (CustomerProfile != null) return CustomerProfile.SMSPhoneNumber ?? "";
                else if (SupplierProfile != null) return SupplierProfile.SMSPhoneNumber ?? "";
                else if (Customer != null) return Customer.CarrierProfile.SMSPhoneNumber ?? "";
                else if (Supplier != null) return Supplier.CarrierProfile.SMSPhoneNumber ?? "";
                else return string.Empty;
            }
        }
        public virtual int _MailTemplateType { get; set; }
    }
}
