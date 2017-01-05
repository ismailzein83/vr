using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.Common;
 
namespace Retail.BusinessEntity.Business
{
    public class ProductDefinitionManager
    {
        #region Public Methods

        public IEnumerable<ProductDefinition> GetProductDefinitions()
        {
            VRComponentTypeManager vrComponentTypeManager = new Vanrise.Common.Business.VRComponentTypeManager();
            return vrComponentTypeManager.GetComponentTypes<ProductDefinitionSettings, ProductDefinition>();
        }

        public ProductDefinition GetPackageDefinitionById(Guid productDefinitionId)
        {
            var packageDefinitions = GetProductDefinitions();
            return packageDefinitions.FindRecord(x => x.VRComponentTypeId == productDefinitionId);
        }

        public IEnumerable<ProductDefinitionInfo> GetProductDefinitionsInfo()
        {
            var packageDefinitions = GetProductDefinitions();
            return packageDefinitions.MapRecords(ProductDefinitionInfoMapper);
        }

        public IEnumerable<ProductDefinitionConfig> GetProductDefinitionExtendedSettingsConfigs()
        {
            var templateConfigManager = new ExtensionConfigurationManager();
            return templateConfigManager.GetExtensionConfigurations<ProductDefinitionConfig>(ProductDefinitionConfig.EXTENSION_TYPE);
        }

        #endregion

        #region Mapper

        public ProductDefinitionInfo ProductDefinitionInfoMapper(ProductDefinition productDefinition)
        {
            return new ProductDefinitionInfo
            {
                Name = productDefinition.Name,
                ProductDefinitionId = productDefinition.VRComponentTypeId,
                RuntimeEditor = productDefinition.Settings.ExtendedSettings.RuntimeEditor
            };
        }

        #endregion
    }
}
