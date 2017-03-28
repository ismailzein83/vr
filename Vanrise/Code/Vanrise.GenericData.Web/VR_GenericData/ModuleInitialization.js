﻿app.run(['VR_GenericData_GenericRuleDefinitionService', 'VR_GenericData_GenericRule', 'VR_GenericData_GenericBusinessEntityService', 'VR_GenericData_DataRecordTypeService', function (VR_GenericData_GenericRuleDefinitionService, VR_GenericData_GenericRule, VR_GenericData_GenericBusinessEntityService, VR_GenericData_DataRecordTypeService) {
    VR_GenericData_GenericRule.registerObjectTrackingDrillDownToGenericRule();
    VR_GenericData_GenericBusinessEntityService.registerObjectTrackingDrillDownToGenericBusinessEntity();
    VR_GenericData_DataRecordTypeService.registerObjectTrackingDrillDownToDataRecordType();
    VR_GenericData_GenericRuleDefinitionService.registerObjectTrackingDrillDownToGenericRuleDefinition();
}]);
