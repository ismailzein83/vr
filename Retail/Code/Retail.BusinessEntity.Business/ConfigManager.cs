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
        #region public methods

        public AccountGridDefinition GetAccountGridDefinition()
        {
            RetailBETechnicalSetting retailBETechnicalSetting = GetRetailBETechnicalSetting();

            AccountGridDefinition accountGridDefinition = retailBETechnicalSetting.GridDefinition;
            if (accountGridDefinition == null)
                throw new NullReferenceException("accountGridDefinition");

            return accountGridDefinition;
        }

        public List<AccountViewDefinition> GetAccountViewDefinitions()
        {
            RetailBETechnicalSetting retailBETechnicalSetting = GetRetailBETechnicalSetting();

            List<AccountViewDefinition> accountViewDefinitions = retailBETechnicalSetting.AccountViewDefinitions;
            if (accountViewDefinitions == null)
                throw new NullReferenceException("accountViewDefinition");

            return accountViewDefinitions;
        }

        #endregion

        #region private methods

        private RetailBETechnicalSetting GetRetailBETechnicalSetting()
        {
            SettingManager settingManager = new SettingManager();
            RetailBETechnicalSetting retailBETechnicalSetting = settingManager.GetSetting<RetailBETechnicalSetting>(Constants.RetailBETechnicalSettings);

            if (retailBETechnicalSetting == null)
                throw new NullReferenceException("retailBETechnicalSetting");

            return retailBETechnicalSetting;
        }

        #endregion
    }
}
