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

        public IEnumerable<PackageDefinition> GetPackageDefinitions()
        {
            VRComponentTypeManager vrComponentTypeManager = new Vanrise.Common.Business.VRComponentTypeManager();
            return vrComponentTypeManager.GetComponentTypes<PackageDefinitionSettings, PackageDefinition>();
        }

        public IEnumerable<PackageDefinitionInfo> GetPackageDefinitionsInfo()
        {
            var packageDefinitions = GetPackageDefinitions();
            return packageDefinitions.MapRecords(PackageDefinitionInfoMapper);
        }

        public PackageDefinition GetPackageDefinitionById(Guid packageDefinitionId)
        {
            var packageDefinitions = GetPackageDefinitions();
            return packageDefinitions.FindRecord(x => x.VRComponentTypeId == packageDefinitionId);
        }

        public IEnumerable<PackageDefinitionConfig> GetPackageDefinitionExtendedSettingsConfigs()
        {
            var templateConfigManager = new ExtensionConfigurationManager();
            return templateConfigManager.GetExtensionConfigurations<PackageDefinitionConfig>(PackageDefinitionConfig.EXTENSION_TYPE);
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
