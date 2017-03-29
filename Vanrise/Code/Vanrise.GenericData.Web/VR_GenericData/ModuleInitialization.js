﻿app.run(['VR_GenericData_DataRecordStorageService', 'VR_GenericData_DataStoreService', 'VR_GenericData_DataTransformationDefinitionService', 'VR_GenericData_GenericRuleDefinitionService', 'VR_GenericData_GenericRule', 'VR_GenericData_GenericBusinessEntityService', 'VR_GenericData_DataRecordTypeService', function (VR_GenericData_DataRecordStorageService, VR_GenericData_DataStoreService, VR_GenericData_DataTransformationDefinitionService, VR_GenericData_GenericRuleDefinitionService, VR_GenericData_GenericRule, VR_GenericData_GenericBusinessEntityService, VR_GenericData_DataRecordTypeService) {
    VR_GenericData_GenericRule.registerObjectTrackingDrillDownToGenericRule();
    VR_GenericData_GenericBusinessEntityService.registerObjectTrackingDrillDownToGenericBusinessEntity();
    VR_GenericData_DataRecordTypeService.registerObjectTrackingDrillDownToDataRecordType();
    VR_GenericData_GenericRuleDefinitionService.registerObjectTrackingDrillDownToGenericRuleDefinition();
    VR_GenericData_DataTransformationDefinitionService.registerObjectTrackingDrillDownToDataTransformationdefinition();
    VR_GenericData_DataStoreService.registerObjectTrackingDrillDownToDataStore();
    VR_GenericData_DataRecordStorageService.registerObjectTrackingDrillDownToDataRecordStorage();
}]);
