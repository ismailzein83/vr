using Retail.BusinessEntity.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.GenericData.Entities;
using Vanrise.Common;
using Vanrise.Entities;

namespace Retail.BusinessEntity.MainExtensions.PackageTypes
{
    public class VolumePackageDefinitionManager
    {
        public List<CompositeRecordConditionResolvedData> GetCompositeRecordConditionResolvedDataList(Guid volumePackageDefinitionId, Guid volumePackageDefinitionItemId)
        {
            List<CompositeRecordConditionResolvedData> compositeRecordConditionResolvedData = null;

            var volumePackageDefinition = new PackageDefinitionManager().GetPackageDefinitionById(volumePackageDefinitionId);
            volumePackageDefinition.ThrowIfNull("volumePackageDefinition", volumePackageDefinitionId);

            var volumePackageDefinitionSettings = volumePackageDefinition.Settings;
            volumePackageDefinitionSettings.ThrowIfNull("volumePackageDefinition.Settings", volumePackageDefinitionId);

            var extendedSettings = volumePackageDefinitionSettings.ExtendedSettings.CastWithValidate<VolumePackageDefinitionSettings>("volumePackageDefinition.Settings.ExtendedSettings", volumePackageDefinitionId);
            List<VolumePackageDefinitionItem> volumePackageDefinitionItems = extendedSettings.Items;
            volumePackageDefinitionItems.ThrowIfNull("volumePackageDefinitionItems", volumePackageDefinitionId);

            var volumePackageDefinitionItem = volumePackageDefinitionItems.FirstOrDefault(x => x.VolumePackageDefinitionItemId == volumePackageDefinitionItemId);
            volumePackageDefinitionItem.ThrowIfNull("volumePackageDefinitionItem", volumePackageDefinitionItemId);

            var compositeRecordConditionDefinitionGroup = volumePackageDefinitionItem.CompositeRecordConditionDefinitionGroup;
            if (compositeRecordConditionDefinitionGroup == null)
                throw new NullReferenceException(string.Format("compositeRecordConditionDefinitionGroup at volumePackageDefinition Id: '{0}'", volumePackageDefinitionId));

            var compositeRecordFilterDefinitions = volumePackageDefinitionItem.CompositeRecordConditionDefinitionGroup.CompositeRecordFilterDefinitions;
            if (compositeRecordFilterDefinitions == null)
                throw new NullReferenceException(string.Format("compositeRecordFilterDefinitions at volumePackageDefinition Id: '{0}'", volumePackageDefinitionId));

            if (compositeRecordFilterDefinitions.Count == 0)
                throw new VRBusinessException(string.Format("volumePackageDefinition Id: '{0}' should contains at least one compositeRecordFilterDefinition", volumePackageDefinitionId));

            compositeRecordConditionResolvedData = new List<CompositeRecordConditionResolvedData>();
            foreach (var compositeRecordFilterDefinition in compositeRecordFilterDefinitions)
            {
                var context = new CompositeRecordConditionDefinitionSettingsGetFieldsContext();
                compositeRecordFilterDefinition.Settings.GetFields(context);
                compositeRecordConditionResolvedData.Add(new CompositeRecordConditionResolvedData
                {
                    RecordName = compositeRecordFilterDefinition.Name,
                    Fields = context.Fields.Values.ToList()
                });
            }
            return compositeRecordConditionResolvedData;
        }
    }
}