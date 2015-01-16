using System;
using System.Collections.Generic;
using System.Linq;

namespace TABS
{
    [Serializable]
    public class CarrierProfile : Components.BaseEntity, Interfaces.ICachedCollectionContainer
    {
        public override string Identifier { get { return "CarrierProfile:" + ProfileID; } }

        #region static
        /// <summary>
        /// Return all Clients in a list
        /// </summary>
        public static IList<CarrierProfile> Customers
        {
            get
            {
                List<CarrierProfile> customers = new List<CarrierProfile>();

                lock (ObjectAssembler.SyncRoot)
                {
                    if (!SecurityEssentials.Web.Helper.CurrentWebUser.HasRestriction)
                        foreach (TABS.CarrierProfile profile in All.Values)
                        {
                            if (profile.Accounts.Any(a => (a.AccountType == AccountType.Client || a.AccountType == AccountType.Exchange) && a.CarrierAccountID != "SYS"))
                                customers.Add(profile);
                        }
                    else
                        foreach (TABS.CarrierProfile profile in All.Values)
                        {
                            if (profile.Accounts.Any(a => (a.AccountType == AccountType.Termination || a.AccountType == AccountType.Exchange) && TABS.Security.AccountManagerSettings.CurrentUserAccountSettings.Customers.Keys.Contains(a) && a.CarrierAccountID != "SYS"))
                                customers.Add(profile);
                        }
                }
                return customers;
            }
        }

        /// <summary>
        /// Return all Supliers in a list
        /// </summary>
        public static IList<CarrierProfile> Suppliers
        {
            get
            {
                List<CarrierProfile> suppliers = new List<CarrierProfile>();

                lock (ObjectAssembler.SyncRoot)
                {
                    if (!SecurityEssentials.Web.Helper.CurrentWebUser.HasRestriction)
                        foreach (TABS.CarrierProfile profile in All.Values)
                        {

                            if (profile.Accounts.Any(a => (a.AccountType == AccountType.Termination || a.AccountType == AccountType.Exchange) && a.CarrierAccountID != "SYS"))
                                suppliers.Add(profile);
                        }
                    else
                    {
                        foreach (TABS.CarrierProfile profile in All.Values)
                        {
                            if (profile.Accounts.Any(a => (a.AccountType == AccountType.Termination || a.AccountType == AccountType.Exchange) && TABS.Security.AccountManagerSettings.CurrentUserAccountSettings.Suppliers.Keys.Contains(a) && a.CarrierAccountID != "SYS"))
                                suppliers.Add(profile);
                        }
                    }
                }
                return suppliers;
            }
        }

        internal static Dictionary<int, CarrierProfile> _All;
        public static Dictionary<int, CarrierProfile> All
        {
            get
            {
                lock (ObjectAssembler.SyncRoot)
                {

                    if (_All == null)
                    {
                        _All = ObjectAssembler.GetCarrierProfiles();
                    }
                }
                return _All;
            }
        }
        #endregion static

        #region Data Members
        private int _ProfileID;
        private string _Name;
        private string _CompanyLogoName;
        private string _CompanyName;
        private byte[] _CompanyLogo;
        private string _Address1;
        private string _Address2;
        private string _Address3;
        private string _Country;
        private string _City;
        private string _Telephone;
        private string _Fax;
        private string _BillingContact;
        private string _BillingEmail;
        private string _PricingContact;
        private string _PricingEmail;
        private string _AccountManagerEmail;
        private string _SupportContact;
        private string _SupportEmail;
        private string _TechnicalContact;
        private string _TechnicalEmail;
        private Currency _Currency;
        private string _BankingDetails;
        private int _PaymentTerms;
        private int _DuePeriod;
        private string _IsTaxAffectsCost = "N";
        private string _TaxFormula;
        private decimal _Tax1;
        private decimal _Tax2;
        private decimal _VAT;
        private double? _Guarantee;
        private string _IsDeleted = "N";
        private string _InvoiceByProfile = "N";

        private string _RegistrationNumber;
        private string _EscalationLevel;
        private string _SMSPhoneNumber;
        private string _Website;
        //---------------------Bug#	3031 
        private string _BillingDisputeEmail;
        public virtual string BillingDisputeEmail
        {
            get { return _BillingDisputeEmail; }
            set { _BillingDisputeEmail = value; }
        }
        //----------------------------
        public virtual int ProfileID
        {
            get { return _ProfileID; }
            set { _ProfileID = value; }
        }

        public virtual string EscalationLevel
        {
            get { return _EscalationLevel; }
            set { _EscalationLevel = value; }
        }

