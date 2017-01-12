using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Common;

namespace Vanrise.GenericData.Business
{
    public class BEParentChildRelationDefinitionManager
    {
        #region Public Methods

        public IEnumerable<BEParentChildRelationDefinition> GetBEParentChildRelationDefinitions()
        {
            VRComponentTypeManager vrComponentTypeManager = new Vanrise.Common.Business.VRComponentTypeManager();
            return vrComponentTypeManager.GetComponentTypes<BEParentChildRelationDefinitionSettings, BEParentChildRelationDefinition>();
        }

        public BEParentChildRelationDefinition GetPackageDefinitionById(Guid beParentChildRelationDefinitionId)
        {
            var packageDefinitions = GetBEParentChildRelationDefinitions();
            return packageDefinitions.FindRecord(x => x.VRComponentTypeId == beParentChildRelationDefinitionId);
        }

        //public IEnumerable<BEParentChildRelationDefinitionInfo> GetBEParentChildRelationDefinitionsInfo()
        //{
        //    var packageDefinitions = GetBEParentChildRelationDefinitions();
        //    return packageDefinitions.MapRecords(BEParentChildRelationDefinitionInfoMapper);
        //}

        //public IEnumerable<BEParentChildRelationDefinitionConfig> GetBEParentChildRelationDefinitionExtendedSettingsConfigs()
        //{
        //    var templateConfigManager = new ExtensionConfigurationManager();
        //    return templateConfigManager.GetExtensionConfigurations<BEParentChildRelationDefinitionConfig>(BEParentChildRelationDefinitionConfig.EXTENSION_TYPE);
        //}

        #endregion

        #region Mapper

        //public BEParentChildRelationDefinitionInfo BEParentChildRelationDefinitionInfoMapper(BEParentChildRelationDefinition beParentChildRelationDefinition)
        //{
        //    return new BEParentChildRelationDefinitionInfo
        //    {
        //        Name = beParentChildRelationDefinition.Name,
        //        BEParentChildRelationDefinitionId = beParentChildRelationDefinition.VRComponentTypeId,
        //        AccountBEDefinitionId = beParentChildRelationDefinition.Settings.AccountBEDefinitionId,
        //        RuntimeEditor = beParentChildRelationDefinition.Settings.ExtendedSettings.RuntimeEditor
        //    };
        //}

        #endregion
    }
}
