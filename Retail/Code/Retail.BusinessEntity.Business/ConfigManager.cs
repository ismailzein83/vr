using Retail.BusinessEntity.Entities;
using System;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace Retail.BusinessEntity.Business
{
    public class ConfigManager
    {
        #region Public Methods

        public Guid GetAccountDIDRelationDefinitionId()
        {
            DIDTechnicalSettings didTechnicalSettings = this.GetDIDTechnicalSettings();
            Guid accountDIDRelationDefinitionId = didTechnicalSettings.AccountDIDRelationDefinitionId;
            if (accountDIDRelationDefinitionId == null)
                throw new NullReferenceException("didTechnicalSettings.AccountDIDRelationDefinitionId");

            return accountDIDRelationDefinitionId;
        }

        public VRTaxesDefinition GetRetailTaxesDefinitions()
        {
            RetailInvoiceSettings retailInvoiceSettings = this.GetRetailInvoiceSettings();
            if (retailInvoiceSettings == null)
                return null;

            return retailInvoiceSettings.VRTaxesDefinition;
        }

        public string GetFaultTicketsSerialNumberPattern()
        {
            FaultTicketsSettingsData faultTicketsSettingsData = GetFaultTicketsSettingsData();
            faultTicketsSettingsData.FaultTicketSetting.ThrowIfNull("faultTicketsSettingsData.FaultTicketSetting");
            faultTicketsSettingsData.FaultTicketSetting.SerialNumberPattern.ThrowIfNull("faultTicketsSettingsData.FaultTicketSetting.SerialNumberPattern");
            return faultTicketsSettingsData.FaultTicketSetting.SerialNumberPattern;
        }
        public long GetFaultTicketsSerialNumberInitialSequence()
        {
            FaultTicketsSettingsData faultTicketsSettingsData = GetFaultTicketsSettingsData();
            faultTicketsSettingsData.FaultTicketSetting.ThrowIfNull("faultTicketsSettingsData.FaultTicketSetting");
            return faultTicketsSettingsData.FaultTicketSetting.InitialSequence;
        }
        public Guid? GetFaultTicketsOpenMailTemplateId()
        {
            FaultTicketsSettingsData faultTicketsSettingsData = GetFaultTicketsSettingsData();
            faultTicketsSettingsData.FaultTicketSetting.ThrowIfNull("faultTicketsSettingsData.FaultTicketSetting");
            return faultTicketsSettingsData.FaultTicketSetting.OpenMailTemplateId;
        }
        public Guid? GetFaultTicketsPendingMailTemplateId()
        {
            FaultTicketsSettingsData faultTicketsSettingsData = GetFaultTicketsSettingsData();
            faultTicketsSettingsData.FaultTicketSetting.ThrowIfNull("faultTicketsSettingsData.FaultTicketSetting");
            return faultTicketsSettingsData.FaultTicketSetting.PendingMailTemplateId;
        }
        public Guid? GetFaultTicketsClosedMailTemplateId()
        {
            FaultTicketsSettingsData faultTicketsSettingsData = GetFaultTicketsSettingsData();
            faultTicketsSettingsData.FaultTicketSetting.ThrowIfNull("faultTicketsSettingsData.FaultTicketSetting");
            return faultTicketsSettingsData.FaultTicketSetting.ClosedMailTemplateId;
        }
        #endregion

        #region Private Methods

        private DIDTechnicalSettings GetDIDTechnicalSettings()
        {
            DIDTechnicalSettings didTechnicalSettings = new SettingManager().GetSetting<DIDTechnicalSettings>(DIDTechnicalSettings.SETTING_TYPE);
            if (didTechnicalSettings == null)
                throw new NullReferenceException("didTechnicalSettings");

            return didTechnicalSettings;
        }

        private RetailInvoiceSettings GetRetailInvoiceSettings()
        {
           return new SettingManager().GetSetting<RetailInvoiceSettings>(RetailInvoiceSettings.SETTING_TYPE);
        }
        private FaultTicketsSettingsData GetFaultTicketsSettingsData()
        {
            SettingManager manager = new SettingManager();
            FaultTicketsSettingsData faultTicketsSettingsData =
                manager.GetSetting<FaultTicketsSettingsData>(Constants.FaultTicketsSettingsData);
            if (faultTicketsSettingsData == null)
                throw new NullReferenceException("FaultTicketsSettings");
            return faultTicketsSettingsData;
        }

        #endregion
    }
}