using Retail.Voice.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;

namespace Retail.Voice.Business
{
    public class ConfigManager
    {
        #region Public Methods

        public AccountIdentification GetAccountIdentification()
        {
            VoiceTechnicalSettings voiceTechnicalSettings = GetVoiceTechnicalSettings();
            voiceTechnicalSettings.AccountIdentification.ThrowIfNull("voiceTechnicalSettings.AccountIdentification");
            return voiceTechnicalSettings.AccountIdentification;
        }

        public InternationalIdentification GetInternationalIdentification()
        {
            VoiceTechnicalSettings voiceTechnicalSettings = GetVoiceTechnicalSettings();
            voiceTechnicalSettings.InternationalIdentification.ThrowIfNull("voiceTechnicalSettings.InternationalIdentification");
            return voiceTechnicalSettings.InternationalIdentification;
        }

        public InternationalNumberIdentification GetInternationalNumberIdentification()
        {
            InternationalIdentification internationalIdentification = GetInternationalIdentification();
            return internationalIdentification.InternationalNumberIdentification;
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
