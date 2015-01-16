using System;
using System.Collections.Generic;

namespace TABS
{
    public class AutoInvoiceLogger : Interfaces.ICachedCollectionContainer
    {
        public AutoInvoiceSetting Setting
        {
            get
            {
                if (TABS.AutoInvoiceSetting.All.ContainsKey(SettingID))
                    return TABS.AutoInvoiceSetting.All[SettingID];
                return null;
            }
        }
        public virtual int LogID { get; set; }
        public virtual int BatchID { get; set; }
        public virtual string CustomerID { get; set; }
        public virtual int SettingID { get; set; }
        public virtual char IsCDRConflict { get; set; }
        public bool Generated { get { return IsGenerated == 'Y' ? true : false; } }
        public bool Sent { get { return IsSent == 'Y' ? true : false; } }
        public virtual char IsGenerated { get; set; }
        public virtual char IsSent { get; set; }
        public virtual string Note { get; set; }
        public virtual int Type { get; set; }
        public virtual DateTime? GeneratedDate { get; set; }
        public virtual int InvoiceID { get; set; }
        public CarrierAccount Customer
        {
            get
            {
                if (TABS.CarrierAccount.All.ContainsKey(CustomerID))
                    return TABS.CarrierAccount.All[CustomerID];
                return null;
            }
        }
        public Billing_Invoice Invoice { get { return ObjectAssembler.GetBillingInvoice(InvoiceID); } }
        internal static List<AutoInvoiceLogger> _All;



        public static List<AutoInvoiceLogger> All
        {
            get
            {
                lock (ObjectAssembler.SyncRoot)
                {
                    if (_All == null)
                        _All = ObjectAssembler.GetAutoInvoiceLogger();
                }
                return _All;
            }
        }



        public static void ClearCachedCollections()
        {
            _All = null;
            TABS.Components.CacheProvider.Clear(typeof(AutoInvoiceLogger).FullName);
        }

    }
}
