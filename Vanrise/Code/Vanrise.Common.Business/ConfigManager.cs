﻿using System;
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

        public GeneralSettingData GetGeneralSetting()
        {
            SettingManager settingManager = new SettingManager();
            var generalSettings = settingManager.GetSetting<GeneralSettingData>(Constants.GeneralSettingType);

         
            return generalSettings;
        }

        public GeneralTechnicalSettingData GetGeneralTechnicalSetting()
        {
            SettingManager settingManager = new SettingManager();
            var generalSettings = settingManager.GetSetting<GeneralTechnicalSettingData>(Constants.GeneralTechnicalSettingType);
            return generalSettings;
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
            if (bankDetailsSettings == null)
                return null;
            if (bankDetailsSettings.BankDetails == null )
                return null;
            IEnumerable<BankDetail> bankDetails = bankDetailsSettings.BankDetails;
            return bankDetails;
        }

        public IEnumerable<BankDetailsSettingsInfo> GetBankDetailsInfo()
        {
            IEnumerable<BankDetail> bankDetails = GetBankDetails();
            if (bankDetails == null)
                return null;

            List<BankDetailsSettingsInfo> lstBankDetailsInfo = new List<BankDetailsSettingsInfo>();
            foreach (var bankDetail in bankDetails)
            {
                BankDetailsSettingsInfo bankDetailsSettingsInfo = new BankDetailsSettingsInfo();
                bankDetailsSettingsInfo = BankDetailInfoMapper(bankDetail);
                lstBankDetailsInfo.Add(bankDetailsSettingsInfo);
            }

            if (lstBankDetailsInfo == null)
                return null;

            return lstBankDetailsInfo;
        }


        public Vanrise.Entities.InsertOperationOutput<BankDetail> AddBank(BankDetail BankDetail)
        {
            Vanrise.Entities.InsertOperationOutput<BankDetail> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<BankDetail>();
            SettingManager settingManager = new SettingManager();
            BankDetailsSettings bankDetailsSettings = settingManager.GetSetting<BankDetailsSettings>(BankDetailsSettings.SETTING_TYPE);

            SettingToEdit bankSettingsToEdit = new SettingToEdit(){
                Name = "Bank Details",
                SettingId = new Guid("1cb20f2c-a835-4320-aec7-e034c5a756e9")
            };
            if (bankDetailsSettings == null)
                bankDetailsSettings = new BankDetailsSettings();
            if (bankDetailsSettings.BankDetails == null)
                bankDetailsSettings.BankDetails =  new List<BankDetail>();

            bankDetailsSettings.BankDetails.Add(BankDetail);
            bankSettingsToEdit.Data = bankDetailsSettings;
            settingManager.UpdateSetting(bankSettingsToEdit);

            insertOperationOutput.InsertedObject = BankDetail;
            insertOperationOutput.Result = InsertOperationResult.Succeeded;
            return insertOperationOutput;
        }

        public IEnumerable<CompanySettingsInfo> GetCompanySettingsInfo()
        {
            IEnumerable<CompanySetting> companySettings = GetCompanySetting();
            List<CompanySettingsInfo> lstCompanySettingsInfo = new List<CompanySettingsInfo>();
            foreach (var company in companySettings)
            {
                CompanySettingsInfo companySettingsInfo = new CompanySettingsInfo();
                companySettingsInfo = CompanySettingInfoMapper(company);
                lstCompanySettingsInfo.Add(companySettingsInfo);
            }

            return lstCompanySettingsInfo;
        }

        public string GetDefaultCompanyName()
        {
            string name = null;
            IEnumerable<CompanySetting> companySettings = GetCompanySetting();
            List<CompanySettingsInfo> lstCompanySettingsInfo = new List<CompanySettingsInfo>();
            var company =  companySettings.FindRecord(c=>c.IsDefault);
            if (company != null)
                name = company.CompanyName;
            return name;           
        }

        public string GetProductVersionNumber()
        {
            return GetProductInfo().VersionNumber;
        }

        public List<CompanyContactType> GetCompanyContactTypes()
        {
            var companyContactTypes = new List<CompanyContactType>();
            var generalTechnicalSettingData = GetGeneralTechnicalSetting();
            if (generalTechnicalSettingData != null && generalTechnicalSettingData.CompanySettingDefinition != null)
                 companyContactTypes =  generalTechnicalSettingData.CompanySettingDefinition.ContactTypes;
            return companyContactTypes;
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
        public CompanySetting GetCompanySettingById(Guid companySettingId)
        {
            IEnumerable<CompanySetting> settings = GetCompanySetting();

            foreach (var setting in settings)
            {
                if (setting.CompanySettingId == companySettingId)
                    return setting;
            }
            return GetDefaultCompanySetting();
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


        #region Mappers

        private BankDetailsSettingsInfo BankDetailInfoMapper(BankDetail bankDetail)
        {
            return new BankDetailsSettingsInfo()
            {
                BankDetailId = bankDetail.BankDetailId,
                Bank = bankDetail.Bank,
            };
        }

        private CompanySettingsInfo CompanySettingInfoMapper(CompanySetting company)
        {
            return new CompanySettingsInfo()
            {
                CompanySettingId = company.CompanySettingId,
                CompanyName = company.CompanyName,
            };
        }
        #endregion

    }
}
