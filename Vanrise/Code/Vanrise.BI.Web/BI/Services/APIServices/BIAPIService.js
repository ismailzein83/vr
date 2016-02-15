(function (appControllers) {

     "use strict";
     BIAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_BI_ModuleConfig'];

     function BIAPIService(BaseAPIService, UtilsService, VR_BI_ModuleConfig) {

         function GetUserMeasuresValidator(userMeasuresValidatorInput) {
             return BaseAPIService.post(UtilsService.getServiceURL(VR_BI_ModuleConfig.moduleName, "BI", "GetUserMeasuresValidator"), userMeasuresValidatorInput);
         }

         function ExportTopEntities(input) {
             return BaseAPIService.post(UtilsService.getServiceURL(VR_BI_ModuleConfig.moduleName, "BI", "ExportTopEntities"), input, {
                     returnAllResponseParameters: true,
                     responseTypeAsBufferArray: true
                 });
         }

         function ExportMeasureValues(input) {
             return BaseAPIService.post(UtilsService.getServiceURL(VR_BI_ModuleConfig.moduleName, "BI", "ExportMeasureValues"), input, {
                     returnAllResponseParameters: true,
                     responseTypeAsBufferArray: true
                 });
         }

         function GetSummaryMeasureValues(fromDate, toDate,timeEntityName, measureTypesNames) {
             return BaseAPIService.post(UtilsService.getServiceURL(VR_BI_ModuleConfig.moduleName, "BI", "GetSummaryMeasureValues"),
                 {
                     FromDate: fromDate,
                     ToDate: toDate,
                     TimeEntityName: timeEntityName,
                     MeasureTypesNames: measureTypesNames
                 });
         }

         function GetTopEntities(input) {
             return BaseAPIService.post(UtilsService.getServiceURL(VR_BI_ModuleConfig.moduleName, "BI", "GetTopEntities"), input);
         }

         function GetMeasureValues(input) {
             return BaseAPIService.post(UtilsService.getServiceURL(VR_BI_ModuleConfig.moduleName, "BI", "GetMeasureValues"), input);
         }

         function GetEntityMeasuresValues(entityTypeName, entityId, timeDimensionType, fromDate, toDate,timeEntityName, measureTypesNames) {
             return BaseAPIService.post(UtilsService.getServiceURL(VR_BI_ModuleConfig.moduleName, "BI", "GetEntityMeasuresValues"),
                 {
                     EntityTypeName: entityTypeName,
                     EntityId: entityId,
                     TimeDimensionType: timeDimensionType,
                     FromDate: fromDate,
                     ToDate: toDate,
                     TimeEntityName: timeEntityName,
                     MeasureTypesNames: measureTypesNames
                 });
         }

         return ({
             GetMeasureValues: GetMeasureValues,
             GetEntityMeasuresValues: GetEntityMeasuresValues,
             GetTopEntities: GetTopEntities,
             GetSummaryMeasureValues: GetSummaryMeasureValues,
             ExportMeasureValues: ExportMeasureValues,
             ExportTopEntities: ExportTopEntities,
             GetUserMeasuresValidator: GetUserMeasuresValidator
         });
     }

     appControllers.service('VR_BI_BIAPIService', BIAPIService);

 })(appControllers);