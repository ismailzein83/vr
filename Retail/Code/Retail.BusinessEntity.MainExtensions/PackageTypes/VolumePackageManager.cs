//using Retail.BusinessEntity.Business;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Vanrise.GenericData.Entities;

//namespace Retail.BusinessEntity.MainExtensions.PackageTypes
//{
//    public class VolumePackageManager
//    {
//        public List<CompositeRecordConditionResolvedData> GetCompositeRecordConditionResolvedDataList(Guid volumePackageDefinitionId, Guid volumePackageDefinitionItemId)
//        {
//            List<CompositeRecordConditionResolvedData> compositeRecordConditionResolvedData = null;

//            var volumePackageDefinition = new PackageDefinitionManager().GetPackageDefinitionById(volumePackageDefinitionId);

//            if (volumePackageDefinition != null)
//            {
//                var settings = (VolumePackageDefinitionSettings)volumePackageDefinition.Settings.ExtendedSettings;

//                if (settings != null && settings.Items != null && settings.Items.Count > 0)
//                {
//                    var volumePackageDefinitionItem = settings.Items.Find(x => x.VolumePackageDefinitionItemId == volumePackageDefinitionItemId);

//                    var compositeRecordFilterDefinitions = volumePackageDefinitionItem.CompositeRecordConditionDefinitionGroup.CompositeRecordFilterDefinitions;
//                    if (compositeRecordFilterDefinitions != null && compositeRecordFilterDefinitions.Count > 0)
//                    {
//                        compositeRecordConditionResolvedData = new List<CompositeRecordConditionResolvedData>();
//                        foreach (var compositeRecordFilterDefinition in compositeRecordFilterDefinitions)
//                        {
//                            var context = new CompositeRecordConditionDefinitionSettingsGetFieldsContext();
//                            compositeRecordFilterDefinition.Settings.GetFields(context);
//                            compositeRecordConditionResolvedData.Add(new CompositeRecordConditionResolvedData
//                            {
//                                RecordName = compositeRecordFilterDefinition.Name,
//                                Fields = context.Fields.Values.ToList()
//                            });
//                        }
//                    }
//                }
//            }
//            return compositeRecordConditionResolvedData;
//        }
//    }
//}