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
    public class PackageDefinitionManager
    {
        public IEnumerable<PackageDefinitionConfig> GetPackageDefinitionExtendedSettingsConfigs()
        {
            var templateConfigManager = new ExtensionConfigurationManager();
            return templateConfigManager.GetExtensionConfigurations<PackageDefinitionConfig>(PackageDefinitionConfig.EXTENSION_TYPE);
        }


        public IEnumerable<PackageDefinitionInfo> GetPackageDefinitionsInfo()
        {
            VRComponentTypeManager vrComponentTypeManager = new Vanrise.Common.Business.VRComponentTypeManager();
            var packageDefinitions = vrComponentTypeManager.GetComponentTypes<PackageDefinitionSettings,PackageDefinition>();
            return packageDefinitions.MapRecords(PackageDefinitionInfoMapper);
        }



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
