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
            Dictionary<Guid, PackageDefinition> cachedPackageDefinitions = null;

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

        #endregion

        #region Private Methods

        public Dictionary<Guid, PackageDefinition> GetCachedPackageDefinitionswithHidden()
        {
            VRComponentTypeManager vrComponentTypeManager = new Vanrise.Common.Business.VRComponentTypeManager();
            return vrComponentTypeManager.GetCachedComponentTypes<PackageDefinitionSettings, PackageDefinition>();
        }

        private Dictionary<Guid, PackageDefinition> GetCachedPackageDefinitions()
        {
            return new VRComponentTypeManager().GetCachedOrCreate("GetCachedProductDefinitions", () =>
            {
                var includedProductDefinitions = new Dictionary<Guid, PackageDefinition>();
                VRRetailBEVisibilityManager retailBEVisibilityManager = new VRRetailBEVisibilityManager();
                Dictionary<Guid, VRRetailBEVisibilityAccountDefinitionPackageDefinition> visiblePackageDefinitionsById;

                var allPackageDefinitions = this.GetCachedPackageDefinitionswithHidden();

                if (retailBEVisibilityManager.ShouldApplyPackageDefinitionsVisibility(out visiblePackageDefinitionsById))
                {
                    foreach (var packageDefinition in allPackageDefinitions)
                    {
                        if (visiblePackageDefinitionsById.ContainsKey(packageDefinition.Key))
                            includedProductDefinitions.Add(packageDefinition.Key, packageDefinition.Value);
                    }
                }
                else
                {
                    includedProductDefinitions = allPackageDefinitions;
                }

                return includedProductDefinitions;
            });
        }

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
