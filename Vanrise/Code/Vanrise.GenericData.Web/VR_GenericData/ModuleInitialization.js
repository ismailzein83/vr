app.run(['VR_GenericData_DataRecordFieldChoiceService', 'VR_GenericData_BELookupRuleDefinitionService', 'VR_GenericData_SummaryTransformationDefinitionService', 'VR_GenericData_DataRecordStorageService', 'VR_GenericData_DataStoreService', 'VR_GenericData_DataTransformationDefinitionService', 'VR_GenericData_GenericRuleDefinitionService', 'VR_GenericData_GenericRule', 'VR_GenericData_GenericBusinessEntityService', 'VR_GenericData_DataRecordTypeService', 'VR_GenericData_GenericBEActionService', 'VR_GenericData_GenericEditorConditionalRuleContainerService','VR_GenericData_GenericBECustomActionService',
    function (VR_GenericData_DataRecordFieldChoiceService, VR_GenericData_BELookupRuleDefinitionService, VR_GenericData_SummaryTransformationDefinitionService, VR_GenericData_DataRecordStorageService, VR_GenericData_DataStoreService, VR_GenericData_DataTransformationDefinitionService, VR_GenericData_GenericRuleDefinitionService, VR_GenericData_GenericRule, VR_GenericData_GenericBusinessEntityService, VR_GenericData_DataRecordTypeService, VR_GenericData_GenericBEActionService, VR_GenericData_GenericEditorConditionalRuleContainerService, VR_GenericData_GenericBECustomActionService) {
        VR_GenericData_GenericRule.registerObjectTrackingDrillDownToGenericRule();
        VR_GenericData_DataRecordTypeService.registerObjectTrackingDrillDownToDataRecordType();
        VR_GenericData_GenericRuleDefinitionService.registerObjectTrackingDrillDownToGenericRuleDefinition();
        VR_GenericData_DataTransformationDefinitionService.registerObjectTrackingDrillDownToDataTransformationdefinition();
        VR_GenericData_DataStoreService.registerObjectTrackingDrillDownToDataStore();
        VR_GenericData_DataRecordStorageService.registerObjectTrackingDrillDownToDataRecordStorage();
        VR_GenericData_SummaryTransformationDefinitionService.registerObjectTrackingDrillDownToSummaryTransformationDefinition();
        VR_GenericData_BELookupRuleDefinitionService.registerObjectTrackingDrillDownToBELookupRuleDefinition();
        VR_GenericData_DataRecordFieldChoiceService.registerObjectTrackingDrillDownToDataRecordFieldChoice();
        VR_GenericData_GenericBEActionService.registerEditBEAction();
        VR_GenericData_GenericBEActionService.registerDeleteBEAction();
        VR_GenericData_GenericBEActionService.registerSendEmailGenericBEAction();
        VR_GenericData_GenericEditorConditionalRuleContainerService.registerFieldValueConditionalRuleAction();
        VR_GenericData_GenericBECustomActionService.registerBulkAddCustomAction();
        VR_GenericData_GenericBEActionService.registerDownloadFileGenericBEAction();
        VR_GenericData_GenericBECustomActionService.registeNewOrExistingCustomAction();
    }]);
 