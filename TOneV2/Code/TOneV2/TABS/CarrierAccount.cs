using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Linq;

namespace TABS
{
    [Serializable]
    public class CarrierAccount : Components.FlaggedServicesEntity, Interfaces.IDateTimeSensitive, Interfaces.ICachedCollectionContainer
    {
        #region Account Enumeration
        public class AccountEnumerator : IEnumerator<CarrierAccount>
        {
            IEnumerator _AllEnumerator;
            AccountType[] _Types = new AccountType[] { AccountType.Client, AccountType.Exchange, AccountType.Termination };

            protected AccountEnumerator() { _AllEnumerator = All.Values.GetEnumerator(); }

            public AccountEnumerator(AccountType[] typeFilter)
                : this()
            {
                if (typeFilter != null && typeFilter.Length > 0)
                    _Types = typeFilter;
            }

            /// <summary>
            /// Get an Account enumerator for "Suppliers" or "Clients".
            /// Exchange or Termination are considered Suppliers.
            /// Exchange or Client are considered Clients.
            /// </summary>
            /// <param name="IsSupplier"></param>
            public AccountEnumerator(bool IsSupplier)
                : this()
            {
                // Exchange or Terminal
                if (IsSupplier)
                    _Types = new AccountType[] { AccountType.Exchange, AccountType.Termination };
                // Exchange or Client
                else
                    _Types = new AccountType[] { AccountType.Exchange, AccountType.Client };
            }

            #region IEnumerator<CarrierAccount> Members

            public CarrierAccount Current
            {
                get { return _AllEnumerator.Current as CarrierAccount; }
            }

            #endregion

            #region IDisposable Members

            public void Dispose()
            {
                _AllEnumerator = null;
            }

            #endregion

            #region IEnumerator Members

            object System.Collections.IEnumerator.Current
            {
                get { return _AllEnumerator.Current; }
            }

            public bool MoveNext()
            {
                bool moved = _AllEnumerator.MoveNext();
                while (moved)
                {
                    bool allowed = false;
                    foreach (AccountType allowedType in _Types)
                        if (this.Current.AccountType == allowedType && this.Current.ActivationStatus != ActivationStatus.Inactive)
                        {
                            allowed = true;
                            break;
                        }
                    if (!allowed) moved = _AllEnumerator.MoveNext();
                    else break;
                }
                return moved;
            }

            public void Reset()
            {
                _AllEnumerator.Reset();
            }
            #endregion
        }

        /// <summary>
        /// Get an enumerator for all Suppliers
        /// </summary>
        /// <returns></returns>
        public static IEnumerator<CarrierAccount> GetAllSuppliersEnumerator() { return new AccountEnumerator(true); }

        /// <summary>
        /// Get an enumerator for all Clients
        /// </summary>
        /// <returns></returns>
        public static IEnumerator<CarrierAccount> GetAllClientsEnumerator() { return new AccountEnumerator(false); }

        /// <summary>
        /// Return all Suppliers in a list
        /// </summary>
        public static IList<CarrierAccount> Suppliers
        {
            get
            {
                List<CarrierAccount> suppliers = new List<CarrierAccount>(All.Count);
                IEnumerator<CarrierAccount> enumerator = GetAllSuppliersEnumerator();
                while (enumerator.MoveNext())
                    if (enumerator.Current._ActivationStatus != ActivationStatus.Inactive)
                        suppliers.Add(enumerator.Current);
                suppliers.Remove(CarrierAccount.SYSTEM);
                return suppliers;
            }
        }

        /// <summary>
        /// Returns all suppliers related to account manager
        /// </summary>
        public static IList<CarrierAccount> AccountManagerFilteredSuppliers
        {
            get
            {
                if (SecurityEssentials.Web.Helper.CurrentWebUser != null && SecurityEssentials.Web.Helper.CurrentWebUser.HasRestriction)
                    return Suppliers.Where(s => SecurityEssentials.Web.Helper.CurrentWebUser.AMU.ContainsCustomer(s.CarrierAccountID)).ToList();
                return Suppliers;
            }
        }

