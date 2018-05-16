using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Invoice.Entities;
using Vanrise.Common.Business;
using Vanrise.Common;

namespace TOne.WhS.Invoice.Business
{
    public class ConfigManager
    {
        #region Public Methods
        public Dictionary<Guid, InvoiceTypeSetting> GetInvoiceTypeSettings()
        { 
            SettingManager settingManager = new SettingManager();
            InvoiceSettings invoiceSettings = settingManager.GetSetting<InvoiceSettings>(InvoiceSettings.SETTING_TYPE);

            if (invoiceSettings == null)
                return null;

            return invoiceSettings.InvoiceTypeSettings;
        }

        public InvoiceTypeSetting GetInvoiceTypeSettingsById(Guid invoiceTypeId)
        {
            var invoiceTypeSettings = GetInvoiceTypeSettings();
            if (invoiceTypeSettings != null && invoiceTypeSettings.Count != 0)
            {
                return invoiceTypeSettings.GetRecord(invoiceTypeId);
            }
            return null;
        }

        //public Guid GetDefaultCustomerInvoiceTemplateMessageId()
        //{
        //    CustomerInvoiceSettings customerInvoiceSettings = GetDefaultCustomerInvoiceSettings();
        //    if (customerInvoiceSettings == null)
        //        throw new NullReferenceException("defaultCustomerInvoice");
        //    return customerInvoiceSettings.DefaultEmailId;
        //}
        //#endregion
     
        //#region Private Methods
        //private CustomerInvoiceSettings GetDefaultCustomerInvoiceSettings()
        //{
        //    InvoiceSettings setting = GetInvoiceSettings();
        //    if (setting.CustomerInvoiceSettings == null)
        //        throw new NullReferenceException("setting.CustomerInvoiceSettings");
        //    foreach (var customerInvoiceSettings in setting.CustomerInvoiceSettings)
        //    {
        //        if (customerInvoiceSettings.IsDefault)
        //            return customerInvoiceSettings;
        //    }
        //    throw new NullReferenceException("setting.CustomerInvoiceSettings");
        //}
        //private InvoiceSettings GetInvoiceSettings()
        //{
        //    SettingManager settingManager = new SettingManager();
        //    InvoiceSettings invoiceSettings = settingManager.GetSetting<InvoiceSettings>(InvoiceSettings.SETTING_TYPE);

        //    if (invoiceSettings == null)
        //        throw new NullReferenceException("invoiceSettings");

        //    return invoiceSettings;
        //}

        #endregion

    }
}
