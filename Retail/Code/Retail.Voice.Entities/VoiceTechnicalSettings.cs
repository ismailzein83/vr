using System;
using Vanrise.Entities;

namespace Retail.Voice.Entities
{
    public class VoiceTechnicalSettings : SettingData
    {
        public const string SETTING_TYPE = "Retail_Voice_VoiceTechnicalSettings";

        public AccountIdentification AccountIdentification { get; set; }

        public InternationalIdentification InternationalIdentification { get; set; }
    }

    public abstract class AccountIdentification
    {
        public abstract Guid ConfigId { get; }

        public abstract void Execute(IAccountIdentificationContext context);
    }

    public abstract class InternationalIdentification
    {
        public abstract Guid ConfigId { get; }

        public abstract void Execute(IInternationalIdentificationContext context);
    }
}