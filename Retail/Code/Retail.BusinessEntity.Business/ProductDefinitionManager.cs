using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Security.Business;

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
            productDefinition.ThrowIfNull(string.Format("productDefinition of ProductDefinitionId: {0}", productDefinitionId));
            productDefinition.Settings.ThrowIfNull(string.Format("productDefinition.Settings of ProductDefinitionId: {0}", productDefinitionId));
            return productDefinition.Settings;
        }

        public string GetProductDefinitionName(Guid productDefinitionId)
        {
            var productDefinition = GetProductDefinition(productDefinitionId);
            return productDefinition != null ? productDefinition.Name : null;
        }

        public Guid GetProductDefinitionAccountBEDefinitionId(Guid productDefinitionId)
        {
            ProductDefinitionSettings productDefinitionSettings = GetProductDefinitionSettings(productDefinitionId);
            return productDefinitionSettings.AccountBEDefinitionId;
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

                        if (filter.Filters != null && !CheckIfFilterIsMatch(productDefinition, filter.Filters))
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

        internal Dictionary<Guid, ProductDefinition> GetCachedProductDefinitionsWithHidden()
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

        private bool CheckIfFilterIsMatch(ProductDefinition productDefinition, List<IProductDefinitionFilter> filters)
        {
            var context = new ProductDefinitionFilterContext { ProductDefinitionId = productDefinition.VRComponentTypeId };
            foreach (var filter in filters)
            {
                if (!filter.IsMatched(context))
                    return false;
            }
            return true;
        }

        #endregion

        #region Security

        public bool DoesUserHaveViewProductDefinitions(int userId)
        {
            return GetViewAllowedProductDefinitions(userId).Count > 0;
        }

        public bool DoesUserHaveViewProductDefinitions()
        {
            int userId = SecurityContext.Current.GetLoggedInUserId();
            return GetViewAllowedProductDefinitions(userId).Count > 0;
        }

        public HashSet<Guid> GetViewAllowedProductDefinitions()
        {
            int userId = SecurityContext.Current.GetLoggedInUserId();
            return GetViewAllowedProductDefinitions(userId);
        }

        public HashSet<Guid> GetViewAllowedProductDefinitions(int userId)
        {
            HashSet<Guid> ids = new HashSet<Guid>();
            var allProducts = this.GetCachedProductDefinitions();
            foreach (var p in allProducts)
            {
                if (DoesUserHaveViewProductDefinition(p.Key, userId))
                    ids.Add(p.Key);
            }
            return ids;
        }

        public bool DoesUserHaveViewProductDefinition(Guid productDefinitionId, int userId)
        {
            return DoesUserHaveAccessToProductDef(productDefinitionId, userId, new AccountBEDefinitionManager().DoesUserHaveViewProductAccess);
        }

        public bool DoesUserHaveAddProductDefinitions()
        {
            return GetdAddAllowedProductDefinitions().Count > 0;
        }

        public HashSet<Guid> GetdAddAllowedProductDefinitions()
        {
            HashSet<Guid> ids = new HashSet<Guid>();
            int userId = SecurityContext.Current.GetLoggedInUserId();
            var allProducts = this.GetCachedProductDefinitions();
            foreach (var p in allProducts)
            {
                if (DoesUserHaveAddProductDefinitions(p.Key, userId))
                    ids.Add(p.Key);
            }
            return ids;
        }

        public bool DoesUserHaveAddProductDefinitions(Guid productDefinitionId)
        {
            int userId = SecurityContext.Current.GetLoggedInUserId();
            return DoesUserHaveAddProductDefinitions(productDefinitionId, userId);
        }

        public bool DoesUserHaveAddProductDefinitions(Guid productDefinitionId, int userId)
        {
            return DoesUserHaveAccessToProductDef(productDefinitionId, userId, new AccountBEDefinitionManager().DoesUserHaveAddProductAccess);
        }

        public bool DoesUserHaveEditProductDefinitions(Guid productDefinitionId)
        {
            int userId = SecurityContext.Current.GetLoggedInUserId();
            return DoesUserHaveEditProductDefinitions(productDefinitionId, userId);
        }

        public bool DoesUserHaveEditProductDefinitions(Guid productDefinitionId, int userId)
        {
            return DoesUserHaveAccessToProductDef(productDefinitionId, userId, new AccountBEDefinitionManager().DoesUserHaveEditProductAccess);
        }

        public bool DoesUserHaveAccessToProductDef(Guid productDefinitionId, int userId, Func<int, Guid, bool> doesUserHaveProductAccessOnAccDef)
        {
            var product = GetProductDefinition(productDefinitionId);
            if (product != null && product.Settings != null && product.Settings.AccountBEDefinitionId != null)
                return doesUserHaveProductAccessOnAccDef(userId, product.Settings.AccountBEDefinitionId);
            return true;
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
