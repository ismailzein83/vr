using Retail.Voice.Entities;
using System;
using Vanrise.Common.Business;

namespace Retail.Voice.Business
{
    public class ConfigManager
    {
        public InternationalIdentification GetInternationalIdentification()
        {
            VoiceTechnicalSettings voiceTechnicalSettings = GetVoiceTechnicalSettings();
            if (voiceTechnicalSettings.InternationalIdentification == null)
                throw new NullReferenceException("voiceTechnicalSettings.InternationalIdentification");
            return voiceTechnicalSettings.InternationalIdentification;
        }

        public AccountIdentification GetAccountIdentification()
        {
            VoiceTechnicalSettings voiceTechnicalSettings = GetVoiceTechnicalSettings();
            if (voiceTechnicalSettings.AccountIdentification == null)
                throw new NullReferenceException("voiceTechnicalSettings.AccountIdentification");
            return voiceTechnicalSettings.AccountIdentification;
        }

        private VoiceTechnicalSettings GetVoiceTechnicalSettings()
        {
            SettingManager settingManager = new SettingManager();
            VoiceTechnicalSettings voiceTechnicalSettings = settingManager.GetSetting<VoiceTechnicalSettings>(VoiceTechnicalSettings.SETTING_TYPE);

            if (voiceTechnicalSettings == null)
                throw new NullReferenceException("voiceTechnicalSettings");

            return voiceTechnicalSettings;
        }
    }
}
