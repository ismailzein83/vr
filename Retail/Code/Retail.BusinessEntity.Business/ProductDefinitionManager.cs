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

        public ProductDefinition GetProductDefinition(Guid productDefinitionId)
        {
            var productDefinitions = GetProductDefinitions();
            return productDefinitions.FindRecord(x => x.VRComponentTypeId == productDefinitionId);
        }
        public ProductDefinitionSettings GetProductDefinitionSettings(Guid productDefinitionId)
        {
            var productDefinition = GetProductDefinition(productDefinitionId);

            if (productDefinition.Settings == null)
                throw new NullReferenceException(string.Format("productDefinition.Settings of productDefinitionId {0}", productDefinitionId));

            return productDefinition.Settings;
        }

        public IEnumerable<ProductDefinitionInfo> GetProductDefinitionsInfo()
        {
            var productDefinitions = GetProductDefinitions();
            return productDefinitions.MapRecords(ProductDefinitionInfoMapper);
        }

        public IEnumerable<ProductDefinitionConfig> GetProductDefinitionExtendedSettingsConfigs()
        {
            var templateConfigManager = new ExtensionConfigurationManager();
            return templateConfigManager.GetExtensionConfigurations<ProductDefinitionConfig>(ProductDefinitionConfig.EXTENSION_TYPE);
        }

        public Guid GetProductDefinitionAccountBEDefId(Guid productDefinitionId)
        {
            ProductDefinitionManager productDefinitionManager = new ProductDefinitionManager();
            ProductDefinition productDefinition = productDefinitionManager.GetProductDefinition(productDefinitionId);
            if (productDefinition == null)
                throw new NullReferenceException(string.Format("productDefinition of productDefinitionId: {0}", productDefinitionId));

            if (productDefinition.Settings == null)
                throw new NullReferenceException(string.Format("productDefinition.Settings of productDefinitionId: {0}", productDefinitionId));

            return productDefinition.Settings.AccountBEDefinitionId;
        }

        #endregion

        #region Mapper

        public ProductDefinitionInfo ProductDefinitionInfoMapper(ProductDefinition productDefinition)
        {
            return new ProductDefinitionInfo
            {
                Name = productDefinition.Name,
                ProductDefinitionId = productDefinition.VRComponentTypeId
            };
        }

        #endregion
    }
}