        /// <summary>
        /// Return all Clients in a list
        /// </summary>
        public static IList<CarrierAccount> Customers
        {
            get
            {
                List<CarrierAccount> customers = new List<CarrierAccount>(All.Count);
                IEnumerator<CarrierAccount> enumerator = GetAllClientsEnumerator();
                while (enumerator.MoveNext())
                    if (enumerator.Current._ActivationStatus != ActivationStatus.Inactive)
                        customers.Add(enumerator.Current);
                customers.Remove(CarrierAccount.SYSTEM);
                //Restriction functionality
                if (SecurityEssentials.Web.Helper.CurrentWebUser != null && SecurityEssentials.Web.Helper.CurrentWebUser.HasRestriction)
                    return customers.Where(c => SecurityEssentials.Web.Helper.CurrentWebUser.AMU.ContainsCustomer(c.CarrierAccountID)).ToList();
                //return TABS.Security.AccountManagerSettings.CurrentUserAccountSettings.Customers.Keys.Intersect(customers).ToList();
                //End Restriction functionality
                return customers;

            }
        }

        #endregion Account Enumeration
        #region Events
        public event EventHandler RoutingStatusChanged;
        protected void OnRoutingStatusChanged()
        {
            if (RoutingStatusChanged != null)
                RoutingStatusChanged(this, EventArgs.Empty);
        }
        #endregion
        /// <summary>
        /// Needed for marker interface to be called by reflection
        /// </summary>
        public static void ClearCachedCollections()
        {
            _All = null;
            TABS.Components.CacheProvider.Clear(typeof(CarrierAccount).FullName);
        }

        public override string Identifier { get { return "CarrierAccount:" + ((CarrierAccountID == null) ? "<NEW>:" + Name : CarrierAccountID); } }

        internal static Dictionary<string, CarrierAccount> _All;

        /// <summary>
        /// All the Carrier Accounts Defined in the System
        /// </summary>
        public static Dictionary<string, CarrierAccount> All
        {
            get
            {
                lock (ObjectAssembler.SyncRoot)
                {
                    if (_All == null)
                    {
                        ADOObjectAssembler ADOObjectAssembler = new ADOObjectAssembler();
                        Dictionary<int, CarrierProfile> CarrierProfiles = ADOObjectAssembler.GetAllCarrierProfile();
                        Dictionary<int, CarrierGroup> CarrierGroups = ADOObjectAssembler.GetAllCarrierGroups();
                        _All = ADOObjectAssembler.GetAllCarrierAccounts(CarrierProfiles, CarrierGroups);

                        _All = ObjectAssembler.GetCarrierAccounts();
                        //check for system account 
                        if (!_All.ContainsKey(SystemAccountID))
                        {
                            CarrierAccount systemAccount = ObjectAssembler.CreateSystemCarrierAccount();
                            _All.Add(systemAccount.CarrierAccountID, systemAccount);
                        }

                        // check for blocked account
                        if (!_All.ContainsKey(BlockedAccountID))
                        {
                            CarrierAccount blockedAccount = ObjectAssembler.CreateBlockedCarrierAccount();
                            _All.Add(blockedAccount.CarrierAccountID, blockedAccount);
                        }
                    }
                }
                return _All;
            }
        }


        public virtual string CarrierGroups { get; set; }
        //---------------------------------------------------------------------------------------------------------------------------
        public virtual string AccountCarrierGroupsNames
        {
            get
            {
                return string.Join(",", AccountCarrierGroups.Select(v => v.Value.CarrierGroupName.ToString()).ToArray());
            }
        }
        //--------------------------------------------------------------------------------------------------------------------------

        [XmlIgnore]
        public Dictionary<int, CarrierGroup> AccountCarrierGroups
        {
            get
            {
                if (_AccountCarrierGroups == null)
                {
                    _AccountCarrierGroups = ObjectAssembler.GetCarrierGroups(this);
                }
                return _AccountCarrierGroups;
            }
        }

        /// <summary>
        /// The System Account ID (Unique and Identifiable).
        /// </summary>
        internal const string SystemAccountID = "SYS";

        /// <summary>
        /// The Blocked Account ID (Unique and Identifiable).
        /// </summary>
        internal const string BlockedAccountID = "BLK";


        /// <summary>
        /// The SYSTEM Carrier Account. (Us).
        /// </summary>
        public static CarrierAccount SYSTEM { get { return All[SystemAccountID]; } }


        /// <summary>
        /// The Blocked Carrier Account..
        /// </summary>
        public static CarrierAccount BLOCKED { get { return All[BlockedAccountID]; } }

