using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;

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

        public List<Guid> GetIncludedAccountTypeIds()
        {
            IncludedAccountTypes includedAccountTypes = this.GetIncludedAccountTypes();
            List<Guid> includedAccountTypeIds = includedAccountTypes.AcountTypeIds;
            if (includedAccountTypeIds == null)
                throw new NullReferenceException("includedAccountTypeIds");

            return includedAccountTypeIds;
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

        private RetailBETechnicalSettings GetRetailBETechnicalSettings()
        {
            RetailBETechnicalSettings retailBETechnicalSettings = new SettingManager().GetSetting<RetailBETechnicalSettings>(RetailBETechnicalSettings.SETTING_TYPE);
            if (retailBETechnicalSettings == null)
                throw new NullReferenceException("didTechnicalSettings");

            return retailBETechnicalSettings;
        }

        private IncludedAccountTypes GetIncludedAccountTypes()
        {
            RetailBETechnicalSettings retailBETechnicalSettings = this.GetRetailBETechnicalSettings();
            IncludedAccountTypes includedAccountTypes = retailBETechnicalSettings.IncludedAccountTypes;
            if (includedAccountTypes == null)
                throw new NullReferenceException("includedAccountTypes");

            return includedAccountTypes;
        }

        #endregion
    }
}
