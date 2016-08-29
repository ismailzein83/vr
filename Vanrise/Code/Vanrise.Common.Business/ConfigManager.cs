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

        public string GetProductName()
        {
            SettingManager settingManager = new SettingManager();
            ProductInfoTechnicalSettings productInfoTechnicalSettings = settingManager.GetSetting<ProductInfoTechnicalSettings>(ProductInfoTechnicalSettings.SETTING_TYPE);

            string productName = productInfoTechnicalSettings.ProductName;

            if (productName == null)
                throw new NullReferenceException("ProductInfoTechnicalSettings.ProductName NullReferenceException");

            return productName;
        }
        public string GetProductVersionNumber()
        {
            SettingManager settingManager = new SettingManager();
            ProductInfoTechnicalSettings productInfoTechnicalSettings = settingManager.GetSetting<ProductInfoTechnicalSettings>(ProductInfoTechnicalSettings.SETTING_TYPE);

            string versionNumber = productInfoTechnicalSettings.VersionNumber;

            if (versionNumber == null)
                throw new NullReferenceException("ProductInfoTechnicalSettings.VersionNumber NullReferenceException");

            return versionNumber;
        }
        #endregion

    }
}