        #region Data Members
        private string _CarrierAccountID;
        private CarrierProfile _CarrierProfile;
        private ActivationStatus _ActivationStatus;
        private RoutingStatus _RoutingStatus;
        private AccountType _AccountType;
        private PaymentType _CustomerPaymentType;
        private PaymentType _SupplierPaymentType;
        private SupplierRatePolicy _SupplierRatepolicy;
        private int _SupplierCreditLimit;
        private CarrierGroup _CarrierGroup;
        private int _CustomerCreditLimit;
        private short _GMTTime;
        private string _IsTOD;
        private string _IsAToZ = "N";
        private string _IsDeleted = "N";
        private string _IsCustomerCeiling;
        private string _IsSupplierCeiling;
        private string _IsProduct;
        private string _Notes;
        private bool _IsOriginatingZonesEnabled;
        private int? _RateIncreaseDays;
        private string _BankReferences;
        private string _CarrierGroups;
        private string _CarrierMask;
        private string _IsCustomDispute = "N";
        private Dictionary<int, CarrierGroup> _AccountCarrierGroups;

        [NonSerialized]
        private Iesi.Collections.Generic.ISet<CarrierAccountConnection> _Connections;
        private IList<TABS.Billing_Invoice> _CarrierInvoices;

        public virtual int NominalCapacityInE1s { get; set; }

        [XmlIgnore]
        public virtual string CustomerMask { get; set; }

        public CarrierAccount CustomerMaskAccount
        {
            get
            {
                if (string.IsNullOrEmpty(CustomerMask)) return TABS.CarrierAccount.SYSTEM;
                TABS.CarrierAccount account = TABS.SpecialSystemParameters.MaskAccount.SystemMask.Accounts.SingleOrDefault(m => m.CarrierAccountID == CustomerMask);
                if (account == null) account = TABS.CarrierAccount.SYSTEM;
                return account;
            }
        }

        public virtual decimal NominalCallMinutesPerHour { get { return NominalCapacityInE1s * 30 * 60; } }

        [XmlIgnore]
        public virtual Iesi.Collections.Generic.ISet<CarrierAccountConnection> Connections
        {
            get { return _Connections; }
            set { _Connections = value; }
        }
        public virtual string InvoiceSerialPattern { get; set; }
        public virtual string CarrierAccountID
        {
            get { return _CarrierAccountID; }
            set { _CarrierAccountID = value; }
        }

        public virtual string IsCustomerCeiling
        {
            get { return _IsCustomerCeiling; }
            set { _IsCustomerCeiling = value; }
        }
        public virtual string IsSupplierCeiling
        {
            get { return _IsSupplierCeiling; }
            set { _IsSupplierCeiling = value; }
        }

        public string CarrierMask
        {
            get { return _CarrierMask; }
            set { _CarrierMask = value; }
        }
        public virtual CarrierProfile CarrierProfile
        {
            get { return _CarrierProfile; }
            set { _CarrierProfile = value; }
        }

        public virtual ActivationStatus ActivationStatus
        {
            get { return _ActivationStatus; }
            set { _ActivationStatus = value; }
        }

        public virtual RoutingStatus RoutingStatus
        {
            get { return _RoutingStatus; }
            set { _RoutingStatus = value; OnRoutingStatusChanged(); }
        }

        public virtual SupplierRatePolicy SupplierRatePolicy
        {
            get { return _SupplierRatepolicy; }
            set { _SupplierRatepolicy = value; }
        }

        public virtual AccountType AccountType
        {
            get { return _AccountType; }
            set { _AccountType = value; }
        }

        public virtual PaymentType CustomerPaymentType
        {
            get { return _CustomerPaymentType; }
            set { _CustomerPaymentType = value; }
        }

        public virtual PaymentType SupplierPaymentType
        {
            get { return _SupplierPaymentType; }
            set { _SupplierPaymentType = value; }
        }
        public virtual int SupplierCreditLimit
        {
            get { return _SupplierCreditLimit; }
            set { _SupplierCreditLimit = value; }
        }


        public virtual CarrierGroup CarrierGroup
        {
            get { return _CarrierGroup; }
            set { _CarrierGroup = value; }
        }
        public virtual int CustomerCreditLimit
        {
            get { return _CustomerCreditLimit; }
            set { _CustomerCreditLimit = value; }
        }


        public virtual short SupplierGMTTime
        {
            get { return _GMTTime; }
            set { _GMTTime = value; }
        }