        public virtual string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        public virtual string CompanyName
        {
            get { return _CompanyName; }
            set { _CompanyName = value; }
        }
        public virtual string CompanyLogoName
        {
            get { return _CompanyLogoName; }
            set { _CompanyLogoName = value; }
        }
        public virtual byte[] CompanyLogo
        {
            get { return _CompanyLogo; }
            set { _CompanyLogo = value; }
        }

        public virtual string Address1
        {
            get { return _Address1; }
            set { _Address1 = value; }
        }

        public virtual string Address2
        {
            get { return _Address2; }
            set { _Address2 = value; }
        }

        public virtual string Address3
        {
            get { return _Address3; }
            set { _Address3 = value; }
        }

        public virtual string Country
        {
            get { return _Country; }
            set { _Country = value; }
        }

        public virtual string City
        {
            get { return _City; }
            set { _City = value; }
        }

        public virtual string Telephone
        {
            get { return _Telephone; }
            set { _Telephone = value; }
        }

        public virtual string Fax
        {
            get { return _Fax; }
            set { _Fax = value; }
        }

        public virtual string BillingContact
        {
            get { return _BillingContact; }
            set { _BillingContact = value; }
        }

        public virtual string BillingEmail
        {
            get { return _BillingEmail; }
            set { _BillingEmail = value; }
        }

        public virtual string PricingContact
        {
            get { return _PricingContact; }
            set { _PricingContact = value; }
        }

        public virtual string PricingEmail
        {
            get { return _PricingEmail; }
            set { _PricingEmail = value; }
        }

        public virtual string TechnicalContact
        {
            get { return _TechnicalContact; }
            set { _TechnicalContact = value; }
        }

        public virtual string TechnicalEmail
        {
            get { return _TechnicalEmail; }
            set { _TechnicalEmail = value; }
        }

        public virtual string AccountManagerEmail
        {
            get { return _AccountManagerEmail; }
            set { _AccountManagerEmail = value; }
        }

        public virtual string SupportContact
        {
            get { return _SupportContact; }
            set { _SupportContact = value; }
        }

        public virtual string SupportEmail
        {
            get { return _SupportEmail; }
            set { _SupportEmail = value; }
        }


        public virtual Currency Currency
        {
            get { return _Currency; }
            set { _Currency = value; }
        }
        public virtual string RegistrationNumber
        {

            get { return _RegistrationNumber; }
            set { _RegistrationNumber = value; }
        }


        public virtual string BankingDetails
        {
            get { return _BankingDetails; }
            set { _BankingDetails = value; }
        }

        public virtual int PaymentTerms
        {
            get { return _PaymentTerms; }
            set { _PaymentTerms = value; }
        }
        public virtual int DuePeriod
        {
            get { return _DuePeriod; }
            set { _DuePeriod = value; }
        }

        public virtual decimal Tax1
        {
            get { return _Tax1; }
            set { _Tax1 = value; }
        }

        public virtual decimal Tax2
        {
            get { return _Tax2; }
            set { _Tax2 = value; }
        }

        public virtual bool IsTaxAffectsCost
        {
            get { return "Y".Equals(_IsTaxAffectsCost); }
            set { _IsTaxAffectsCost = value ? "Y" : "N"; }
        }

        public virtual bool InvoiceByProfile
        {
            get { return "Y".Equals(_InvoiceByProfile); }
            set { _InvoiceByProfile = value ? "Y" : "N"; }
        }


        public virtual string TaxFormula
        {
            get { return _TaxFormula; }
            set { _TaxFormula = value; }
        }

        public virtual decimal VAT
        {
            get { return _VAT; }
            set { _VAT = value; }
        }

        public virtual double? Guarantee
        {
            get { return _Guarantee; }
            set { _Guarantee = value; }
        }

        public virtual bool IsDeleted
        {
            get { return "Y".Equals(_IsDeleted); }
            set { _IsDeleted = value ? "Y" : "N"; }
        }
        public virtual string SMSPhoneNumber
        {
            get { return _SMSPhoneNumber; }
            set { _SMSPhoneNumber = value; }
        }

        public virtual string Website
        {
            get { return _Website; }
            set { _Website = value; }
        }

        public virtual PaymentType CustomerPaymentType { get; set; }
        public virtual PaymentType SupplierPaymentType { get; set; }
        public virtual int CustomerCreditLimit { get; set; }
        public virtual int SupplierCreditLimit { get; set; }
        public virtual bool IsNettingEnabled { get; set; }

        public virtual DateTime? CustomerActivateDate { get; set; }
        public virtual DateTime? CustomerDeactivateDate { get; set; }
        public virtual DateTime? SupplierActivateDate { get; set; }
        public virtual DateTime? SupplierDeactivateDate { get; set; }

