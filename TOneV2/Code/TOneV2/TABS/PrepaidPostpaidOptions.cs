using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TABS
{
    [Serializable]
    public class PrepaidPostpaidOptions : Interfaces.ICachedCollectionContainer, IComparable<PrepaidPostpaidOptions>
    {
        internal static Dictionary<string, Dictionary<decimal, PrepaidPostpaidOptions>> _PrepaidCustomersOptions;
        internal static Dictionary<string, Dictionary<decimal, PrepaidPostpaidOptions>> _PrepaidSuppliersOptions;
        internal static Dictionary<string, Dictionary<decimal, PrepaidPostpaidOptions>> _PostpaidCustomersOptions;
        internal static Dictionary<string, Dictionary<decimal, PrepaidPostpaidOptions>> _PostpaidSuppliersOptions;

        /// <summary>
        /// Needed for marker interface to be called by reflection
        /// </summary>
        public static void ClearCachedCollections()
        {
            _All = null;
            _PrepaidSuppliersOptions = null;
            _PrepaidCustomersOptions = null;
            _PostpaidSuppliersOptions = null;
            _PostpaidCustomersOptions = null;
        }

        public static Dictionary<string, Dictionary<decimal, PrepaidPostpaidOptions>> PrepaidCustomersOptions
        {
            get
            {
                lock (typeof(PrepaidPostpaidOptions))
                {
                    if (_PrepaidCustomersOptions == null)
                        _PrepaidCustomersOptions = ObjectAssembler.GetPrepaidPostpaidOptions(true, true);
                }
                return _PrepaidCustomersOptions;
            }
        }

        public static Dictionary<string, Dictionary<decimal, PrepaidPostpaidOptions>> PrepaidSuppliersOptions
        {
            get
            {
                lock (typeof(PrepaidPostpaidOptions))
                {
                    if (_PrepaidSuppliersOptions == null)
                        _PrepaidSuppliersOptions = ObjectAssembler.GetPrepaidPostpaidOptions(false, true);
                }
                return _PrepaidSuppliersOptions;
            }
        }

        public static Dictionary<string, Dictionary<decimal, PrepaidPostpaidOptions>> PostpaidCustomersOptions
        {
            get
            {
                lock (typeof(PrepaidPostpaidOptions))
                {
                    if (_PostpaidCustomersOptions == null)
                        _PostpaidCustomersOptions = ObjectAssembler.GetPrepaidPostpaidOptions(true, false);
                }
                return _PostpaidCustomersOptions;
            }
        }

        public static Dictionary<string, Dictionary<decimal, PrepaidPostpaidOptions>> PostpaidSuppliersOptions
        {
            get
            {
                lock (typeof(PrepaidPostpaidOptions))
                {
                    if (_PostpaidSuppliersOptions == null)
                        _PostpaidSuppliersOptions = ObjectAssembler.GetPrepaidPostpaidOptions(false, false);
                }
                return _PostpaidSuppliersOptions;
            }
        }

        internal static Dictionary<int, PrepaidPostpaidOptions> _All;
        public static Dictionary<int, PrepaidPostpaidOptions> All
        {
            get
            {
                lock (ObjectAssembler.SyncRoot)
                {

                    if (_All == null)
                    {
                        _All = new Dictionary<int, PrepaidPostpaidOptions>();
                        IList<TABS.PrepaidPostpaidOptions> options = ObjectAssembler.GetList<PrepaidPostpaidOptions>();
                        foreach (var p in options)
                            _All.Add(p.PrepaidPostpaidOptionsID, p);
                    }
                }
                return _All;
            }
            set { _All = value; }
        }

        public static Dictionary<string, IList<PrepaidPostpaidOptions>> GetOptions(bool isCustomer, bool isPrepaid)
        {
            var query = from p in All.Values
                        where p.IsCustomer == isCustomer && p.IsPrepaid == isPrepaid
                        select p;

            Dictionary<string, IList<PrepaidPostpaidOptions>> dicOptions = new Dictionary<string, IList<PrepaidPostpaidOptions>>();
            foreach (var item in query)
            {
                string key = isCustomer
                                ? ((item.CustomerProfile != null) ? "P_" + item.CustomerProfile.ProfileID : "A_" + item.Customer.CarrierAccountID)
                                : ((item.SupplierProfile != null) ? "P_" + item.SupplierProfile.ProfileID : "A_" + item.Supplier.CarrierAccountID);
                if (!dicOptions.ContainsKey(key))
                    dicOptions[key] = new List<PrepaidPostpaidOptions>();
                dicOptions[key].Add(item);
            }
            return dicOptions;
        }

        public virtual int PrepaidPostpaidOptionsID { get; set; }
        public virtual CarrierProfile CustomerProfile { get; set; }
        public virtual CarrierProfile SupplierProfile { get; set; }
        public virtual CarrierAccount Customer { get; set; }
        public virtual CarrierAccount Supplier { get; set; }
        private string _MinimumActionEmailInterval;
        public virtual TimeSpan? MinimumActionEmailInterval
        {
            get { return _MinimumActionEmailInterval == null ? default(TimeSpan?) : TimeSpan.Parse(_MinimumActionEmailInterval); }
            set { _MinimumActionEmailInterval = value == null ? null : value.ToString(); }
        }
        public virtual string Email { get; set; }
        public virtual decimal Amount { get; set; }
        public virtual decimal Percentage { get; set; }
        public virtual EventActions Actions { get; set; }
        public virtual bool IsCustomer { get; set; }
        public virtual bool IsPrepaid { get; set; }

        public virtual string CarrierID
        {
            get
            {
                return (IsCustomer)
                            ? ((Customer != null)
                                        ? Customer.CarrierAccountID
                                        : CustomerProfile.ProfileID.ToString())
                            : ((Supplier != null)
                                        ? Supplier.CarrierAccountID
                                        : SupplierProfile.ProfileID.ToString());
            }
        }

        public virtual string Currency
        {
            get
            {
                return (IsCustomer)
                            ? ((Customer != null)
                                    ? Customer.CarrierProfile.Currency.Symbol
                                    : CustomerProfile.Currency.Symbol)
                            : ((Supplier != null)
                                    ? Supplier.CarrierProfile.Currency.Symbol
                                    : SupplierProfile.Currency.Symbol);
            }
        }

        public virtual string CarrierName
        {
            get
            {
                return (IsCustomer)
                            ? ((Customer != null)
                                        ? Customer.Name
                                        : CustomerProfile.Name)
                            : ((Supplier != null)
                                    ? Supplier.Name
                                    : SupplierProfile.Name);
            }
        }
        public virtual string MaskedCarrierName
        {
            get
            {
                if (IsCustomer)
                    return (Customer != null)
                                    ? ((Customer.CustomerMaskAccount != null)
                                                        ? Customer.CustomerMaskAccount.CarrierProfile.Name
                                                        : TABS.CarrierAccount.SYSTEM.CarrierProfile.Name)
                                    : ((CustomerProfile.Accounts.Count() > 1 && CustomerProfile.Accounts.First().CustomerMaskAccount != null)
                                                        ? CustomerProfile.Accounts.First().CustomerMaskAccount.CarrierProfile.Name
                                                        : TABS.CarrierAccount.SYSTEM.CarrierProfile.Name);
                return (Supplier != null)
                            ? ((Supplier.CustomerMaskAccount != null)
                                    ? Supplier.CustomerMaskAccount.CarrierProfile.Name
                                    : TABS.CarrierAccount.SYSTEM.CarrierProfile.Name)
                            : ((SupplierProfile.Accounts.Count() > 1 && SupplierProfile.Accounts.First().CustomerMaskAccount != null)
                                    ? SupplierProfile.Accounts.First().CustomerMaskAccount.CarrierProfile.Name
                                    : TABS.CarrierAccount.SYSTEM.CarrierProfile.Name);
            }
            //get
            //{
            //    if (IsCustomer)
            //    {
            //        if (Customer != null)
            //        {
            //            if (!Customer.CustomerMaskAccount.Equals(TABS.CarrierAccount.SYSTEM))
            //                return Customer.CustomerMaskAccount.Name;
            //            return Customer.Name;
            //        }
            //        else
            //        {
            //            if (CustomerProfile.Accounts.Count() > 1 && !CustomerProfile.Accounts.First().CustomerMaskAccount.Equals(TABS.CarrierAccount.SYSTEM))
            //                return CustomerProfile.Accounts.First().CustomerMaskAccount.Name;
            //            return CustomerProfile.Name;
            //        }
            //    }
            //    else
            //    {
            //        if (Supplier != null)
            //        {
            //            if (!Supplier.CustomerMaskAccount.Equals(TABS.CarrierAccount.SYSTEM))
            //                return Supplier.CustomerMaskAccount.Name;
            //            return Supplier.Name;
            //        }
            //        else
            //        {
            //            if (SupplierProfile.Accounts.Count() > 1 && !SupplierProfile.Accounts.First().CustomerMaskAccount.Equals(TABS.CarrierAccount.SYSTEM))
            //                return SupplierProfile.Accounts.First().CustomerMaskAccount.Name;
            //            return SupplierProfile.Name;
            //        }
            //    }
            //}
        }

        public string CarrierBillingEmail
        {
            get
            {
                return (IsCustomer)
                            ? (Customer != null)
                                    ? Customer.CarrierProfile.BillingEmail
                                    : CustomerProfile.BillingEmail
                            : (Supplier != null)
                                    ? Supplier.CarrierProfile.BillingEmail
                                    : SupplierProfile.BillingEmail;
                //if (IsCustomer)
                //{
                //    if (Customer != null)
                //    {
                //        if (!Customer.CustomerMaskAccount.Equals(TABS.CarrierAccount.SYSTEM))
                //            return Customer.CustomerMaskAccount.CarrierProfile.BillingEmail;
                //        return Customer.CarrierProfile.BillingEmail;
                //    }
                //    else
                //    {
                //        if (CustomerProfile.Accounts.Count() > 1 && !CustomerProfile.Accounts.First().CustomerMaskAccount.Equals(TABS.CarrierAccount.SYSTEM))
                //            return CustomerProfile.Accounts.First().CustomerMaskAccount.CarrierProfile.BillingEmail;
                //        return CustomerProfile.BillingEmail;
                //    }
                //}
                //else
                //{
                //    if (Supplier != null)
                //    {
                //        if (!Supplier.CustomerMaskAccount.Equals(TABS.CarrierAccount.SYSTEM))
                //            return Supplier.CustomerMaskAccount.CarrierProfile.BillingEmail;
                //        return Supplier.CarrierProfile.BillingEmail;
                //    }
                //    else
                //    {
                //        if (SupplierProfile.Accounts.Count() > 1 && !SupplierProfile.Accounts.First().CustomerMaskAccount.Equals(TABS.CarrierAccount.SYSTEM))
                //            return SupplierProfile.Accounts.First().CustomerMaskAccount.CarrierProfile.BillingEmail;
                //        return SupplierProfile.BillingEmail;
                //    }
                //}
            }
        }
        public string MaskedBillingEmail
        {
            get
            {
                if (IsCustomer)
                    return (Customer != null)
                                    ? ((Customer.CustomerMaskAccount != null)
                                                        ? Customer.CustomerMaskAccount.CarrierProfile.BillingEmail
                                                        : TABS.CarrierAccount.SYSTEM.CarrierProfile.BillingEmail)
                                    : ((CustomerProfile.Accounts.Count() > 1 && CustomerProfile.Accounts.First().CustomerMaskAccount != null)
                                                        ? CustomerProfile.Accounts.First().CustomerMaskAccount.CarrierProfile.BillingEmail
                                                        : TABS.CarrierAccount.SYSTEM.CarrierProfile.BillingEmail);
                return (Supplier != null)
                            ? ((Supplier.CustomerMaskAccount != null)
                                    ? Supplier.CustomerMaskAccount.CarrierProfile.BillingEmail
                                    : TABS.CarrierAccount.SYSTEM.CarrierProfile.BillingEmail)
                            : ((SupplierProfile.Accounts.Count() > 1 && SupplierProfile.Accounts.First().CustomerMaskAccount != null)
                                    ? SupplierProfile.Accounts.First().CustomerMaskAccount.CarrierProfile.BillingEmail
                                    : TABS.CarrierAccount.SYSTEM.CarrierProfile.BillingEmail);
            }
        }

        public string CarrierPricingEmail
        {
            get
            {
                return (IsCustomer)
                           ? (Customer != null)
                                   ? Customer.CarrierProfile.PricingEmail
                                   : CustomerProfile.PricingEmail
                           : (Supplier != null)
                                   ? Supplier.CarrierProfile.PricingEmail
                                   : SupplierProfile.PricingEmail;
            }
        }
        public string MaskedCarrierPricingEmail
        {
            get
            {
                if (IsCustomer)
                    return (Customer != null)
                                    ? ((Customer.CustomerMaskAccount != null)
                                                        ? Customer.CustomerMaskAccount.CarrierProfile.PricingEmail
                                                        : TABS.CarrierAccount.SYSTEM.CarrierProfile.PricingEmail)
                                    : ((CustomerProfile.Accounts.Count() > 1 && CustomerProfile.Accounts.First().CustomerMaskAccount != null)
                                                        ? CustomerProfile.Accounts.First().CustomerMaskAccount.CarrierProfile.PricingEmail
                                                        : TABS.CarrierAccount.SYSTEM.CarrierProfile.PricingEmail);
                return (Supplier != null)
                            ? ((Supplier.CustomerMaskAccount != null)
                                    ? Supplier.CustomerMaskAccount.CarrierProfile.PricingEmail
                                    : TABS.CarrierAccount.SYSTEM.CarrierProfile.PricingEmail)
                            : ((SupplierProfile.Accounts.Count() > 1 && SupplierProfile.Accounts.First().CustomerMaskAccount != null)
                                    ? SupplierProfile.Accounts.First().CustomerMaskAccount.CarrierProfile.PricingEmail
                                    : TABS.CarrierAccount.SYSTEM.CarrierProfile.PricingEmail);

                //if (IsCustomer)
                //{
                //    if (Customer != null)
                //    {
                //        if (!Customer.CustomerMaskAccount.Equals(TABS.CarrierAccount.SYSTEM))
                //            return Customer.CustomerMaskAccount.CarrierProfile.PricingEmail;
                //        return Customer.CarrierProfile.PricingEmail;
                //    }
                //    else
                //    {
                //        if (CustomerProfile.Accounts.Count() > 1 && !CustomerProfile.Accounts.First().CustomerMaskAccount.Equals(TABS.CarrierAccount.SYSTEM))
                //            return CustomerProfile.Accounts.First().CustomerMaskAccount.CarrierProfile.PricingEmail;
                //        return CustomerProfile.PricingEmail;
                //    }
                //}
                //else
                //{
                //    if (Supplier != null)
                //    {
                //        if (!Supplier.CustomerMaskAccount.Equals(TABS.CarrierAccount.SYSTEM))
                //            return Supplier.CustomerMaskAccount.CarrierProfile.PricingEmail;
                //        return Supplier.CarrierProfile.PricingEmail;
                //    }
                //    else
                //    {
                //        if (SupplierProfile.Accounts.Count() > 1 && !SupplierProfile.Accounts.First().CustomerMaskAccount.Equals(TABS.CarrierAccount.SYSTEM))
                //            return SupplierProfile.Accounts.First().CustomerMaskAccount.CarrierProfile.PricingEmail;
                //        return SupplierProfile.PricingEmail;
                //    }
                //}
            }
        }

        public virtual string RegistrationNumber
        {
            get
            {
                return (IsCustomer)
                           ? (Customer != null)
                                   ? Customer.CarrierProfile.RegistrationNumber
                                   : CustomerProfile.RegistrationNumber
                           : (Supplier != null)
                                   ? Supplier.CarrierProfile.RegistrationNumber
                                   : SupplierProfile.RegistrationNumber;
            }
        }
        public virtual string MaskedRegistrationNumber
        {
            get
            {
                if (IsCustomer)
                    return (Customer != null)
                                    ? ((Customer.CustomerMaskAccount != null)
                                                        ? Customer.CustomerMaskAccount.CarrierProfile.RegistrationNumber
                                                        : TABS.CarrierAccount.SYSTEM.CarrierProfile.RegistrationNumber)
                                    : ((CustomerProfile.Accounts.Count() > 1 && CustomerProfile.Accounts.First().CustomerMaskAccount != null)
                                                        ? CustomerProfile.Accounts.First().CustomerMaskAccount.CarrierProfile.RegistrationNumber
                                                        : TABS.CarrierAccount.SYSTEM.CarrierProfile.RegistrationNumber);
                return (Supplier != null)
                            ? ((Supplier.CustomerMaskAccount != null)
                                    ? Supplier.CustomerMaskAccount.CarrierProfile.RegistrationNumber
                                    : TABS.CarrierAccount.SYSTEM.CarrierProfile.RegistrationNumber)
                            : ((SupplierProfile.Accounts.Count() > 1 && SupplierProfile.Accounts.First().CustomerMaskAccount != null)
                                    ? SupplierProfile.Accounts.First().CustomerMaskAccount.CarrierProfile.RegistrationNumber
                                    : TABS.CarrierAccount.SYSTEM.CarrierProfile.RegistrationNumber);

                //if (IsCustomer)
                //{
                //    if (Customer != null)
                //    {
                //        if (!Customer.CustomerMaskAccount.Equals(TABS.CarrierAccount.SYSTEM))
                //            return Customer.CustomerMaskAccount.CarrierProfile.RegistrationNumber;
                //        return Customer.CarrierProfile.RegistrationNumber;
                //    }
                //    else
                //    {
                //        if (CustomerProfile.Accounts.Count() > 1 && !CustomerProfile.Accounts.First().CustomerMaskAccount.Equals(TABS.CarrierAccount.SYSTEM))
                //            return CustomerProfile.Accounts.First().CustomerMaskAccount.CarrierProfile.RegistrationNumber;
                //        return CustomerProfile.RegistrationNumber;
                //    }
                //}
                //else
                //{
                //    if (Supplier != null)
                //    {
                //        if (!Supplier.CustomerMaskAccount.Equals(TABS.CarrierAccount.SYSTEM))
                //            return Supplier.CustomerMaskAccount.CarrierProfile.RegistrationNumber;
                //        return Supplier.CarrierProfile.RegistrationNumber;
                //    }
                //    else
                //    {
                //        if (SupplierProfile.Accounts.Count() > 1 && !SupplierProfile.Accounts.First().CustomerMaskAccount.Equals(TABS.CarrierAccount.SYSTEM))
                //            return SupplierProfile.Accounts.First().CustomerMaskAccount.CarrierProfile.RegistrationNumber;
                //        return SupplierProfile.RegistrationNumber;
                //    }
                //}
            }
        }

        public string AccountManagerEmail
        {
            get
            {
                return (IsCustomer)
                          ? (Customer != null)
                                  ? Customer.CarrierProfile.AccountManagerEmail
                                  : CustomerProfile.AccountManagerEmail
                          : (Supplier != null)
                                  ? Supplier.CarrierProfile.AccountManagerEmail
                                  : SupplierProfile.AccountManagerEmail;
            }
        }
        public string MaskedAccountManagerEmail
        {
            get
            {
                if (IsCustomer)
                    return (Customer != null)
                                    ? ((Customer.CustomerMaskAccount != null)
                                                        ? Customer.CustomerMaskAccount.CarrierProfile.AccountManagerEmail
                                                        : TABS.CarrierAccount.SYSTEM.CarrierProfile.AccountManagerEmail)
                                    : ((CustomerProfile.Accounts.Count() > 1 && CustomerProfile.Accounts.First().CustomerMaskAccount != null)
                                                        ? CustomerProfile.Accounts.First().CustomerMaskAccount.CarrierProfile.AccountManagerEmail
                                                        : TABS.CarrierAccount.SYSTEM.CarrierProfile.AccountManagerEmail);
                return (Supplier != null)
                            ? ((Supplier.CustomerMaskAccount != null)
                                    ? Supplier.CustomerMaskAccount.CarrierProfile.AccountManagerEmail
                                    : TABS.CarrierAccount.SYSTEM.CarrierProfile.AccountManagerEmail)
                            : ((SupplierProfile.Accounts.Count() > 1 && SupplierProfile.Accounts.First().CustomerMaskAccount != null)
                                    ? SupplierProfile.Accounts.First().CustomerMaskAccount.CarrierProfile.AccountManagerEmail
                                    : TABS.CarrierAccount.SYSTEM.CarrierProfile.AccountManagerEmail);

                //if (IsCustomer)
                //{
                //    if (Customer != null)
                //    {
                //        if (!Customer.CustomerMaskAccount.Equals(TABS.CarrierAccount.SYSTEM))
                //            return Customer.CustomerMaskAccount.CarrierProfile.AccountManagerEmail;
                //        return Customer.CarrierProfile.AccountManagerEmail;
                //    }
                //    else
                //    {
                //        if (CustomerProfile.Accounts.Count() > 1 && !CustomerProfile.Accounts.First().CustomerMaskAccount.Equals(TABS.CarrierAccount.SYSTEM))
                //            return CustomerProfile.Accounts.First().CustomerMaskAccount.CarrierProfile.AccountManagerEmail;
                //        return CustomerProfile.AccountManagerEmail;
                //    }
                //}
                //else
                //{
                //    if (Supplier != null)
                //    {
                //        if (!Supplier.CustomerMaskAccount.Equals(TABS.CarrierAccount.SYSTEM))
                //            return Supplier.CustomerMaskAccount.CarrierProfile.AccountManagerEmail;
                //        return Supplier.CarrierProfile.AccountManagerEmail;
                //    }
                //    else
                //    {
                //        if (SupplierProfile.Accounts.Count() > 1 && !SupplierProfile.Accounts.First().CustomerMaskAccount.Equals(TABS.CarrierAccount.SYSTEM))
                //            return SupplierProfile.Accounts.First().CustomerMaskAccount.CarrierProfile.AccountManagerEmail;
                //        return SupplierProfile.AccountManagerEmail;
                //    }
                //}
            }
        }

        public virtual bool isActiveCarrier
        {
            get
            {
                return (IsCustomer)
                            ? ((Customer != null)
                                        ? Customer.isActive
                                        : CustomerProfile.isActive)
                            : ((Supplier != null)
                                    ? Supplier.isActive
                                    : SupplierProfile.isActive);
            }
        }

        //public virtual string AccountManagerEmail
        //{
        //    get
        //    {
        //        if (IsCustomer)
        //        {
        //            if (Customer != null)
        //                return Customer.CarrierProfile.AccountManagerEmail;
        //            return CustomerProfile.AccountManagerEmail;
        //        }
        //        else
        //        {
        //            if (Supplier != null)
        //                return Supplier.CarrierProfile.AccountManagerEmail;
        //            return SupplierProfile.AccountManagerEmail;
        //        }
        //    }
        //}

        public string SMSPhoneNumber
        {
            get
            {
                return (IsCustomer)
                          ? ((Customer != null)
                                      ? Customer.CarrierProfile.SMSPhoneNumber
                                      : CustomerProfile.SMSPhoneNumber)
                          : ((Supplier != null)
                                      ? Supplier.CarrierProfile.SMSPhoneNumber
                                      : SupplierProfile.SMSPhoneNumber);
            }
        }
        public int TotalCreditLimit
        {
            get
            {
                return (IsCustomer)
                          ? ((Customer != null)
                                      ? Customer.CustomerCreditLimit
                                      : CustomerProfile.CustomerCreditLimit)
                          : ((Supplier != null)
                                      ? Supplier.SupplierCreditLimit
                                      : SupplierProfile.SupplierCreditLimit);
            }
        }

        public decimal Balance { get; set; }
        public decimal Tolerance { get; set; }
        public decimal NetTolerance { get; set; }
        public virtual int _MailTemplateType { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("");
            sb.Append("{0} ({1}) - ");
            if (IsPrepaid) sb.Append(" Amount: {2} ");
            else sb.Append(" Percentage: {2}");
            sb.Append(" Actions {3}");
            return string.Format(sb.ToString(), CarrierName, IsCustomer ? "Customer" : "Supplier", (IsPrepaid) ? Amount : Percentage, Actions);
        }

        #region IComparable<PrepaidPostpaidOptions> Members
        public int CompareTo(PrepaidPostpaidOptions other)
        {
            // Not for same carrier (or profile)?
            int result = this.CarrierID.CompareTo(other.CarrierID);
            if (result != 0) return result;
            // Same carrier compare by amount
            result = (IsPrepaid) ? this.Amount.CompareTo(other.Amount) : this.Percentage.CompareTo(other.Percentage);
            return result;
        }
        #endregion

    }
}
