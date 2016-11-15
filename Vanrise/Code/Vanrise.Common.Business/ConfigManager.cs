using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Business
{
    public class ConfigManager
    {
        #region Public Methods
        public int GetSystemCurrencyId()
        {
            SettingManager settingManager = new SettingManager();
            CurrencySettingData systemCurrency = settingManager.GetSetting<CurrencySettingData>(Constants.BaseCurrencySettingType);

            if (systemCurrency == null)
                throw new NullReferenceException("systemCurrency");

            return systemCurrency.CurrencyId;
        }

        public EmailSettingData GetSystemEmail()
        {
            SettingManager settingManager = new SettingManager();
            var emailSettings = settingManager.GetSetting<EmailSettingData>(Constants.EmailSettingType);

            if (emailSettings == null)
                throw new NullReferenceException("emailSettings");

            return emailSettings;
        }

        public ProductInfo GetProductInfo()
        {
            SettingManager settingManager = new SettingManager();
            ProductInfoTechnicalSettings productInfoTechnicalSettings = settingManager.GetSetting<ProductInfoTechnicalSettings>(ProductInfoTechnicalSettings.SETTING_TYPE);

            ProductInfo productInfo = productInfoTechnicalSettings.ProductInfo;

            if (productInfo == null)
                throw new NullReferenceException("ProductInfoTechnicalSettings.ProductInfo NullReferenceException");

            return productInfo;
        }
        public IEnumerable<BankDetail> GetBankDetails()
        {
            SettingManager settingManager = new SettingManager();
            BankDetailsSettings bankDetailsSettings = settingManager.GetSetting<BankDetailsSettings>(BankDetailsSettings.SETTING_TYPE);
            IEnumerable<BankDetail> bankDetails = bankDetailsSettings.BankDetails;
            if (bankDetails == null)
                throw new NullReferenceException("BankDetailsSettings.BankDetails NullReferenceException");
            return bankDetails;
        }

        public IEnumerable<CompanySetting> GetCompanySetting()
        {
            SettingManager settingManager = new SettingManager();
            CompanySettings companySettings = settingManager.GetSetting<CompanySettings>(CompanySettings.SETTING_TYPE);
            IEnumerable<CompanySetting> settings = companySettings.Settings;
            if (settings == null)
                throw new NullReferenceException("companySettings.Settings NullReferenceException");
            return settings;
        }
        public CompanySetting GetDefaultCompanySetting()
        {

            IEnumerable<CompanySetting> settings = GetCompanySetting();

            foreach(var setting in settings)
            {
                if (setting.IsDefault)
                    return setting;
            }
            return null;
        }
        #endregion

    }
}
