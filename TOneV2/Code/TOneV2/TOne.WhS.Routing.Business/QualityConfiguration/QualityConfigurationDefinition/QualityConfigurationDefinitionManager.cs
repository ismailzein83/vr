using System;
using System.Collections.Generic;
using TOne.WhS.Routing.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;

namespace TOne.WhS.Routing.Business
{
    public class QualityConfigurationDefinitionManager
    { 
        #region Public Methods

        public QualityConfigurationDefinition GetQualityConfigurationDefinition(Guid qualityConfigurationDefinitionId)
        {
            var beParentChildRelationDefinitions = this.GetCachedQualityConfigurationDefinitions();
            return beParentChildRelationDefinitions.GetRecord(qualityConfigurationDefinitionId);
        }

        public QualityConfigurationDefinitionExtendedSettings GetQualityConfigurationDefinitionExtendedSettings(Guid qualityConfigurationDefinitionId)
        {
            QualityConfigurationDefinition qualityConfigurationDefinition = this.GetQualityConfigurationDefinition(qualityConfigurationDefinitionId);
            qualityConfigurationDefinition.ThrowIfNull("qualityConfigurationDefinition", qualityConfigurationDefinitionId);
            qualityConfigurationDefinition.Settings.ThrowIfNull("qualityConfigurationDefinition.Settings", qualityConfigurationDefinitionId);
            qualityConfigurationDefinition.Settings.ExtendedSettings.ThrowIfNull("qualityConfigurationDefinition.Settings.ExtendedSettings", qualityConfigurationDefinitionId);
            return qualityConfigurationDefinition.Settings.ExtendedSettings;
        }

        public IEnumerable<QualityConfigurationDefinitionInfo> GetQualityConfigurationDefinitionInfo(QualityConfigurationDefinitionFilter filter)
        {
            Func<QualityConfigurationDefinition, bool> filterExpression = null;
            if (filter != null)
            {
                filterExpression = (qualityConfigurationDefinition) =>
                {
                    return true;
                };
            }

            var beParentChildRelationDefinitions = this.GetCachedQualityConfigurationDefinitions();
            return beParentChildRelationDefinitions.MapRecords(QualityConfigurationDefinitionInfoMapper, filterExpression);
        }

        public IEnumerable<QualityConfigurationDefinitionExtendedSettingsConfig> GetQualityConfigurationDefinitionExtendedSettingsConfigs()
        {
            var manager = new ExtensionConfigurationManager();
            return manager.GetExtensionConfigurations<QualityConfigurationDefinitionExtendedSettingsConfig>(QualityConfigurationDefinitionExtendedSettingsConfig.EXTENSION_TYPE);
        }

        #endregion

        #region Private Methods

        private Dictionary<Guid, QualityConfigurationDefinition> GetCachedQualityConfigurationDefinitions()
        {
            VRComponentTypeManager vrComponentTypeManager = new Vanrise.Common.Business.VRComponentTypeManager();
            return vrComponentTypeManager.GetCachedComponentTypes<QualityConfigurationDefinitionSettings, QualityConfigurationDefinition>();
        }

        #endregion

        #region Mappers

        public QualityConfigurationDefinitionInfo QualityConfigurationDefinitionInfoMapper(QualityConfigurationDefinition qualityConfigurationDefinition)
        {
            QualityConfigurationDefinitionExtendedSettings extendedSettings = GetQualityConfigurationDefinitionExtendedSettings(qualityConfigurationDefinition.VRComponentTypeId);

            return new QualityConfigurationDefinitionInfo
            {
                QualityConfigurationDefinitionId = qualityConfigurationDefinition.VRComponentTypeId,
                Name = qualityConfigurationDefinition.Name,
                RuntimeEditor = extendedSettings.RuntimeEditor
            };
        }

        #endregion
    }
}