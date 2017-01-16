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

    public interface IAccountIdentificationContext
    {
        dynamic RawCDR { get; }
        string CallingNumber { get; }
        string CalledNumber { get; }
        long? CallingAccountId { set; }
        long? CalledAccountId { set; }
    }

    public class AccountIdentificationContext : IAccountIdentificationContext
    {
        public dynamic RawCDR { get; set; }
        public string CallingNumber { get; set; }
        public string CalledNumber { get; set; }
        public long? CallingAccountId { get; set; }
        public long? CalledAccountId { get; set; }
    }

    public interface IInternationalIdentificationContext
    {
    }
}