using System;
using System.Collections.Generic;
using System.Linq;

namespace TABS
{
    [Serializable]
    public class AutoInvoiceSetting : IComparable<AutoInvoiceSetting>, Interfaces.ICachedCollectionContainer
    {

        protected string _SendEmail;
        protected string _CheckUnpricedCDRs;

        public virtual int SettingID { get; set; }
        public virtual string Description { get; set; }
        public virtual int Type { get; set; }
        public virtual DateTime StartDate { get; set; }
        public virtual int DayNumber { get; set; }
        public virtual bool CheckUnpricedCDRs
        {
            get { return "Y".Equals(_CheckUnpricedCDRs); }
            set { _CheckUnpricedCDRs = value ? "Y" : "N"; }
        }

        public AutoInvoiceSetting()
        {
            _Accounts = new List<CarrierAccount>();
        }
        public virtual bool SendEmail
        {
            get { return "Y".Equals(_SendEmail); }
            set { _SendEmail = value ? "Y" : "N"; }
        }

        private static Dictionary<int, AutoInvoiceSetting> _All;
        private List<CarrierAccount> _Accounts;
        private static string _lstCustomersName;

        public string lstCustomersName
        {
            get
            {
                _lstCustomersName = GetDelemetedCustomersName(this.Accounts.ToList(), ",");
                return _lstCustomersName;
            }
        }

        private static string GetDelemetedCustomersName(List<CarrierAccount> customers, string delimeter)
        {
            if (customers != null && customers.Count > 0)
                return customers.Select(k => k.Name).Aggregate((id1, id2) => String.Format("{0}{1}{2}", id1, delimeter, id2));
            return string.Empty;
        }

        public List<CarrierAccount> Accounts
        {
            get
            {
                if (_Accounts.Count > 0)
                    return _Accounts;
                return ObjectAssembler.GetAccountsofSetting(this);
            }
            set { _Accounts = value; }
        }

        public static Dictionary<int, AutoInvoiceSetting> All
        {
            get
            {
                lock (ObjectAssembler.SyncRoot)
                {
                    if (_All == null)
                        _All = ObjectAssembler.GetAutoInvoiceSetting();
                }
                return _All;
            }
        }

        public string AccountsCommaSep()
        {
            return string.Join(",", Accounts.Select(c => c.CarrierAccountID).ToArray());
        }

        public static void ClearCachedCollections()
        {
            _All = null;
            TABS.Components.CacheProvider.Clear(typeof(AutoInvoiceSetting).FullName);
        }


        public override string ToString()
        {
            string date = StartDate.ToString("yyyy-MM-ddTHH':'mm':'ss");
            string setting = String.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}", Description, Enum.GetName(typeof(AutoGenerateInvoice), Type), date, SendEmail, CheckUnpricedCDRs, DayNumber, SettingID);

            return setting;
        }

        #region IComparable<AutoInvoiceSetting> Members

        public int CompareTo(AutoInvoiceSetting other)
        {
            return Description.CompareTo(other.Description);
        }

        #endregion
    }
}
