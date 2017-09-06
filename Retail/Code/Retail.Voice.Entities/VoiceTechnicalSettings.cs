using System;
using Vanrise.Entities;

namespace Retail.Voice.Entities
{
    public enum InternationalNumberIdentification { NormalizedNumber = 0, NonNormalizedNumber = 1, } 

    public class VoiceTechnicalSettings : SettingData
    {
        public const string SETTING_TYPE = "Retail_Voice_VoiceTechnicalSettings";

        public AccountIdentification AccountIdentification { get; set; }

        public InternationalIdentification InternationalIdentification { get; set; }

        public ImportCDRSettings ImportCDRSettings { get; set; }
    }

    public abstract class AccountIdentification
    {
        public abstract Guid ConfigId { get; }

        public abstract void Execute(IAccountIdentificationContext context);
    }

    public abstract class InternationalIdentification
    {
        public abstract Guid ConfigId { get; }

        public InternationalNumberIdentification InternationalNumberIdentification { get; set; }

        public abstract void Execute(IInternationalIdentificationContext context);
    }

    public class ImportCDRSettings
    {
        public int? SaleAmountPrecision { get; set; }
    }
}