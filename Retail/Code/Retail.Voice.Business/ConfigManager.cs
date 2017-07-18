using Retail.Voice.Entities;
using System;
using Vanrise.Common.Business;
using Vanrise.Common;

namespace Retail.Voice.Business
{
    public class ConfigManager
    {
        #region Public Methods

        public InternationalIdentification GetInternationalIdentification()
        {
            VoiceTechnicalSettings voiceTechnicalSettings = GetVoiceTechnicalSettings();
            voiceTechnicalSettings.InternationalIdentification.ThrowIfNull("voiceTechnicalSettings.InternationalIdentification");
            return voiceTechnicalSettings.InternationalIdentification;
        }

        public AccountIdentification GetAccountIdentification()
        {
            VoiceTechnicalSettings voiceTechnicalSettings = GetVoiceTechnicalSettings();
            voiceTechnicalSettings.AccountIdentification.ThrowIfNull("voiceTechnicalSettings.AccountIdentification");
            return voiceTechnicalSettings.AccountIdentification;
        }

        public int? GetSaleAmountPrecision()
        {
            ImportCDRSettings importCDRSettings = GetImportCDRSettings();
            return importCDRSettings.SaleAmountPrecision;
        }

        #endregion
        
        #region Private Methods

        private VoiceTechnicalSettings GetVoiceTechnicalSettings()
        {
            SettingManager settingManager = new SettingManager();
            VoiceTechnicalSettings voiceTechnicalSettings = settingManager.GetSetting<VoiceTechnicalSettings>(VoiceTechnicalSettings.SETTING_TYPE);
            voiceTechnicalSettings.ThrowIfNull("voiceTechnicalSettings");
            return voiceTechnicalSettings;
        }

        private ImportCDRSettings GetImportCDRSettings()
        {
            VoiceTechnicalSettings voiceTechnicalSettings = GetVoiceTechnicalSettings();
            voiceTechnicalSettings.ImportCDRSettings.ThrowIfNull("voiceTechnicalSettings.ImportCDRSettings");
            return voiceTechnicalSettings.ImportCDRSettings;
        }

        #endregion
    }
}
