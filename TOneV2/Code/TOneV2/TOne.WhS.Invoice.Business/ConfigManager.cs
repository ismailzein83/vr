using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Invoice.Entities;
using Vanrise.Common.Business;

namespace TOne.WhS.Invoice.Business
{
    public class ConfigManager
    {
        #region Public Methods
        public Guid GetDefaultCustomerInvoiceTemplateMessageId()
        {
            InvoiceSettings setting = GetInvoiceSettings();
            if (setting.CustomerInvoiceSettings == null)
                throw new NullReferenceException("setting.CustomerInvoiceSettings");
            return setting.CustomerInvoiceSettings.DefaultEmailId;
        }
        #endregion
     
        #region Private Methods
        private InvoiceSettings GetInvoiceSettings()
        {
            SettingManager settingManager = new SettingManager();
            InvoiceSettings invoiceSettings = settingManager.GetSetting<InvoiceSettings>(InvoiceSettings.SETTING_TYPE);

            if (invoiceSettings == null)
                throw new NullReferenceException("invoiceSettings");

            return invoiceSettings;
        }

        #endregion

    }
}
