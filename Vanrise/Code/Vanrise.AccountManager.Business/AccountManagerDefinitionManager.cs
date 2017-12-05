using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.Business;
using Vanrise.Common.Business;
using Vanrise.AccountManager.Entities;
using Vanrise.Common;
using Vanrise.AccountManager.Data;
using Vanrise.Caching;

namespace Vanrise.AccountManager.Business
{
     
    public class AccountManagerDefinitionManager
    {
        #region Ctor
        static BusinessEntityDefinitionManager.CacheManager s_cacheManager = CacheManagerFactory.GetCacheManager<BusinessEntityDefinitionManager.CacheManager>();
        BaseCacheManager GetCacheManager()
        {
            return s_cacheManager;
        }
        #endregion
        
        #region Public Methods
        public BusinessEntityDefinition GetAccountManagerDefinition(Guid accountManagerDefinitionId)
       {
           BusinessEntityDefinitionManager manager = new BusinessEntityDefinitionManager();
           return manager.GetBusinessEntityDefinition(accountManagerDefinitionId);
       }
        public string GetAccountManagerDefinitionName(Guid accountManagerDefinitionId)
        {
            return GetAccountManagerDefinition(accountManagerDefinitionId).Name;
        }
        public Dictionary<Guid, AccountManagerAssignmentsInfo> GetAccountManagerAssignmentsInfo()
        {
            var accountManagerDefinitionSettings = GetAccountManagerDefinitionSettings();
            Dictionary<Guid, AccountManagerAssignmentsInfo> accountManagerAssignmnetsDefinitions = new Dictionary<Guid, AccountManagerAssignmentsInfo>();
            foreach(var accountManagerDefinitionSetting in accountManagerDefinitionSettings)
            {
                foreach (var accountManagerAssignmnetDefinition in accountManagerDefinitionSetting.Value.AssignmentDefinitions){
                AccountManagerAssignmentsInfo accountManagerAssignmentInfo = new AccountManagerAssignmentsInfo ();
                accountManagerAssignmentInfo.AssignmentDefinition = accountManagerAssignmnetDefinition;
                accountManagerAssignmentInfo.AccountManagerDefinitionId = accountManagerDefinitionSetting.Key;
                accountManagerAssignmnetsDefinitions.Add(accountManagerAssignmnetDefinition.AccountManagerAssignementDefinitionId, accountManagerAssignmentInfo);
                }
            }
            return accountManagerAssignmnetsDefinitions;
        }
        public AccountManagerAssignmentsInfo GetAccountManagerAssignmnetInfoByAssignmentDefinitionId(Guid accountManagerAssignmnetDefinitionId)
        {
            var accountManagerAssignmnetsInfo = GetAccountManagerAssignmentsInfo();
            return accountManagerAssignmnetsInfo.FirstOrDefault(x => x.Key == accountManagerAssignmnetDefinitionId).Value;
        }
        public Dictionary<Guid, AccountManagerBEDefinitionSettings> GetAccountManagerDefinitionSettings()
        {
            BusinessEntityDefinitionManager manager = new BusinessEntityDefinitionManager();
            var accountManagerDefinitions = manager.GetBusinessEntityDefinitionsByConfigId(AccountManagerBEDefinitionSettings.s_configId);
            Dictionary<Guid, AccountManagerBEDefinitionSettings> accountManagerBEDefinitionSettings = new Dictionary<Guid, AccountManagerBEDefinitionSettings>();
            foreach (var accountManagerDefinition in accountManagerDefinitions)
            {
                var accountManagerDefinitionSettings = accountManagerDefinition.Value.Settings as AccountManagerBEDefinitionSettings;
                
              if(  accountManagerDefinitionSettings != null)
                accountManagerBEDefinitionSettings.Add(accountManagerDefinition.Key, accountManagerDefinitionSettings);
            }
            return accountManagerBEDefinitionSettings;
        }
       public AccountManagerBEDefinitionSettings GetAccountManagerDefinitionSettings(Guid accountManagerDefinitionId)
       {
           var accountManagerDefinition = GetAccountManagerDefinition(accountManagerDefinitionId);
           accountManagerDefinition.ThrowIfNull("accountManagerDefinition", accountManagerDefinitionId);
           accountManagerDefinition.Settings.ThrowIfNull("accountManagerDefinition.Settings");
           return accountManagerDefinition.Settings.CastWithValidate<AccountManagerBEDefinitionSettings>("accountManagerDefinition.Settings");
       }
       public IEnumerable<AccountManagerSubViewDefinition> GetAccountManagerSubViewsDefinition(Guid accountManagerDefinitionId)
       {
           var accountManagerDefinitionSettings = GetAccountManagerDefinitionSettings(accountManagerDefinitionId);
           return accountManagerDefinitionSettings.SubViews;
       }
       public AccountManagerAssignmentDefinition GetAccountManagerAssignmentDefinition(Guid accountManagerDefinitionId, Guid accountManagerAssignementDefinitionId)
       {
           var accountManagerDefinitionSettings = GetAccountManagerDefinitionSettings(accountManagerDefinitionId);
           if(accountManagerDefinitionSettings.AssignmentDefinitions == null)
               return null;
           return accountManagerDefinitionSettings.AssignmentDefinitions.FindRecord(x=>x.AccountManagerAssignementDefinitionId == accountManagerAssignementDefinitionId);
       }
       public IEnumerable<AccountManagerAssignmentConfig> GetAssignmentDefinitionConfigs()
       {
           ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
           return manager.GetExtensionConfigurations<AccountManagerAssignmentConfig>(AccountManagerAssignmentConfig.EXTENSION_TYPE).OrderByDescending(x => x.Name);
       }
       public IEnumerable<AccountManagerSubViewsConfig> GetSubViewsDefinitionConfigs()
       {
           ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
           return manager.GetExtensionConfigurations<AccountManagerSubViewsConfig>(AccountManagerSubViewsConfig.EXTENSION_TYPE).OrderByDescending(x => x.Name);
       }

       public T GetCachedOrCreate<T>(Object cacheName, Func<T> createObject)
       {
           return GetCacheManager().GetOrCreateObject(cacheName, createObject);
       }
        #endregion
    }
  
}
