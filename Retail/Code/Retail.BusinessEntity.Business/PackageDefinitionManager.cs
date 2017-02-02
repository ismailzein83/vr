using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.Common;
using Vanrise.Entities;
using Retail.BusinessEntity.Data;

namespace Retail.BusinessEntity.Business
{
    public class PackageDefinitionManager
    {
        #region Public Methods

        public PackageDefinition GetPackageDefinitionById(Guid packageDefinitionId)
        {
            var packageDefinitions = GetCachedPackageDefinitionswithHidden();
            return packageDefinitions.FindRecord(x => x.VRComponentTypeId == packageDefinitionId);
        }

        public IEnumerable<PackageDefinitionInfo> GetPackageDefinitionsInfo(PackageDefinitionFilter filter)
        {
            IEnumerable<PackageDefinition> cachedPackageDefinitions = null;

            Func<PackageDefinition, bool> filterExpression = null;
            if (filter != null)
            {
                if (filter.IncludeHiddenPackageDefinitions)
                    cachedPackageDefinitions = this.GetCachedPackageDefinitionswithHidden();

                filterExpression = (packageDefinition) =>
                {
                    if (filter.AccountBEDefinitionId.HasValue && packageDefinition.Settings != null &&
                        filter.AccountBEDefinitionId.Value != packageDefinition.Settings.AccountBEDefinitionId)
                        return false;
                    return true;
                };
            }

            if (cachedPackageDefinitions == null)
                cachedPackageDefinitions = this.GetCachedPackageDefinitions();

            return cachedPackageDefinitions.MapRecords(PackageDefinitionInfoMapper, filterExpression).OrderBy(x => x.Name);
        }

        public IEnumerable<PackageDefinitionConfig> GetPackageDefinitionExtendedSettingsConfigs()
        {
            var templateConfigManager = new ExtensionConfigurationManager();
            return templateConfigManager.GetExtensionConfigurations<PackageDefinitionConfig>(PackageDefinitionConfig.EXTENSION_TYPE);
        }

        public Guid GetPackageDefinitionAccountBEDefId(Guid packageDefinitionId)
        {
            PackageDefinitionManager packageDefinitionManager = new PackageDefinitionManager();
            PackageDefinition packageDefinition = packageDefinitionManager.GetPackageDefinitionById(packageDefinitionId);
            if (packageDefinition == null)
                throw new NullReferenceException(string.Format("packageDefinition of packageDefinitionId: {0}", packageDefinitionId));

            if (packageDefinition.Settings == null)
                throw new NullReferenceException(string.Format("packageDefinition.Settings of packageDefinitionId: {0}", packageDefinitionId));

            return packageDefinition.Settings.AccountBEDefinitionId;
        }

        #region Private Methods

        public IEnumerable<PackageDefinition> GetCachedPackageDefinitionswithHidden()
        {
            VRComponentTypeManager vrComponentTypeManager = new Vanrise.Common.Business.VRComponentTypeManager();
            return vrComponentTypeManager.GetComponentTypes<PackageDefinitionSettings, PackageDefinition>();
        }

        private IEnumerable<PackageDefinition> GetCachedPackageDefinitions()
        {
            return new VRComponentTypeManager().GetCachedOrCreate("GetCachedProductDefinitions", () =>
            {
                VRRetailBEVisibilityManager retailBEVisibilityManager = new VRRetailBEVisibilityManager();
                Dictionary<Guid, VRRetailBEVisibilityAccountDefinitionPackageDefinition> visiblePackageDefinitionsById;
                List<PackageDefinition> includedProductDefinitions = new List<PackageDefinition>();
                
                var allPackageDefinitions = this.GetCachedPackageDefinitionswithHidden();

                if (retailBEVisibilityManager.ShouldApplyPackageDefinitionsVisibility(out visiblePackageDefinitionsById))
                {
                    foreach (var productDefinition in allPackageDefinitions)
                    {
                        if (visiblePackageDefinitionsById.ContainsKey(productDefinition.VRComponentTypeId))
                            includedProductDefinitions.Add(productDefinition);
                    }
                }
                else
                {
                    includedProductDefinitions = allPackageDefinitions.ToList();
                }

                return includedProductDefinitions;
            });
        }

        #endregion

        #endregion

        #region Mapper

        public PackageDefinitionInfo PackageDefinitionInfoMapper(PackageDefinition packageDefinition)
        {
            return new PackageDefinitionInfo
            {
                Name = packageDefinition.Name,
                PackageDefinitionId = packageDefinition.VRComponentTypeId,
                RuntimeEditor = packageDefinition.Settings.ExtendedSettings.RuntimeEditor
            };
        }

        #endregion
    }
}