        public virtual bool CustomerSMSOnPayment { get; set; }
        public virtual bool CustomerMailOnPayment { get; set; }
        public virtual bool SupplierSMSOnPayment { get; set; }
        public virtual bool SupplierMailOnPayment { get; set; }

        public virtual bool CustomerAllowPayment { get; set; }
        public virtual bool SupplierAllowPayment { get; set; }

        public virtual string VatID { get; set; }
        public virtual string VatOffice { get; set; }
        public virtual bool IsProfileInvoice { get; set; }

        public string MailTemplateLogo
        {
            get
            {
                try
                {
                    System.Text.StringBuilder builder = new System.Text.StringBuilder();

                    string container = @"<img src=""data:image/png;base64,{0}""/>";

                    var bas64String = System.Convert.ToBase64String(this.CompanyLogo);

                    builder.AppendFormat(container, bas64String);

                    return builder.ToString();
                }
                catch
                {
                    return string.Empty;
                }
            }
        }

        public string Address
        {
            get
            {
                string address = "";
                if (!string.IsNullOrEmpty(Address1)) address += Address1 + " ";
                if (!string.IsNullOrEmpty(Address2)) address += Address2 + " ";
                if (!string.IsNullOrEmpty(Address3)) address += Address3;
                return address;
            }
        }

        #endregion Data Members

        #region Business Members

        protected IList<CarrierAccount> _Accounts;
        protected IList<CarrierDocument> _Documents;

        /// <summary>
        /// Gets the accounts owned by this carrier profile.
        /// </summary>
        public IList<CarrierAccount> Accounts
        {
            get
            {
                lock (this)
                {
                    if (_Accounts == null)
                    {
                        _Accounts = ObjectAssembler.GetCarrierAccounts(this);
                    }
                }
                return _Accounts;
            }
        }

        /// <summary>
        /// Gets the accounts owned by this carrier profile.
        /// </summary>
        public IList<CarrierDocument> Documents
        {
            get
            {
                lock (this)
                {
                    if (_Documents == null)
                    {
                        _Documents = ObjectAssembler.GetCarrierDocuments(this);
                    }
                }
                return _Documents;
            }
        }

        #endregion Business Members

        public override string ToString()
        {
            return Name;
        }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            CarrierProfile other = obj as CarrierProfile;
            if (other != null)
            {
                if (other.ProfileID > 0 && this.ProfileID > 0 && this.Name != null)
                    return this.Name.Equals(other.Name);
                else
                    return this.ProfileID == other.ProfileID;
            }
            return base.Equals(obj);
        }

        #region ILifecycle Members

        public override NHibernate.Classic.LifecycleVeto OnDelete(NHibernate.ISession s)
        {
            IsDeleted = true;
            CheckDeletion();
            return NHibernate.Classic.LifecycleVeto.Veto;
        }

        public override NHibernate.Classic.LifecycleVeto OnSave(NHibernate.ISession s)
        {
            CheckDeletion();
            return NHibernate.Classic.LifecycleVeto.NoVeto;
        }

        public override NHibernate.Classic.LifecycleVeto OnUpdate(NHibernate.ISession s)
        {
            CheckDeletion();
            return NHibernate.Classic.LifecycleVeto.NoVeto;
        }

        protected virtual void CheckDeletion()
        {
            if (this.IsDeleted)
            {
                if (_All != null && _All.ContainsKey(this.ProfileID))
                    _All.Remove(this.ProfileID);

                foreach (CarrierAccount account in this.Accounts)
                    account.IsDeleted = true;

                Exception ex;
                using (NHibernate.ISession session = DataConfiguration.OpenSession())
                {
                    ObjectAssembler.SaveOrUpdate(session, this.Accounts, out ex);
                    session.Clear();
                }
                if (ex != null)
                    throw ex;
            }
        }

        #endregion

        /// <summary>
        /// Needed for marker interface to be called by reflection
        /// </summary>
        public static void ClearCachedCollections()
        {
            _All = null;
            TABS.Components.CacheProvider.Clear(typeof(CarrierProfile).FullName);
        }

        public bool isActive
        {
            get
            {
                if (Accounts != null)
                    return (Accounts.Where(ca => ca.isActive).Count() > 0);
                return false;
            }
        }


        public static List<TABS.CarrierProfile> InActiveProfiles
        {
            get
            {
                List<TABS.CarrierProfile> InActiveProfiles = new List<TABS.CarrierProfile>();
                foreach (TABS.CarrierProfile Prof in TABS.CarrierProfile.All.Values)
                {
                    if (!Prof.Accounts.Any(a => a.ActivationStatus == TABS.ActivationStatus.Active || a.ActivationStatus == ActivationStatus.Testing)) // Testing Account
                        InActiveProfiles.Add(Prof);
                }
                return InActiveProfiles;
            }
        }

    }
}
