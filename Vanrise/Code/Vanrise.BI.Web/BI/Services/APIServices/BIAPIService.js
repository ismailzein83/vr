(function (appControllers) {

     "use strict";
     BIAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_BI_ModuleConfig'];

     function BIAPIService(BaseAPIService, UtilsService, VR_BI_ModuleConfig) {

         function GetUserMeasuresValidator(userMeasuresValidatorInput) {
             return BaseAPIService.post(UtilsService.getServiceURL(VR_BI_ModuleConfig.moduleName, "BI", "GetUserMeasuresValidator"), userMeasuresValidatorInput);
         }

         function ExportTopEntities(entityTypeName, topByMeasureTypeName, fromDate, toDate, topCount, measureTypesNames) {
             return BaseAPIService.post(UtilsService.getServiceURL(VR_BI_ModuleConfig.moduleName, "BI", "ExportTopEntities"),
                 {
                     EntityTypeName: entityTypeName,
                     TopByMeasureTypeName: topByMeasureTypeName,
                     FromDate: fromDate,
                     ToDate: toDate,
                     TopCount: topCount,
                     MeasureTypesNames: measureTypesNames,
                     TimeEntity:timeEntityName
                 }, {
                     returnAllResponseParameters: true,
                     responseTypeAsBufferArray: true
                 });
         }

         function ExportMeasureValues(timeDimensionType, fromDate, toDate, timeEntityName, measureTypesNames) {
             return BaseAPIService.post(UtilsService.getServiceURL(VR_BI_ModuleConfig.moduleName, "BI", "ExportMeasureValues"),
                 {
                     TimeDimensionType: timeDimensionType,
                     FromDate: fromDate,
                     ToDate: toDate,
                     MeasureTypesNames: measureTypesNames,
                     TimeEntity: timeEntityName
                 }, {
                     returnAllResponseParameters: true,
                     responseTypeAsBufferArray: true
                 });
         }

         function GetSummaryMeasureValues(fromDate, toDate,timeEntityName, measureTypesNames) {
             return BaseAPIService.post(UtilsService.getServiceURL(VR_BI_ModuleConfig.moduleName, "BI", "GetSummaryMeasureValues"),
                 {
                     FromDate: fromDate,
                     ToDate: toDate,
                     TimeEntityId: timeEntityName,
                     MeasureTypesNames: measureTypesNames
                 });
         }

         function GetTopEntities(entityTypeName, topByMeasureTypeName, fromDate, toDate, topCount,timeEntityName, measureTypesNames) {
             return BaseAPIService.post(UtilsService.getServiceURL(VR_BI_ModuleConfig.moduleName, "BI", "GetTopEntities"),
                 {
                     EntityTypeName: entityTypeName,
                     TopByMeasureTypeName: topByMeasureTypeName,
                     FromDate: fromDate,
                     ToDate: toDate,
                     TopCount: topCount,
                     TimeEntityId: timeEntityName,
                     MeasureTypesNames: measureTypesNames
                 });
         }

         function GetMeasureValues(timeDimensionType, fromDate, toDate,timeEntityName, measureTypesNames) {
             return BaseAPIService.post(UtilsService.getServiceURL(VR_BI_ModuleConfig.moduleName, "BI", "GetMeasureValues"),
                 {
                     TimeDimensionType: timeDimensionType,
                     FromDate: fromDate,
                     ToDate: toDate,
                     TimeEntityId: timeEntityName,
                     MeasureTypesNames: measureTypesNames
                 });
         }

         function GetEntityMeasuresValues(entityTypeName, entityId, timeDimensionType, fromDate, toDate,timeEntityName, measureTypesNames) {
             return BaseAPIService.post(UtilsService.getServiceURL(VR_BI_ModuleConfig.moduleName, "BI", "GetEntityMeasuresValues"),
                 {
                     EntityTypeName: entityTypeName,
                     EntityId: entityId,
                     TimeDimensionType: timeDimensionType,
                     FromDate: fromDate,
                     ToDate: toDate,
                     TimeEntityId: timeEntityName,
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