        public virtual short CustomerGMTTime { get; set; }

        public virtual string Notes
        {
            get { return _Notes; }
            set { _Notes = value; }
        }

        public virtual string BankReferences
        {
            get { return _BankReferences; }
            set { _BankReferences = value; }
        }


        public virtual bool IsTOD
        {
            get { return "Y".Equals(_IsTOD); }
            set { _IsTOD = value ? "Y" : "N"; }
        }
        public virtual bool IsAToZ
        {
            get { return "Y".Equals(_IsAToZ); }
            set { _IsAToZ = value ? "Y" : "N"; }
        }

        public virtual string IsProduct
        {
            get { return _IsProduct; }
            set { _IsProduct = value; }
        }

        public virtual bool IsOriginatingZonesEnabled
        {
            get { return _IsOriginatingZonesEnabled; }
            set { _IsOriginatingZonesEnabled = value; }
        }

        public virtual decimal ServicesAmount { get; set; }

        public virtual decimal ConnectionFees { get; set; }

        public virtual bool IsDeleted
        {
            get { return "Y".Equals(_IsDeleted); }
            set { _IsDeleted = value ? "Y" : "N"; }
        }

        public virtual int? RateIncreaseDays
        {
            get { return _RateIncreaseDays; }
            set { _RateIncreaseDays = value; }
        }

        public virtual bool IsCustomDispute
        {
            get { return "Y".Equals(_IsCustomDispute); }
            set { _IsCustomDispute = value ? "Y" : "N"; }
        }

        public virtual bool IsCustomCodeView { get; set; }
        public virtual int CodeView { get; set; }

        public virtual bool IsPassThroughCustomer { get; set; }
        public virtual bool IsPassThroughSupplier { get; set; }
        public virtual bool RepresentsASwitch { get; set; }
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

        public string MaskInvoiceformat { get; set; }
        public int MaskOverAllCounter { get; set; }
        public int YearlyMaskOverAllCounter { get; set; }
        /// <summary>
        /// for AutoGenerateInvoice
        /// </summary>
        public virtual AutoInvoiceSetting Setting { get; set; }

        //private int? AutoInvoiceSettingID;
        //public virtual AutoInvoiceSetting Setting
        //{
        //    get { return (AutoInvoiceSettingID != null && AutoInvoiceSettingID != 0) ? TABS.AutoInvoiceSetting.All[AutoInvoiceSettingID.Value] : null; }
        //    set { AutoInvoiceSettingID = value == null ? null : new Int32?(value.SettingID); }
        //}

        public string PricelistMaskNameFormat { get; set; }


        public virtual int EffectivenessDays
        {
            get
            {
                return (this.RateIncreaseDays.HasValue)
                    ? this.RateIncreaseDays.Value
                    : (int)TABS.SystemConfiguration.KnownParameters[TABS.KnownSystemParameter.sys_BeginEffectiveRateDays].NumericValue
                    ;
            }
        }


        public CarrierAccount()
        {
        }

        public bool isActive { get { return !(ActivationStatus == ActivationStatus.Inactive); } }

        /// <summary>
        /// Email to be used for importing pricelists by email
        /// </summary>
        public string ImportEmail { get; set; }

        /// <summary>
        /// Code agreed on with supplier
        /// </summary>
        public string ImportSubjectCode { get; set; }

        #endregion Data Members

        #region Business Members

        public string Name
        {
            get
            {
                //Restriction Functionality 
                //if terminition then its found on suppliers and customers setting
                //if Terminiation so only in suppliers
                //if Client so only in customers
                //if (SecurityEssentials.Web.Helper.CurrentWebUser!=null && SecurityEssentials.Web.Helper.CurrentWebUser.HasRestriction)
                //    if (this.AccountType == AccountType.Termination || this.AccountType == AccountType.Exchange)
                //        if (!Security.AccountManagerSettings.CurrentUserAccountSettings.Suppliers.Keys.Contains(this))
                //            return CarrierMask == null ? "N/A" : CarrierMask;
                if (SecurityEssentials.Web.Helper.CurrentWebUser != null && SecurityEssentials.Web.Helper.CurrentWebUser.HasRestriction && !TABS.Security.AccountManagerSettings.CurrentUserAccountSettings.IsEmpty)
                {
                    if (!Security.AccountManagerSettings.CurrentUserAccountSettings.Suppliers.Keys.Contains(this) &&
                        !Security.AccountManagerSettings.CurrentUserAccountSettings.Customers.Keys.Contains(this))
                        return CarrierMask == null ? "N/A" : CarrierMask;
                }
                //END Restriction Functionality

                //OLD CODE
                return CarrierProfile.ToString() + ((TABS.SystemParameter.ShowNameSuffix.BooleanValue.Value)
                                                                            ? ((string.IsNullOrEmpty(NameSuffix)
                                                                                       ? ""
                                                                                       : "(" + NameSuffix + ")"))
                                                                      : "");

            }
        }

