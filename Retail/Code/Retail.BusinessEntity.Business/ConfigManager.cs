﻿using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.Common;
using Vanrise.Entities;

namespace Retail.BusinessEntity.Business
{
    public class ConfigManager
    {
        #region Public Methods

        public Guid GetAccountDIDRelationDefinitionId()
        {
            DIDTechnicalSettings DIDTechnicalSettings = this.GetDIDTechnicalSettings();
            Guid accountDIDRelationDefinitionId = DIDTechnicalSettings.AccountDIDRelationDefinitionId;
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
        #endregion
    }
}
