using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.Common;
using Vanrise.Caching;
using Vanrise.GenericData.Data;
using Vanrise.Common.Business;
namespace Vanrise.GenericData.Business
{
    public class GenericRuleTypeConfigManager
    {
        #region Public Methods
        public IEnumerable<GenericRuleTypeConfig> GetGenericRuleTypes()
        {
            var extensionConfigurationManager = new ExtensionConfigurationManager();
            return extensionConfigurationManager.GetExtensionConfigurations<GenericRuleTypeConfig>(GenericRuleTypeConfig.EXTENSION_TYPE); 
        }

        public GenericRuleTypeConfig GetGenericRuleTypeByName(string ruleTypeName)
        {
            var cachedGenericRuleTypes = GetGenericRuleTypes();
            return cachedGenericRuleTypes.FindRecord(x=>x.Name == ruleTypeName);
        }

        public Guid GetGenericRuleTypeIdByName(string ruleTypeName)
        {
            var ruleTypeConfig = GetGenericRuleTypeByName(ruleTypeName);
            if (ruleTypeConfig == null)
                throw new NullReferenceException(String.Format("ruleTypeConfig. ruleTypeName '{0}'", ruleTypeName));
            return ruleTypeConfig.ExtensionConfigurationId;
        }

        public GenericRuleTypeConfig GetGenericRuleTypeById(Guid ruleTypeConfigId)
        {
            var cachedGenericRuleTypes = GetGenericRuleTypes();
            return cachedGenericRuleTypes.FindRecord(x=>x.ExtensionConfigurationId == ruleTypeConfigId);
        }

        #endregion
    }
}
