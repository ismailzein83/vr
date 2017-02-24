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

        public ProductDefinition GetProductDefinition(Guid productDefinitionId)
        {
            var productDefinitions = GetCachedProductDefinitionsWithHidden();
            return productDefinitions.FindRecord(x => x.VRComponentTypeId == productDefinitionId);
        }
        public ProductDefinitionSettings GetProductDefinitionSettings(Guid productDefinitionId)
        {
            var productDefinition = GetProductDefinition(productDefinitionId);

            if (productDefinition.Settings == null)
                throw new NullReferenceException(string.Format("productDefinition.Settings of productDefinitionId {0}", productDefinitionId));

            return productDefinition.Settings;
        }

        public IEnumerable<ProductDefinitionInfo> GetProductDefinitionsInfo(ProductDefinitionFilter filter)
        {
            Dictionary<Guid, ProductDefinition> cachedProductDefinitions = null;

            Func<ProductDefinition, bool> filterExpression = null;
            if (filter != null)
            {
                if (filter.IncludeHiddenProductDefinitions)
                    cachedProductDefinitions = this.GetCachedProductDefinitionsWithHidden();

                filterExpression = (productDefinition) =>
                    {
                        if (filter.AccountBEDefinitionId.HasValue && productDefinition.Settings != null && 
                            filter.AccountBEDefinitionId.Value != productDefinition.Settings.AccountBEDefinitionId)
                            return false;
                        return true;
                    };
            }
            
            if (cachedProductDefinitions == null)
                cachedProductDefinitions = this.GetCachedProductDefinitions();

            return cachedProductDefinitions.MapRecords(ProductDefinitionInfoMapper, filterExpression).OrderBy(x => x.Name);
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

        #region Private Methods

        private Dictionary<Guid, ProductDefinition> GetCachedProductDefinitionsWithHidden()
        {
            VRComponentTypeManager vrComponentTypeManager = new Vanrise.Common.Business.VRComponentTypeManager();
            return vrComponentTypeManager.GetCachedComponentTypes<ProductDefinitionSettings, ProductDefinition>();
        }

        private Dictionary<Guid, ProductDefinition> GetCachedProductDefinitions()
        {
            return new VRComponentTypeManager().GetCachedOrCreate("GetProductDefinitions", () =>
            {
                var includedProductDefinitions = new Dictionary<Guid, ProductDefinition>();
                VRRetailBEVisibilityManager retailBEVisibilityManager = new VRRetailBEVisibilityManager();
                Dictionary<Guid, VRRetailBEVisibilityAccountDefinitionProductDefinition> visibleProductDefinitionsById;
                
                var allProductDefinitions = this.GetCachedProductDefinitionsWithHidden();

                if (retailBEVisibilityManager.ShouldApplyProductDefinitionsVisibility(out visibleProductDefinitionsById))
                {
                    foreach (var productDefinition in allProductDefinitions)
                    {
                        if (visibleProductDefinitionsById.ContainsKey(productDefinition.Key))
                            includedProductDefinitions.Add(productDefinition.Key, productDefinition.Value);
                    }
                }
                else
                {
                    includedProductDefinitions = allProductDefinitions;
                }

                return includedProductDefinitions;
            });
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