        public string NamebyProfileInvoice
        {
            get
            {
                return this.CarrierProfile.InvoiceByProfile ? this.Name + " (Invoice By Profile)" : this.Name;
            }
        }

        public string NameSuffix { get; set; }

        public string NameForPricelistAndInvoice
        {
            get
            {
                return CarrierProfile.ToString() + ((TABS.SystemParameter.ShowNameSuffixForPriceListAndInvoice.BooleanValue.Value)
                                                         ? ((string.IsNullOrEmpty(NameSuffix)
                                                                    ? ""
                                                                    : "(" + NameSuffix + ")"))
                                                         : "");
            }
        }

        public string CompanyNameForPricelistAndInvoice
        {
            get
            {
                return CarrierProfile.CompanyName + ((TABS.SystemParameter.ShowNameSuffixForPriceListAndInvoice.BooleanValue.Value)
                                                         ? ((string.IsNullOrEmpty(NameSuffix)
                                                                    ? ""
                                                                    : "(" + NameSuffix + ")"))
                                                         : "");
            }
        }

        [XmlIgnore]
        protected IList<Zone> _EffectiveSuppliedZones;
        protected PriceList _EffectiveSupplyPriceList;
        protected PriceList _EffectiveSalePriceList;
        [XmlIgnore]
        protected Dictionary<Zone, Rate> _EffectiveSuppliedRates;
        [XmlIgnore]
        protected Dictionary<Zone, Rate> _EffectiveSalesRates;
        [XmlIgnore]
        protected IList<ToDConsideration> _EffectiveToDConsidertions;
        [XmlIgnore]
        protected Dictionary<Zone, Commission> _EffectiveCarrierCommissions;
        [XmlIgnore]
        protected IList<Tariff> _EffectiveTariffs;
        /// <summary>
        /// Gets or sets the currently effective ToD Considerations for this account as a supplier.
        /// </summary>
        [XmlIgnore]
        public IList<Tariff> EffectiveTariffs
        {
            get
            {
                if (_EffectiveTariffs == null)
                {
                    //_EffectiveTariffs = ObjectAssembler.GetTariffs(this, null, DateTime.Now);
                }
                return _EffectiveTariffs;
            }
            set
            {
                _EffectiveTariffs = value;
            }
        }
        /// <summary>
        /// Gets or sets the currently effective ToD Considerations for this account as a supplier.
        /// </summary>
        [XmlIgnore]
        public IList<ToDConsideration> EffectiveToDConsidertions
        {
            get
            {
                if (_EffectiveToDConsidertions == null)
                {
                    //_EffectiveToDConsidertions = ObjectAssembler.GetToDConsiderations(this, null, DateTime.Now);
                }
                return _EffectiveToDConsidertions;
            }
            set
            {
                _EffectiveToDConsidertions = value;
            }
        }
        [XmlIgnore]
        /// <summary>
        /// Gets or sets the currently effective zones supplied by this Account.
        /// </summary>
        public IList<Zone> EffectiveSuppliedZones
        {
            get
            {
                if (_EffectiveSuppliedZones == null)
                {
                   // _EffectiveSuppliedZones = ObjectAssembler.GetZones(this, DateTime.Now);
                }
                return _EffectiveSuppliedZones;
            }
            set
            {
                _EffectiveSuppliedZones = value;
            }
        }
        [XmlIgnore]
        public Dictionary<Zone, Rate> EffectiveSalesRates
        {
            get
            {
                if (_EffectiveSalesRates == null)
                {
                   // _EffectiveSalesRates = ObjectAssembler.GetEffectiveSaleRates(this, DateTime.Now);
                }
                return _EffectiveSalesRates;
            }
            set
            {
                _EffectiveSalesRates = value;
            }
        }
        [XmlIgnore]
        /// <summary>
        /// Gets or sets the currently effective supply rates supplied by this Account 
        /// Note that the rates may be from different priclelists.
        /// </summary>
        public Dictionary<Zone, Rate> EffectiveSuppliedRates
        {
            get
            {
                if (_EffectiveSuppliedRates == null)
                {
                    _EffectiveSuppliedRates = ObjectAssembler.GetEffectiveSupplyRates(this, DateTime.Now);
                }
                return _EffectiveSuppliedRates;
            }
            set
            {
                _EffectiveSuppliedRates = value;
            }
        }

        [XmlIgnore]
        /// <summary>
        /// Sets or gets the Effective Supply Price list for this carrier account.
        /// </summary>
        public PriceList EffectiveSupplyPriceList
        {
            get
            {
                if (_EffectiveSupplyPriceList == null)
                {
                   // _EffectiveSupplyPriceList = ObjectAssembler.GetEffectivePricelist(this, DateTime.Now);
                }
                return _EffectiveSupplyPriceList;
            }
            set
            {
                _EffectiveSupplyPriceList = value;
                if (value != null)
                    _EffectiveSupplyPriceList.Supplier = this;
            }
        }
        [XmlIgnore]
        private PricelistImportOption _PricelistImportOption;
        [XmlIgnore]
        public PricelistImportOption PricelistImportOption
        {
            get
            {
                lock (ObjectAssembler.SyncRoot)
                {
                    if (_PricelistImportOption == null)
                    {
                       // _PricelistImportOption = ObjectAssembler.GetPricelistImportOption(this);
                        if (_PricelistImportOption == PricelistImportOption.None)
                        {
                            _PricelistImportOption = new PricelistImportOption();
                            _PricelistImportOption.Supplier = this;
                            _PricelistImportOption.LastUpdate = DateTime.Now;
                            Exception ex;
                            ObjectAssembler.Save(_PricelistImportOption, out ex);
                        }
                        _PricelistImportOption.Supplier = this;
                    }
                }
                return _PricelistImportOption;
            }
            set
            {
                _PricelistImportOption = value;
                _PricelistImportOption.Supplier = this;
            }
        }
        [XmlIgnore]
        /// <summary>
        /// gets all the invoices for the current customer
        /// </summary>
        public IList<TABS.Billing_Invoice> CarrierInvoices
        {
            get
            {
                if (_CarrierInvoices == null)
                {
                    _CarrierInvoices = null; //ObjectAssembler.GetBillingInvoices(this);
                }
                return _CarrierInvoices;
            }
            set
            {
                _CarrierInvoices = value;
            }
        }

        #endregion Business Members
        //------------------------------------------------> Bug# 2893 <-----------------------------------------------------------------
        /*
         * 
         *this function has been added For custom paging
         *similar for the function below it but plus we are 
         *sending the page size and page index as parameter
         *and getting the record counts as output
         * 
         */
        public IList<PriceList> GetSupplyPriceLists(bool newestFirst, int PageIndex, int Pagesize, out int RecordsCount)
        {
            return ObjectAssembler.GetSupplierPricelists(this, newestFirst, PageIndex, Pagesize, out RecordsCount);
        }
        //--------------------------------------------------------------------------------------------------------
        public System.Data.IDataReader GetSupplyPriceListsADO(bool newestFirst, int From, int To, string WhereCondition, string ColumnName)
        {
            return ObjectAssembler.GetSupplierPricelistsADO(this, newestFirst, From, To, WhereCondition, ColumnName);
        }
        //-------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Get the pricelists supplied by this carrier account.
        /// </summary>
        /// <param name="newestFirst">if true the pricelists will be ordered, newest returned first, otherwise in no particular order</param>
        /// <returns>A list of pricelists</returns>
        public IList<PriceList> GetSupplyPriceLists(bool newestFirst)
        {
            return ObjectAssembler.GetSupplierPricelists(this, newestFirst);
        }

        /// <summary>
        /// Get the pricelists supplied by us.
        /// </summary>
        /// <param name="newestFirst">if true the pricelists will be ordered, newest returned first, otherwise in no particular order</param>
        /// <returns>A list of pricelists</returns>
        public IList<PriceList> GetSalePriceLists(bool newestFirst)
        {
            return ObjectAssembler.GetSalePricelists(this, newestFirst);
        }

        /// <summary>
        /// Get the pricelists supplied by us.
        /// </summary>
        /// <param name="newestFirst">if true the pricelists will be ordered, newest returned first, otherwise in no particular order</param>
        /// <returns>A list of pricelists</returns>
        public IList<PriceList> GetSalePriceLists(bool newestFirst, int pageindex, int pagesize, out int recordCount)
        {
            return ObjectAssembler.GetSalePricelists(this, newestFirst, pageindex, pagesize, out recordCount);
        }
        public System.Data.IDataReader GetSalePriceListsADO(bool newestFirst, int From, int To, string WhereCondition, string ColumnName)
        {
            return ObjectAssembler.GetSalePriceListADO(this, newestFirst, From, To, WhereCondition, ColumnName);
        }

        /// <summary>
        /// Get the Sale Pricelists history for this carrier account (Customer).
        /// </summary>
        /// <returns></returns>
        public IList<PriceList> GetSalePicelistHistory()
        {
            return ObjectAssembler.CurrentSession.CreateQuery("FROM PriceList P WHERE P.Customer=:Customer ORDER BY P.BeginEffectiveDate DESC")
                .SetParameter("Customer", this)
                .List<PriceList>();
        }

        /// <summary>
        /// get the connection types 
        /// </summary>
        /// <param name="newestFirst"></param>
        /// <returns></returns>
        public IList<CarrierAccountConnection> GetSupplierConnectionTypes()
        {
            return ObjectAssembler.GetCarrierAccountConnections(this);
        }

        public override string ToString()
        {
            return this.Name;
        }

        public override bool Equals(object obj)
        {
            CarrierAccount other = obj as CarrierAccount;
            if (obj == null) return false;
            return this.CarrierAccountID.Equals(other.CarrierAccountID);
        }

        public override int GetHashCode()
        {
            return (this._CarrierAccountID == null) ? base.GetHashCode() : this._CarrierAccountID.GetHashCode();
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
                if (_All != null && _All.ContainsKey(this.CarrierAccountID))
                    _All.Remove(this.CarrierAccountID);

                this.ActivationStatus = ActivationStatus.Inactive;
                this.RoutingStatus = RoutingStatus.Blocked;

                List<object> toSave = new List<object>();

                //foreach (Rate effectiveRate in this.EffectiveSuppliedRates.Values)
                //{
                //    toSave.Add(effectiveRate);
                //    effectiveRate.EndEffectiveDate = DateTime.Now;
                //}

                //foreach (Zone effectiveZone in this.EffectiveSuppliedZones)
                //{
                //    toSave.Add(effectiveZone);
                //    effectiveZone.EndEffectiveDate = DateTime.Now;
                //    foreach (Code effectiveCode in effectiveZone.EffectiveCodes)
                //    {
                //        toSave.Add(effectiveCode);
                //        effectiveCode.EndEffectiveDate = DateTime.Now;
                //    }
                //}

                if (this.EffectiveSupplyPriceList != PriceList.None)
                {
                    this.EffectiveSupplyPriceList.EndEffectiveDate = DateTime.Now;
                    toSave.Add(this.EffectiveSupplyPriceList);
                }

                Exception ex;
                using (NHibernate.ISession session = DataConfiguration.OpenSession())
                {
                    ObjectAssembler.SaveOrUpdate(session, toSave, out ex);
                    session.Clear();
                }
                if (ex != null)
                    throw ex;

            }
        }

        #endregion

        #region IDateTimeSensitive Members

        public TABS.Interfaces.IDateTimeSensitive RefreshableContainer
        {
            get { return null; }
        }

        public IEnumerable<TABS.Interfaces.IDateTimeSensitive> RefreshableChildren
        {
            get { return null; }
        }

        protected DateTime? _LastRefresh;

        public DateTime? LastRefresh { get { return _LastRefresh; } }

        /// <summary>
        /// Will nullify all "Effective" stuff to refresh them on demand.
        /// </summary>
        public void Refresh()
        {
            this._EffectiveSuppliedRates = null;
            this._EffectiveSuppliedZones = null;
            this._EffectiveSupplyPriceList = null;

            this._EffectiveSalesRates = null;
            this._EffectiveSalePriceList = null;

            this._EffectiveTariffs = null;
            this._EffectiveToDConsidertions = null;
            this._EffectiveCarrierCommissions = null;

            this._LastRefresh = DateTime.Now;
        }

        #endregion
    }
}